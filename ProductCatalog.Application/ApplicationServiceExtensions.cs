using MediatR;
using Microsoft.Extensions.DependencyInjection;
using ProductCatalog.Application.Common.Behaviors;

namespace ProductCatalog.Application;

public static class ApplicationServiceExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddMediatR(cfg => 
            cfg.RegisterServicesFromAssembly(typeof(ApplicationServiceExtensions).Assembly));
        
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(BlockUserPipelineBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingPipelineBehavior<,>));

        return services;
    }
}