using System.Text;
using System.Text.Json;
using ExampleApp.DataAccess;
using ExampleApp.DataAccess.Entities;
using ExampleApp.Web;
using ExampleApp.Web.Extensions;
using ExampleApp.Web.Models.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(o => 
    o.UseNpgsql(builder.Configuration.GetConnectionString("Db"), 
        x => x.MigrationsAssembly("ExampleApp.DataAccess")));

builder.Services.AddControllers();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<DataSeeder>();
builder.Services.AddScoped<UserManager<AppUser>>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddIdentity<AppUser, IdentityRole<long>>(options =>
    {
        options.User.RequireUniqueEmail = false;
    })
    .AddEntityFrameworkStores<AppDbContext>();
builder.Services.AddAuthentication(opt => {
        opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:ValidIssuer"],
            ValidAudience = builder.Configuration["Jwt:ValidAudience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Secret"]))
        };
        options.Events = new JwtBearerEvents
        {
            OnChallenge = context =>
            {
                context.HandleResponse();
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                context.Response.ContentType = "application/json";
        
                if (string.IsNullOrEmpty(context.Error))
                    context.Error = "invalid_token";
                if (string.IsNullOrEmpty(context.ErrorDescription))
                    context.ErrorDescription = "This request requires a valid JWT access token to be provided";
        
                if (context.AuthenticateFailure == null ||
                    context.AuthenticateFailure.GetType() != typeof(SecurityTokenExpiredException))
                    return context.Response.WriteAsync(JsonSerializer.Serialize(new
                    {
                        error = context.Error,
                        error_description = context.ErrorDescription
                    }));
                var authenticationException = context.AuthenticateFailure as SecurityTokenExpiredException;
                context.Response.Headers.Add("x-token-expired", authenticationException?.Expires.ToString("o"));
                context.ErrorDescription =
                    $"The token expired on {authenticationException?.Expires:o}";
        
                return context.Response.WriteAsync(JsonSerializer.Serialize(new
                {
                    error = context.Error,
                    error_description = context.ErrorDescription
                }));
            }
        };
    });
builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("Jwt"));
builder.Services.AddSingleton<IAuthorizationHandler, RequireScopeHandler>();
builder.Services.AddAuthorization(options =>
{ 
    Array.ForEach(builder.Services.BuildServiceProvider().GetRequiredService<AppDbContext>().Scopes.ToArray(), scope => 
        options.AddPolicy(scope.Name, policy => policy.Requirements.Add(
            new ScopeRequirement(builder.Configuration["Jwt:ValidIssuer"], scope.Name))));
});

var app = builder.Build();

// seed scopes
app.UseDataSeeder();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();