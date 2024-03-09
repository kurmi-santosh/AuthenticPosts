using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;

const string AuthScheme = "cookie";

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddAuthentication(AuthScheme)
       .AddCookie(AuthScheme);

builder.Services.AddAuthorization(authBuilder =>
{
    authBuilder.AddPolicy("swedan-passport", policyBuilder =>
    {
        // We need 1. Authenticated user, 2. With cookie auth scheme having 3. Claim nation:Swedan
        policyBuilder.RequireAuthenticatedUser()
            .AddAuthenticationSchemes(AuthScheme)
            .AddRequirements()
            .RequireClaim("nation", "swedan");
           
    });
});

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/username", (HttpContext ctx) =>
{
    return ctx.User.FindFirst("usr")?.Value;
});

app.MapGet("/swedan", (HttpContext ctx) =>
{
    return "authorized";
    
}).RequireAuthorization("swedan-passport");


app.MapGet("/login", async (HttpContext ctx) =>
{
    var claims = new List<Claim>();
    claims.Add(new Claim("usr", "naidu"));
    claims.Add(new Claim("nation", "swedan"));
    var identity = new ClaimsIdentity(claims, AuthScheme);
    var userCP = new ClaimsPrincipal(identity);

    await ctx.SignInAsync(AuthScheme, userCP);
    return "ok";
}).AllowAnonymous();

app.Run();

public class MyRequirement: IAuthorizationRequirement { }

public class MyRequirementHandler : AuthorizationHandler<MyRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, MyRequirement requirement)
    {
        context.Succeed(new MyRequirement());
        return Task.CompletedTask;
    }
}
