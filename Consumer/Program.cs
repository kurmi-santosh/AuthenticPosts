var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddHttpClient();

var app = builder.Build();

app.UseRouting();
app.UseEndpoints(endPoints =>
{
    endPoints.MapDefaultControllerRoute();
});

app.Run();

