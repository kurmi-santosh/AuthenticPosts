# AuthenticPosts
Posts service with .NET Core 7 having EF Core and JWT Authentication

### Adding Swagger to project

- Add Nuget package Swashbuckle.AspNetCore
- Need to add SwaggerGen service

```c#

    builder.Services.AddSwaggerGen(x =>
    {
        x.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "Posts API", Version = "v1" });
    });

```

- Configure app to use Swagger

```c#
    // Create object and bind the values from the configuration
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

```

