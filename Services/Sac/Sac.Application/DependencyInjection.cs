#region

using System.Reflection;
using BuildingBlocks.Source.Application.Behaviors;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

#endregion

namespace Sac.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();

        _ = services.AddMediatR(config =>
        {
            _ = config.RegisterServicesFromAssembly(assembly);
            _ = config.AddOpenBehavior(typeof(ValidationBehavior<,>));
            _ = config.AddOpenBehavior(typeof(LoggingBehavior<,>));
        });

        _ = services.AddValidatorsFromAssembly(assembly);

        return services;
    }
}
