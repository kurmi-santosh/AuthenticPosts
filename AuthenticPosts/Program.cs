using Microsoft.AspNetCore.DataProtection;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDataProtection();


var app = builder.Build();

app.MapGet("/username", (HttpContext ctx, IDataProtectionProvider idp) =>
{
    var protector = idp.CreateProtector("auth-cookie");

    var authCookie = ctx.Request.Headers.Cookie.FirstOrDefault(x => x!=null && x.StartsWith("auth="));
    var protectedPayload = authCookie?.Split("=").Last();
   
    if (protectedPayload != null)
    {
        var payload = protector.Unprotect(protectedPayload);

        var parts = payload.Split(":");
        var key = parts[0];
        var value = parts[1];
        return value;
    }
    return null;
});

app.MapGet("/login", (HttpContext ctx, IDataProtectionProvider idp) =>
{
    var protector = idp.CreateProtector("auth-cookie");
    ctx.Response.Headers["set-cookie"] = $"auth={protector.Protect("usr:kurmi")}";
    return "ok";
});

app.Run();

