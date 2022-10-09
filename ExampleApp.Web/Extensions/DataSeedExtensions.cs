namespace ExampleApp.Web.Extensions;

public static class DataSeedExtensions
{
    public static IApplicationBuilder UseDataSeeder(this IApplicationBuilder app)
    {
        return app != null ? app.UseMiddleware<SeedMiddleware>() : throw new ArgumentNullException(nameof (app));
    }
}