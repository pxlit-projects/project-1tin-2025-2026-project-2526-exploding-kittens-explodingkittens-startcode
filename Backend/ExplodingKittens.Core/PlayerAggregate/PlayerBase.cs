using ExplodingKittens.Core.PlayerAggregate.Contracts;

namespace ExplodingKittens.Core.PlayerAggregate;

/// <inheritdoc cref="IPlayer"/>
internal class PlayerBase
{
    protected PlayerBase(Guid id, string name, DateOnly birthDate)
    {
        
    }
}