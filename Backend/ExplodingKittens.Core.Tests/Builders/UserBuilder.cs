using ExplodingKittens.Core.Tests.Extensions;
using ExplodingKittens.Core.UserAggregate;

namespace ExplodingKittens.Core.Tests.Builders;

public class UserBuilder
{
    private readonly User _user = new()
    {
        Id = Guid.NewGuid(),
        Email = Guid.NewGuid().ToString(),
        BirthDate = Random.Shared.NextDateOnly(1980, 2015),
        UserName = Guid.NewGuid().ToString(),
        PasswordHash = Guid.NewGuid().ToString()
    };

    public UserBuilder AsCloneOf(User user)
    {
        _user.Id = user.Id;
        _user.Email = user.Email;
        _user.BirthDate = user.BirthDate;
        _user.UserName = user.UserName;
        _user.PasswordHash = user.PasswordHash;
        return this;
    }

    public UserBuilder WithUserName(string userName)
    {
        _user.UserName = userName;
        return this;
    }

    public UserBuilder WithBirthDate(DateOnly birthDate)
    {
        _user.BirthDate = birthDate;
        return this;
    }

    public User Build()
    {
        return _user;
    }
}