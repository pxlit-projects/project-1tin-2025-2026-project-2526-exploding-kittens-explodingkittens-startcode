namespace ExplodingKittens.Api.Models.Output;

public class AccessPassModel
{
    public UserModel User { get; set; } = new UserModel();
    public string Token { get; set; } = string.Empty;
}