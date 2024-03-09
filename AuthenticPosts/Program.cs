const string AuthScheme = "cookie";

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddAuthentication(AuthScheme)
       .AddCookie(AuthScheme);

builder.Services.AddAuthorization();

builder.Services.AddControllers();

var app = builder.Build();

app.UseHttpsRedirection();

app.MapControllers();

app.Run();