
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProductCatalog.Application.Common.Interfaces;
using ProductCatalog.Application.Interfaces.Repositories;
using ProductCatalog.Application.Interfaces.Repositories.Categories;
using ProductCatalog.Application.Interfaces.Repositories.Products;
using ProductCatalog.Application.Interfaces.Repositories.Users;
using ProductCatalog.Domain.Entities;
using ProductCatalog.Infrastructure.Persistence;
using ProductCatalog.Infrastructure.Persistence.Repositories.Categories;
using ProductCatalog.Infrastructure.Persistence.Repositories.Products;
using ProductCatalog.Infrastructure.Persistence.Repositories.Users;
using ProductCatalog.Infrastructure.Services;

namespace ProductCatalog.Infrastructure;

public static class InfrastructureServiceExtensions
{ 
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));
        
        services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
        services.AddScoped<IJwtService, JwtService>();
        services.AddHttpClient<ICurrencyApiService, CurrencyApiService>(client =>
        {
            client.BaseAddress = new Uri("https://api.nbrb.by/exrates/");
        });
        
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<ICategoryReadRepository, CategoryReadRepository>();
        
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IProductReadRepository, ProductReadRepository>();
        
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IUserReadRepository, UserReadRepository>();

        services.AddScoped<IUserContext, UserContext>();

        return services;
    }
}