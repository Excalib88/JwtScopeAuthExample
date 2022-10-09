using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ExampleApp.Web.Models.Identity;

public class UserInfo
{
    public long Id { get; set; }
    public string? UserName { get; set; }
    public string? Email { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Patronymic { get; set; }
    public DateTime? BirthDate { get; set; }
    public string? Scope { get; set; }

    public UserInfo(ClaimsPrincipal claims)
    {
        Id = int.Parse(claims.FindFirstValue(JwtRegisteredClaimNames.NameId));
        UserName = claims.FindFirstValue(JwtRegisteredClaimNames.Name);
        Email = claims.FindFirstValue(JwtRegisteredClaimNames.Email);
        FirstName = claims.FindFirstValue(JwtRegisteredClaimNames.GivenName);
        LastName = claims.FindFirstValue(JwtRegisteredClaimNames.FamilyName);
        Patronymic = claims.FindFirstValue("middle_name");
        BirthDate = DateTime.Parse(claims.FindFirstValue(JwtRegisteredClaimNames.Birthdate));
        Scope = claims.FindFirstValue("scope");
    }
}