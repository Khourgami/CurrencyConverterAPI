using Asp.Versioning;
using CurrencyConverter.Api.Middleware;
using CurrencyConverter.Application.DependencyInjection;
using CurrencyConverter.Application.Interfaces;
using CurrencyConverter.Application.Services;
using CurrencyConverter.Infrastructure.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;
using Serilog.Events;
using System.Security.Claims;
using System.Text;

public class Program  // <-- must be public
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);


        builder.Configuration
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
            .AddEnvironmentVariables();

        builder.Services.AddHealthChecks();

        // Add framework services
        builder.Services.AddControllers();
        builder.Services.AddSwaggerGen();

        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .WriteTo.Seq("http://localhost:5341") // optional, replace with Seq URL
            .CreateLogger();

        builder.Host.UseSerilog();

        //Add OpenTelemetry tracing
        builder.Services.AddOpenTelemetry()
            .WithTracing(tracerProviderBuilder =>
            {
                tracerProviderBuilder
                    .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("CurrencyConverterApi"))
                    .AddAspNetCoreInstrumentation(options =>
                    {
                        options.RecordException = true;
                    })
                    .AddHttpClientInstrumentation()
                    .AddConsoleExporter();
            });

        // Application layer registrations
        builder.Services.AddApplicationServices();

        //// Infrastructure registrations (single source of truth)
        builder.Services.AddInfrastructure(builder.Configuration);

        //IdentityModelEventSource.ShowPII = true;

        var jwt = builder.Configuration.GetSection("Jwt");
        var secret = jwt["Secret"];
        var key = Encoding.UTF8.GetBytes(secret);
        var keyBytes = Encoding.UTF8.GetBytes(jwt["Secret"] ?? throw new InvalidOperationException("Jwt:Secret missing"));

        // Add authentication & make events verbose
        builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.SaveToken = true;
            options.RequireHttpsMetadata = false; // local dev

            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,

                ValidIssuer = jwt["Issuer"],
                ValidAudience = jwt["Audience"],

                // Use the same secret that TokenService uses
                IssuerSigningKey = new SymmetricSecurityKey(keyBytes),

                // ensure roles and name are read from the standard claim types
                RoleClaimType = ClaimTypes.Role,
                NameClaimType = ClaimTypes.Name,

                // reduce clock skew for debugging
                ClockSkew = TimeSpan.Zero
            };

            // Diagnostic events — logs why tokens are rejected
            options.Events = new JwtBearerEvents
            {
                OnMessageReceived = ctx =>
                {
                    // Called early – useful to inspect header
                    var logger = ctx.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
                    logger.LogDebug("OnMessageReceived - Authorization header: {AuthHeader}", ctx.Request.Headers["Authorization"].ToString());
                    return Task.CompletedTask;
                },

                OnTokenValidated = ctx =>
                {
                    var logger = ctx.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
                    logger.LogInformation("OnTokenValidated: user {Name}", ctx.Principal?.Identity?.Name ?? "<no name>");
                    foreach (var c in ctx.Principal?.Claims ?? Enumerable.Empty<Claim>())
                    {
                        logger.LogInformation("Claim: {Type} = {Value}", c.Type, c.Value);
                    }
                    return Task.CompletedTask;
                },

                OnAuthenticationFailed = ctx =>
                {
                    var logger = ctx.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ctx.Exception, "JWT authentication failed: {Message}", ctx.Exception.Message);
                    // Do NOT swallow exception; middleware will still return 401.
                    return Task.CompletedTask;
                },

                OnChallenge = context =>
                {
                    var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
                    logger.LogWarning("OnChallenge: error={Error}, description={Description}",
                        context.Error, context.ErrorDescription);
                    return Task.CompletedTask;
                }
            };
        });
        // CORS (allow Swagger UI)
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowAll", policy =>
            {
                policy.AllowAnyOrigin()
                      .AllowAnyHeader()
                      .AllowAnyMethod();
            });
        });

        builder.Services.AddApiVersioning(options =>
        {
            options.DefaultApiVersion = new ApiVersion(1, 0);
            options.AssumeDefaultVersionWhenUnspecified = true;
            options.ReportApiVersions = true;
        })
        .AddApiExplorer(options =>
        {
            options.GroupNameFormat = "'v'VVV";
            options.SubstituteApiVersionInUrl = true;
        });

        // Swagger with JWT support
        // Swagger config
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "JWT Demo API", Version = "v1" });

            // 🔑 Enable "Authorize" button
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                In = ParameterLocation.Header,
                Description = "Enter 'Bearer {token}'",
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer"
            });

            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] { }
        }
            });
        });

        builder.Services.AddControllers();

        var app = builder.Build();

        // Middleware order is important
        app.UseMiddleware<ExceptionHandlingMiddleware>();
        app.UseMiddleware<RequestLoggingMiddleware>();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        //app.UseHttpsRedirection();
        app.UseCors("AllowAll");

        // Authentication must come before Authorization
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapHealthChecks("/health");
        app.MapControllers();
        app.Run();
    }
}