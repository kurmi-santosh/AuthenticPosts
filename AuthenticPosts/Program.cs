using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

const string AuthScheme = "cookie";

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddAuthentication(AuthScheme)
       .AddCookie(AuthScheme);

// Add Microsoft.EntityFrameworkCore.InMemory
builder.Services.AddDbContext<IdentityDbContext>(builder => builder.UseInMemoryDatabase("my_db"));

builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    options.User.RequireUniqueEmail = false;

    options.Password.RequireDigit = false;
    options.Password.RequiredLength = 4;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
}).AddEntityFrameworkStores<IdentityDbContext>()
  .AddDefaultTokenProviders();

builder.Services.AddAuthorization();

builder.Services.AddControllers();
var app = builder.Build();

// Let's create an admin role and add user to admin roles list
using (var scope = app.Services.CreateScope()) {

    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    await roleManager.CreateAsync(new IdentityRole() { Name = "admin" });

    var usermanager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
   

    var user = new IdentityUser() { UserName = "test@gmail.com", Email = "test@gmail.com" };
    await usermanager.CreateAsync(user, password: "test");

    await usermanager.AddToRoleAsync(user, "admin");
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();