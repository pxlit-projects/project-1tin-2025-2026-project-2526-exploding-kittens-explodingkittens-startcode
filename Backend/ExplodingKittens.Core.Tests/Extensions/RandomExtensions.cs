using ExplodingKittens.Core.CardAggregate;

namespace ExplodingKittens.Core.Tests.Extensions;

public static class RandomExtensions
{
    extension(Random random)
    {
        public T NextItem<T>(IEnumerable<T> enumerable)
        {
            if (enumerable is null)
            {
                throw new ArgumentNullException(nameof(enumerable));
            }
            List<T> list = enumerable.ToList();
            if (!list.Any())
            {
                throw new ArgumentException("The enumerable is empty.", nameof(enumerable));
            }
            int index = random.Next(0, list.Count);
            return list.ElementAt(index);
        }

        public DateOnly NextDateOnly(int minimumYear, int maximumYear)
        {
            int year = random.Next(minimumYear, maximumYear + 1);
            int month = random.Next(1, 13);
            int day = random.Next(1, 29); // Simplified for brevity
            return new DateOnly(year, month, day);
        }

        public Card NextCard()
        {
            return random.NextItem(Enum.GetValues<Card>());
        }

        public bool NextBool()
        {
            return random.Next(0, 2) == 0;
        }
    }
}