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

### Adding Authentication with JWT to project

- Cookie authentication scheme uses cookie data to restore the user identity.
- JWT Bearer authentication scheme will use the token that is provided as part of the Authorization header in the request to create the user identity.

* In the authentication world, there are certain actions that you can perform:
  - Authenticate : Cookie authentication or JWT
  - Challenge : When an authentication scheme is challenged, the scheme should prompt the user to authenticate themselves.
  - Forbid : This is commonly a HTTP 403 error, and may be a redirect to some error page.
  - Sign-In : When an authentication scheme is being signed in, then the scheme is being told to take an existing user (a ClaimsPrincipal) and to persist that in some way.
  - Sign-Out : This is the inverse of sign-in and will basically tell the authentication scheme to remove that persistence.

- Explicitly define what authentication scheme to use as the default for each of those authentication actions:

  - Authenticate: DefaultAuthenticateScheme, or DefaultScheme
  - Challenge: DefaultChallengeScheme, or DefaultScheme
  - Forbid: DefaultForbidScheme, or DefaultChallengeScheme, or DefaultScheme
  - Sign-in: DefaultSignInScheme, or DefaultScheme
  - Sign-out: DefaultSignOutScheme, or DefaultScheme

- As you can see, each of the authentication actions falls back to DefaultScheme if the specific action’s default isn’t configured.

### Setup

- Add Nuget package Microsoft.AspNetCore.Authentication.JwtBearer

```c#

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

   builder.Services.AddSwaggerGen(x =>
   {
       x.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "Posts API", Version = "v1" });

       x.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
       {
           Description = "JWT Auth header using the bearer scheme",
           Name = "Authorization",
           In= ParameterLocation.Header,
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

   app.UseAuthentication();

```

### Implementing Cache

- We need Microsoft.Extensions.Caching.StackExchangeRedis nuget package
- You can install redis locally

```sh
    brew install redis
    redis-cli
```

- In redis cli, you are connected to locahost:6397
- You can check your keys and values

```sh
    keys "*"
    set "key" "value"
    get "key"
```

### Implementing Pagination

- Wrap the response as data and add

### Error Handling

- via ExceptionFilter attribute

  - CONCERNED ONLY ABOUT ERRORS IN MY CODE - Use Filter

  ```c#
    public class ErrorHandlingAttribute : ExceptionFilterAttribute
    {
        public override void OnException(ExceptionContext context)
        {
            var exception = context.Exception;
            // ObjectResult excepts an object - We can pass ProblemDetails instead of creating our own
            var errorDetails = new ObjectResult(problemDetails);

            context.Result = errorDetails;
            context.ExceptionHandled = true;
        }
    }

  ```

- via Middleware

  - CONCERNED ABOUT ERRORS IN THE ASP.NET FRAMEWORK - Use Middleware

  ```c#
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public ErrorHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch(Exception ex)
            {
                var result = JsonConvert.SerializeObject(new { error = ex.Message });

                    context.Response.ContentType = "application/json";
                    context.Response.StatusCode = 500;
                    return context.Response.WriteAsync(result);
            }
        }
    }

  ```

- CAPTURE THE GLOBAL ERRORS IN THE ASP.NET FRAMEWORK AS WELL AS I HAVE UNIQUE LOGIC BASED ON WHERE THE ERROR OCCURRED - Use Both

- Problem Details with error controller

```c#
    app.UseExceptionHandler("/error");
    app.Map("/error", (HttpContext ctx) =>
    {
        var exception = ctx.Features.Get<IExceptionHandlerFeature>()?.Error;
        return Results.Problem(title: ex?.Message, statusCode: 500);
    });

```
