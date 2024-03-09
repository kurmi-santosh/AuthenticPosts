using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;

const string AuthScheme = "cookie";

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddAuthentication(AuthScheme)
       .AddCookie(AuthScheme);

var app = builder.Build();

app.UseAuthentication();

app.MapGet("/username", (HttpContext ctx) =>
{
    return ctx.User.FindFirst("usr")?.Value;
});

app.MapGet("/login", async (HttpContext ctx) =>
{
    var claims = new List<Claim>();
    claims.Add(new Claim("usr", "naidu"));

    var identity = new ClaimsIdentity(claims, AuthScheme);
    var userCP = new ClaimsPrincipal(identity);

    await ctx.SignInAsync(AuthScheme, userCP);
    return "ok";
});

app.Run();
