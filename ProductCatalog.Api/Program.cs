using FluentValidation;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using ProductCatalog.Api.Configuration;
using ProductCatalog.Api.Dtos.Categories;
using ProductCatalog.Api.Dtos.Products;
using ProductCatalog.Api.Dtos.Users;
using ProductCatalog.Api.Extensions;
using ProductCatalog.Api.Middleware;
using ProductCatalog.Application;
using ProductCatalog.Infrastructure;
using ProductCatalog.Infrastructure.Persistence;
using ProductCatalog.Infrastructure.Services;
using Scalar.AspNetCore;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Extensions;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    Log.Information("Starting application build...");

    var builder = WebApplication.CreateBuilder(args);
    
    builder.Host.UseSerilog((context, services, configuration) => configuration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
    );
    
    builder.Services.AddProblemDetails();
    builder.Services.AddHttpContextAccessor();
    builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
    
    builder.Services.Configure<JwtOptions>(
        builder.Configuration.GetSection("JwtOptions"));
    builder.Services.Configure<CookieSettings>(
        builder.Configuration.GetSection("CookieSettings"));
    
    builder.Services.AddApplicationServices();
    builder.Services.AddInfrastructureServices(builder.Configuration);
    
    builder.Services.AddApiAuthentication(builder.Configuration);

    builder.Services.AddScoped<IValidator<RegisterUserRequest>,
        RegisterUserRequestValidator>();
    builder.Services.AddScoped<IValidator<LoginUserRequest>,
        LoginUserRequestValidator>();

    builder.Services.AddScoped<IValidator<CategoryRequest>,
        CategoryRequestValidator>();

    builder.Services.AddScoped<IValidator<ProductRequest>,
        ProductRequestValidator>();

    builder.Services.AddScoped<IValidator<CreateUserRequest>,
        CreateUserRequestValidator>();
    builder.Services.AddScoped<IValidator<ChangePasswordRequest>,
        ChangePasswordRequestValidator>();

    builder.Services.AddFluentValidationAutoValidation();
    
    builder.Services.AddControllers();
    builder.Services.AddOpenApi(options =>
    {
        options.AddDocumentTransformer((document, _, _) =>
        {
            document.Servers = [new OpenApiServer { Url = "/" }];
            return Task.CompletedTask;
        });
    });
    
    builder.Services.AddCors(options =>
    {
        options.AddDefaultPolicy(policy =>
        {
            policy.WithOrigins("http://localhost:3000");
            policy.AllowAnyHeader();
            policy.AllowAnyMethod();
            policy.AllowCredentials();
        });
    });

    var app = builder.Build();
    
    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        try
        {
            var context = services.GetRequiredService<AppDbContext>(); 
            
            context.Database.Migrate(); 
        }
        catch (Exception ex)
        {
            var logger = services.GetRequiredService<ILogger<Program>>();
            logger.LogError(ex, "An error occurred while migrating the database.");
        }
    }
    
    if (app.Environment.IsDevelopment() || app.Environment.IsStaging())
    {
        app.MapOpenApi();
        app.MapScalarApiReference("/api/scalar");
    }
    
    app.UseExceptionHandler();
    
    app.UseSerilogRequestLogging(); 
    
    app.UseCors();

    var cookieSettings = builder.Configuration
        .GetSection("CookieSettings")
        .Get<CookieSettings>();
    
    app.UseCookiePolicy(new CookiePolicyOptions
    {
        MinimumSameSitePolicy = cookieSettings?.GetSameSiteMode() ?? SameSiteMode.Strict,
        HttpOnly = HttpOnlyPolicy.Always,
        Secure = cookieSettings?.Secure == true 
            ? CookieSecurePolicy.Always
            : CookieSecurePolicy.SameAsRequest
    });

    app.UseAuthentication();
    app.UseAuthorization();
    
    app.MapGet("/api/", () => new
    {
        service = "Каталог продуктов API",
        status = "online",
        version = "1.0",
        message = "Добро пожаловать в API. Полная документация в Scalar:",
        documentation = "http://localhost:3000/api/scalar"
    });
    
    app.MapControllers();

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}