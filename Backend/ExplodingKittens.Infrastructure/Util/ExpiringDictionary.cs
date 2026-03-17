using System.Collections.Concurrent;

namespace ExplodingKittens.Infrastructure.Util;

/// <summary>
/// A dictionary that automatically removes entries after a specified lifespan.
/// </summary>
/// <typeparam name="TKey">The type of the keys in the dictionary.</typeparam>
/// <typeparam name="TValue">The type of the values in the dictionary.</typeparam>
public class ExpiringDictionary<TKey, TValue> where TKey : notnull
{
    private readonly TimeSpan _entryLifeSpan;
    private readonly ConcurrentDictionary<TKey, Entry> _entries;
    private DateTimeOffset _lastExpirationScan;

    /// <summary>
    /// Gets a read-only list containing the values of all entries in the dictionary.
    /// </summary>
    public IReadOnlyList<TValue> Values
    {
        get
        {
            return _entries.Values.Select(entry => entry.Value).ToList();
        }
    }

    /// <summary>
    /// Initializes a new instance of the ExpiringDictionary class with a default entry lifespan of 180 seconds.
    /// </summary>
    public ExpiringDictionary()
    {
        _entries = new ConcurrentDictionary<TKey, Entry>();
        _entryLifeSpan = TimeSpan.FromSeconds(180);
    }

    /// <summary>
    /// Initializes a new instance of the ExpiringDictionary class with a specified lifespan for each entry.
    /// </summary>
    /// <param name="entryLifeSpan">The duration that each entry remains in the dictionary before it expires.</param>
    public ExpiringDictionary(TimeSpan entryLifeSpan)
    {
        _entryLifeSpan = entryLifeSpan;
        _entries = new ConcurrentDictionary<TKey, Entry>();
    }

    /// <summary>
    /// Adds a new entry with the specified key and value, or replaces the value of an existing entry with the specified
    /// key.
    /// </summary>
    /// <param name="key">The key associated with the entry to add or replace.</param>
    /// <param name="value">The value to associate with the specified key.</param>
    public void AddOrReplace(TKey key, TValue value)
    {
        _entries.AddOrUpdate(key, k => new Entry(value, _entryLifeSpan), (k, oldValue) => new Entry(value, _entryLifeSpan));
        StartScanForExpiredEntries();
    }

    /// <summary>
    /// Tries to get the value associated with the specified key.
    /// </summary>
    /// <param name="key">The key of the value to get.</param>
    /// <param name="value">When this method returns, contains the value associated with the specified key, if the key is found; otherwise, the default value for the type of the value parameter.</param>
    /// <returns>true if the dictionary contains an element with the specified key; otherwise, false.</returns>
    public bool TryGetValue(TKey key, out TValue? value)
    {
        bool result = false;
        value = default;
        if (_entries.TryGetValue(key, out Entry? entry))
        {
            value = entry.Value;
            result = true;
        }

        StartScanForExpiredEntries();

        return result;
    }

    /// <summary>
    /// Attempts to remove the value with the specified key from the dictionary.
    /// </summary>
    /// <remarks>This method is thread-safe and can be called concurrently from multiple threads.</remarks>
    /// <param name="key">The key of the element to remove. This parameter cannot be null.</param>
    /// <param name="removedValue">When this method returns, contains the value that was removed if the key was found; otherwise, the default value
    /// for the type of the value parameter.</param>
    /// <returns>true if the element is successfully removed; otherwise, false.</returns>
    public bool TryRemove(TKey key, out TValue? removedValue)
    {
        if (_entries.TryRemove(key, out Entry? removedEntry))
        {
            removedValue = removedEntry.Value;
            return true;
        }

        removedValue = default;
        return false;
    }

    private void StartScanForExpiredEntries()
    {
        var now = DateTimeOffset.Now;
        if (TimeSpan.FromSeconds(30) < now - _lastExpirationScan)
        {
            _lastExpirationScan = now;
            Task.Factory.StartNew(state => ScanForExpiredItems((ConcurrentDictionary<TKey, Entry>)state!), _entries,
                CancellationToken.None, TaskCreationOptions.DenyChildAttach, TaskScheduler.Default);
        }
    }

    private static void ScanForExpiredItems(ConcurrentDictionary<TKey, Entry> dictionary)
    {
        var now = DateTimeOffset.Now;
        foreach (var keyValue in dictionary)
        {
            if (keyValue.Value.IsExpired(now))
            {
                dictionary.TryRemove(keyValue.Key, out Entry? _);
            }
        }
    }

    private class Entry
    {
        private readonly DateTimeOffset _expiration;

        public TValue Value { get; }

        public Entry(TValue value, TimeSpan lifeTime)
        {
            Value = value;
            _expiration = DateTimeOffset.Now.Add(lifeTime);
        }

        public bool IsExpired(DateTimeOffset now)
        {
            return now >= _expiration;
        }
    }
}