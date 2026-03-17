namespace ExplodingKittens.Api.Models.Output;

public class UserModel
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public DateOnly BirthDate { get; set; }
}