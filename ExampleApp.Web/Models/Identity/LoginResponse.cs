namespace ExampleApp.Web.Models.Identity;

public class LoginResponse
{
    public string AccessToken { get; set; } = null!;
    public string RefreshToken { get; set; } = null!;
    public DateTime RefreshTokenExpireAt { get; set; }
}