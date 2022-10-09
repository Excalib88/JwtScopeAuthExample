using ExampleApp.DataAccess;
using ExampleApp.DataAccess.Entities;
using ExampleApp.Web.Models.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ExampleApp.Web.Controllers;

[ApiController]
[Route("scopes")]
public class ScopesController : ApiBaseController
{
    private readonly AppDbContext _context;

    public ScopesController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    [Authorize(Scope.ScopesAllRead)]
    public async Task<IActionResult> GetAll()
    {
        var scopes = await _context.Scopes.Select(x => x.Name).ToListAsync();
        var scopeString = string.Join(' ', scopes);
        
        return Ok(new {scopes, scopeString});
    }

    [HttpGet("my")]
    [Authorize(Scope.ScopesRead)]
    public async Task<IActionResult> GetMy()
    {
        var user = await _context.Users
            .Include(x => x.Scopes)
            .FirstOrDefaultAsync(x => x.Id == CurrentUser.Id);
        var scopes = string.Join(' ', user?.Scopes?.Select(x => x.Name) ?? Array.Empty<string>());
        
        return Ok(new {scopes});
    }

    [HttpPost]
    [Authorize(Scope.ScopesWrite)]
    public async Task<IActionResult> Create(string scopeName)
    {
        await _context.Scopes.AddAsync(new ScopeEntity
        {
            Name = scopeName
        });
        await _context.SaveChangesAsync();

        return Ok();
    }

    [HttpPost("{userId:long}")]
    [Authorize(Scope.ScopesWrite)]
    public async Task<IActionResult> AddToUser(long userId, string scopeName)
    {
        var user = await _context.Users.Include(x => x.Scopes).FirstOrDefaultAsync(x => x.Id == userId);
        var scope = await _context.Scopes.Include(x => x.Users).FirstOrDefaultAsync(x => x.Name == scopeName);
        
        if (user == null) return BadRequest("User not found");
        if (scope == null) return BadRequest("Scope not found");
        
        user.Scopes ??= new List<ScopeEntity>();
        user.Scopes?.Add(scope);
        
        _context.Users.Update(user);
        await _context.SaveChangesAsync();

        return Ok();
    }

    [HttpDelete("{userId:long}")]
    [Authorize(Scope.ScopesWrite)]
    public async Task<IActionResult> RemoveFromUser(long userId, string scopeName)
    {
        var user = await _context.Users.Include(x => x.Scopes).FirstOrDefaultAsync(x => x.Id == userId);
        var scope = await _context.Scopes.Include(x => x.Users).FirstOrDefaultAsync(x => x.Name == scopeName);
        
        if (user == null) return BadRequest("User not found");
        if (scope == null) return BadRequest("Scope not found");
        
        user.Scopes ??= new List<ScopeEntity>();
        user.Scopes?.Remove(scope);
        
        _context.Users.Update(user);
        await _context.SaveChangesAsync();

        return Ok();
    }
}