using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Text;
using WMS.Application.Services;
using WMS.Domain.Interfaces;
using WMS.Infrastructure.Persistence;
using WMS.Infrastructure.Persistence.Repositories;
using WMS.Infrastructure.Security;
using WMS.Shared.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Serilog Configuration
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.Console()
    .WriteTo.File("logs/wms-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

try
{
    Log.Information("🚀 WMS Enterprise API starting...");

    // Add services
    builder.Services.AddControllers()
        .AddApplicationPart(typeof(Program).Assembly)
        .AddControllersAsServices()
        .AddJsonOptions(options =>
        {
            // Configure JSON serialization to use camelCase
            options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
            options.JsonSerializerOptions.DictionaryKeyPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
        });
    //builder.Services.AddOpenApi();

    // Database
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
    {
        if (!string.IsNullOrEmpty(connectionString))
        {
            // Use PostgreSQL
            options.UseNpgsql(connectionString, npgsqlOptions =>
            {
                npgsqlOptions.MigrationsHistoryTable("__EFMigrationsHistory", "wms");
            });
        }
        else
        {
            // Fallback to in-memory if no connection string
            options.UseInMemoryDatabase("WmsDatabase");
        }
    });

    // JWT Configuration
    var jwtSettings = builder.Configuration.GetSection("JwtSettings");
    var secretKey = jwtSettings.GetValue<string>("SecretKey") ?? throw new InvalidOperationException("JWT Secret Key not configured");
    var key = Encoding.ASCII.GetBytes(secretKey);

    builder.Services
        .AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.RequireHttpsMetadata = false;
            options.SaveToken = true;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = jwtSettings.GetValue<string>("Issuer"),
                ValidateAudience = true,
                ValidAudience = jwtSettings.GetValue<string>("Audience"),
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };
        });

    builder.Services.AddAuthorization();

    // CORS
    var corsOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? new[] { "http://localhost:3000", "http://localhost:5173" };
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowFrontend", policy =>
        {
            policy.WithOrigins(corsOrigins)
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials();
        });
    });

    // Services
    // Repositories
    builder.Services.AddScoped<IUserRepository, UserRepository>();
    builder.Services.AddScoped<IRoleRepository, RoleRepository>();
    builder.Services.AddScoped<IPermissionRepository, PermissionRepository>();
    builder.Services.AddScoped<ITenantRepository, TenantRepository>();
    builder.Services.AddScoped<IAuditLogRepository, AuditLogRepository>();
    builder.Services.AddScoped<ICompanyRepository, CompanyRepository>();
    builder.Services.AddScoped<IWarehouseRepository, WarehouseRepository>();
    
    // Unit of Work
    builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
    
    // Security & Authentication
    builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
    builder.Services.AddScoped<ITokenService, TokenService>();
    builder.Services.AddScoped<IPasswordService, PasswordService>();
    builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
    
    // Application Services
    builder.Services.AddScoped<IUserService, UserService>();
    builder.Services.AddScoped<IAuditService, AuditService>();
    builder.Services.AddScoped<ICompanyService, CompanyService>();
    builder.Services.AddScoped<IWarehouseService, WarehouseService>();

    // AutoMapper
    builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

    // MediatR (for use cases)
    builder.Services.AddMediatR(config => config.RegisterServicesFromAssemblies(AppDomain.CurrentDomain.GetAssemblies()));

    var app = builder.Build();

    // Migrations and Seeding
    using (var scope = app.Services.CreateScope())
    {
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordService>();
        var loggerFactory = scope.ServiceProvider.GetRequiredService<ILoggerFactory>();
        var seederLogger = loggerFactory.CreateLogger<DatabaseSeeder>();
        
        // Only apply migrations if using a relational database
        if (db.Database.IsRelational())
        {
            db.Database.Migrate();
            Log.Information("✅ Database migrations applied successfully");
        }
        else
        {
            // For InMemory or other non-relational providers, ensure database exists
            db.Database.EnsureCreated();
            Log.Information("✅ InMemory database created successfully");
        }

        // Seed data
        try
        {
            var seeder = new DatabaseSeeder(db, passwordHasher, seederLogger);
            await seeder.SeedAsync();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error seeding database");
        }
    }

    // Configure the HTTP request pipeline
    if (app.Environment.IsDevelopment())
    {
        //app.MapOpenApi();
        //app.UseSwaggerUI(options => options.SwaggerEndpoint("/openapi/v1.json", "WMS API"));
    }

    app.UseSerilogRequestLogging();
    
    // Desabilitar HTTPS redirect temporariamente para debug
    // app.UseHttpsRedirection();
    
    app.UseCors("AllowFrontend");
    app.UseAuthentication();
    app.UseAuthorization();

    // Mapear endpoints PRIMEIRO
    app.MapGet("/health", () => Results.Ok(new { status = "OK", timestamp = DateTime.UtcNow }))
        .AllowAnonymous();

    app.MapGet("/debug/controllers", () =>
    {
        var assembly = typeof(Program).Assembly;
        var controllers = assembly.GetTypes()
            .Where(t => typeof(ControllerBase).IsAssignableFrom(t) && !t.IsAbstract)
            .Select(t => t.Name)
            .OrderBy(n => n)
            .ToList();

        return Results.Ok(new
        {
            total = controllers.Count,
            controllers = controllers
        });
    }).AllowAnonymous();
    
    app.MapGet("/debug/test", () =>
    {
        return Results.Ok(new { message = "Test works" });
    }).AllowAnonymous();

    app.MapControllers();

    Log.Information("🎧 WMS API ready to accept requests");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "❌ Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
