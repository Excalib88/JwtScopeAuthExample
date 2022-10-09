namespace ExampleApp.Web.Extensions;

public static class StringRandomizer
{
    private const string Chars = @"AaBbCcDdEeFfGgHhIiJjKkLlMmNnOoPpQqRrSsTtUuVvWwXxYyZz0123456789-+=/\:;!@#$%^&*()_";
    private static readonly Random Random = new();
    
    public static string Randomize(int length = 16)
    {
        return new string(Enumerable.Repeat(Chars, length)
            .Select(s => s[Random.Next(s.Length)]).ToArray());
    }
}