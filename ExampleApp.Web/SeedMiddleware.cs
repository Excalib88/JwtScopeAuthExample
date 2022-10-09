namespace ExampleApp.Web;

public class SeedMiddleware
{
    private readonly RequestDelegate _next;
    
    public SeedMiddleware(RequestDelegate next)
    {
        _next = next;
    }
    
    public async Task Invoke(HttpContext context, DataSeeder seeder)
    {
        await seeder.Execute();
        await _next.Invoke(context);
    }
}