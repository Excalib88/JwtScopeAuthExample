namespace ExampleApp.Web.Models.Identity;

public class RegisterRequest: LoginRequest
{
    public string Email { get; set; } = null!;
    public string RetypePassword { get; set; } = null!;
    public string? Firstname { get; set; }
    public string? Lastname { get; set; }
    public string? Patronymic { get; set; }
    public DateTime? BirthDate { get; set; }
}