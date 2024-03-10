using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using AuthenticPosts.Data;
using AuthenticPosts.Services;
using AuthenticPosts.Config;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;


var builder = WebApplication.CreateBuilder(args);
{
    // Add Database.

    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

    builder.Services.AddDbContext<DataContext>(options =>
        options.UseSqlServer(connectionString));


    builder.Services.AddDefaultIdentity<IdentityUser>(options =>
    {
        options.SignIn.RequireConfirmedAccount = true;

        options.User.RequireUniqueEmail = false;

        options.Password.RequireDigit = false;
        options.Password.RequiredLength = 4;
        options.Password.RequireLowercase = false;
        options.Password.RequireUppercase = false;
        options.Password.RequireNonAlphanumeric = false;
    })
        .AddEntityFrameworkStores<DataContext>();

    // Add Redis

    var redisCacheSettings = new RedisCacheSettings();
    builder.Configuration.Bind(nameof(RedisCacheSettings), redisCacheSettings);
    builder.Services.AddSingleton(redisCacheSettings);

    if (redisCacheSettings.Enabled)
    {
        builder.Services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = redisCacheSettings.ConnectionString;
        });
    }

    builder.Services.AddScoped<IResponseCacheService, ResponseCacheService>();

    // Add JWT Authentication

    var jwtSettings = new JWTSettings();
    builder.Configuration.Bind(nameof(JWTSettings), jwtSettings);
    builder.Services.AddSingleton(jwtSettings);

    builder.Services.AddAuthentication(config =>
    {
        config.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        config.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        config.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(x =>
    {
        x.SaveToken = true;
        x.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtSettings.Secret)),
            ValidateIssuer = false,
            ValidateAudience = false,
            RequireExpirationTime = false,
            ValidateLifetime = true
        };
    });

    // Add Swagger

    builder.Services.AddSwaggerGen(x =>
    {
        x.SwaggerDoc("v1", new OpenApiInfo { Title = "Posts API", Version = "v1" });

        x.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Description = "JWT Auth header using the bearer scheme",
            Name = "Authorization",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.ApiKey
        });

        x.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme {
                    Reference = new OpenApiReference{
                        Id = "Bearer", //The name of the previously defined security scheme.
                        Type = ReferenceType.SecurityScheme
                    }
                }, new List<string>()
            }
        });
    });

    // Add services

    builder.Services.AddScoped<IIdentityService, IdentityService>();
    builder.Services.AddScoped<IPostService, PostService>();
    
}


var app = builder.Build();
{

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseMigrationsEndPoint();
    }
    else
    {
        app.UseExceptionHandler("/Home/Error");
        // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
        app.UseHsts();
    }

    var swaggerOptions = new SwaggerOptions();
    builder.Configuration.GetSection(nameof(SwaggerOptions)).Bind(swaggerOptions);

    app.UseSwagger(option =>
    {
        option.RouteTemplate = swaggerOptions.JSONRoute;
    });

    app.UseSwaggerUI(option =>
    {
        option.SwaggerEndpoint(swaggerOptions.UIEndpoint, "v1");
    });

    app.UseHttpsRedirection();
    app.UseStaticFiles();

    app.UseRouting();

    app.UseAuthorization();
    app.UseAuthentication();

    app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");
    app.MapRazorPages();

    app.Run();
}

