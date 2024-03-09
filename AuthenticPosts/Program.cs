var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

app.MapGet("/username", (HttpContext ctx) =>
{
    var authCookie = ctx.Request.Headers.Cookie.FirstOrDefault(x => x!=null && x.StartsWith("auth="));
    var payload = authCookie?.Split("=").Last();
    if (payload!=null)
    {
        var parts = payload.Split(":");
        var key = parts[0];
        var value = parts[1];
        return value;
    }
    return null;
});

app.MapGet("/login", (HttpContext ctx) =>
{
    ctx.Response.Headers["set-cookie"] = "auth=usr:kurmi";
    return "ok";
});

app.Run();

