using ExampleApp.DataAccess;
using ExampleApp.DataAccess.Entities;
using ExampleApp.Web.Extensions;
using ExampleApp.Web.Models.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace ExampleApp.Web.Controllers;

[ApiController]
[Route("identity")]
public class IdentityController : ApiBaseController
{
    private readonly UserManager<AppUser> _userManager;
    private readonly JwtOptions _jwtOptions;
    private readonly AppDbContext _context;

    public IdentityController(UserManager<AppUser> userManager, IOptions<JwtOptions> jwtOptions, AppDbContext context)
    {
        _userManager = userManager;
        _jwtOptions = jwtOptions.Value;
        _context = context;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var user = await _context.Users.Include(x=>x.Scopes).FirstOrDefaultAsync(x => x.UserName == request.UserName);

        if (user == null)
        {
            return BadRequest();
        }

        if (!await _userManager.CheckPasswordAsync(user, request.Password)) return Unauthorized();
        
        var (refreshToken, refreshTokenExpireAt) = user.GenerateRefreshToken(_jwtOptions);
        user.RefreshToken = refreshToken;
        await _context.SaveChangesAsync();
        
        return Ok(new LoginResponse
        {
            AccessToken = user.GenerateAccessToken(_jwtOptions), 
            RefreshToken = refreshToken,
            RefreshTokenExpireAt = refreshTokenExpireAt
        });
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        var user = new AppUser
        {
            Email = request.Email,
            Firstname = request.Firstname,
            Lastname = request.Lastname,
            Patronymic = request.Patronymic,
            BirthDate = request.BirthDate,
            UserName = request.UserName
        };
        var result = await _userManager.CreateAsync(user, request.Password);

        if (result.Succeeded)
        {
            return await Login(new LoginRequest
            {
                UserName = request.UserName,
                Password = request.Password
            });
        }
        
        return Unauthorized();
    }

    [HttpGet("profile")]
    [Authorize(Scope.ProfileRead)]
    public async Task<IActionResult> GetProfile()
    {
        var user = await _context.Users
            .Include(x => x.Scopes)
            .FirstOrDefaultAsync(x => x.Id == CurrentUser.Id);
        
        return Ok(user);
    }
}