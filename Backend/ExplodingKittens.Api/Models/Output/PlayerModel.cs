

namespace ExplodingKittens.Api.Models.Output;

public class PlayerModel
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateOnly BirthDate { get; set; }
}