using Microsoft.AspNetCore.Identity;

namespace ExampleApp.DataAccess.Entities;

public class AppUser : IdentityUser<long>
{
    public string? Firstname { get; set; }
    public string? Lastname { get; set; }
    public string? Patronymic { get; set; }
    public DateTime? BirthDate { get; set; }
    public ICollection<ScopeEntity>? Scopes { get; set; }
}