namespace ExampleApp.Web.Models.Identity;

public class JwtOptions
{
    public string Secret { get; set; } = null!;
    public string ValidIssuer { get; set; } = null!;
    public string ValidAudience { get; set; } = null!;
    public int Expire { get; set; } = 600;
    public int RefreshTokenExpire { get; set; } = 20160;
}