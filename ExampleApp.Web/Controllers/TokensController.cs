using System.Text.Json;
using ExampleApp.DataAccess;
using ExampleApp.Web.Extensions;
using ExampleApp.Web.Models.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace ExampleApp.Web.Controllers;

[Route("tokens")]
public class TokensController : ApiBaseController
{
    private readonly JwtOptions _jwtOptions;
    private readonly AppDbContext _context;

    public TokensController(IOptions<JwtOptions> jwtOptions, AppDbContext context)
    {
        _context = context;
        _jwtOptions = jwtOptions.Value;
    }

    [HttpPost]
    [Route("refresh")]
    public async Task<IActionResult> Refresh(string refreshToken)
    {
        var decryptedData = AesEncryptor.Decrypt(_jwtOptions.Secret, refreshToken);
        var data = JsonSerializer.Deserialize<RefreshTokenData>(decryptedData);

        if (data == null)
        {
            return BadRequest();
        }

        if (data.Expire < DateTime.UtcNow)
        {
            return BadRequest();
        }
        
        var user = await _context.Users.Include(x => x.Scopes).FirstOrDefaultAsync(x => x.Id == data.UserId);

        if (user == null) return BadRequest();

        var (newRefreshToken, refreshTokenExpireAt) = user.GenerateRefreshToken(_jwtOptions);
        
        return Ok(new LoginResponse
        {
            AccessToken = user.GenerateAccessToken(_jwtOptions),
            RefreshToken = newRefreshToken,
            RefreshTokenExpireAt = refreshTokenExpireAt
        });
    }
}