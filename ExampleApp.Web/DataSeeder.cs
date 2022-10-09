using ExampleApp.DataAccess;
using ExampleApp.DataAccess.Entities;
using ExampleApp.Web.Models.Identity;

namespace ExampleApp.Web;

public class DataSeeder
{
    private readonly AppDbContext _context;

    public DataSeeder(AppDbContext context)
    {
        _context = context;
    }

    public async Task Execute()
    {
        if (_context.Scopes.Any())
        {
            return;
        }

        var scopes = new[]
        {
            Scope.ProfileRead,
            Scope.ProfileAllRead,
            Scope.ProfileWrite,
            Scope.ProfileAllWrite,
            Scope.ScopesRead,
            Scope.ScopesAllRead,
            Scope.ScopesWrite,
            Scope.ScopesAllWrite
        };
        
        await _context.Scopes.AddRangeAsync(scopes.Select(x => new ScopeEntity(x)));
        await _context.SaveChangesAsync();
    }
}