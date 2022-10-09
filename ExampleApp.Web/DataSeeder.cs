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
            
        await _context.Scopes.AddRangeAsync(
            new ScopeEntity(Scope.ProfileRead), 
            new ScopeEntity(Scope.ProfileAllRead), 
            new ScopeEntity(Scope.ProfileWrite), 
            new ScopeEntity(Scope.ProfileAllWrite), 
            new ScopeEntity(Scope.ScopesRead), 
            new ScopeEntity(Scope.ScopesAllRead), 
            new ScopeEntity(Scope.ScopesWrite), 
            new ScopeEntity(Scope.ScopesAllWrite));
        await _context.SaveChangesAsync();
    }
}