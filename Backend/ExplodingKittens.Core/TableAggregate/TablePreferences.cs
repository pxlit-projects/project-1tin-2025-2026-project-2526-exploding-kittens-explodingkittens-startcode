using System.ComponentModel;
using ExplodingKittens.Core.TableAggregate.Contracts;

namespace ExplodingKittens.Core.TableAggregate;

/// <inheritdoc cref="ITablePreferences"/>
public class TablePreferences : ITablePreferences
{

    [DefaultValue(2)]
    public int NumberOfPlayers { get; set; }

    [DefaultValue(0)]
    public int NumberOfArtificialPlayers { get; set; }

    public TablePreferences()
    {
        NumberOfPlayers = 2;
        NumberOfArtificialPlayers = 0;
    }

    //DO NOT CHANGE THE CODE BELOW, unless (maybe) when you are working on EXTRA requirements
    public override bool Equals(object? other)
    {
        if (other == null) return false;
        if (other is ITablePreferences otherPreferences)
        {
            if( NumberOfPlayers != otherPreferences.NumberOfPlayers) return false;
            if (NumberOfArtificialPlayers != otherPreferences.NumberOfArtificialPlayers) return false;
        }
        return true;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(NumberOfPlayers, NumberOfArtificialPlayers);
    }
}