using Microsoft.AspNetCore.Identity;

namespace ExplodingKittens.Core.UserAggregate;

public class User : IdentityUser<Guid>
{
    public DateOnly BirthDate { get; set; }
}