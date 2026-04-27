#region

using BuildingBlocks.Source.Application.Behaviors;
using BuildingBlocks.Source.Domain.Exception.Handler;
using BuildingBlocks.Source.Infrastructure.Config;
using BuildingBlocks.Source.Infrastructure.Middleware;
using FluentValidation;
using HealthChecks.UI.Client;
using MediatR;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Rewrite;

#endregion

namespace Sac.Api;

public static class DependencyInjection
{
    public static void AddApiServices(this IServiceCollection services, IConfiguration configuration)
    {
        var assembly = typeof(Program).Assembly;

        // CORS y controllers
        services.AddGlobalCorsPolicy();
        _ = services.Configure<ApiBehaviorOptions>(InvalidModelStateResponseConfigurator.Configure);
        _ = services.AddHttpContextAccessor();

        _ = services.AddControllers(options =>
        {
            options.Conventions.Insert(0, new RoutePrefixConvention("api/v1"));
        });

        // MediatR + behaviors
        _ = services.AddMediatR(config =>
        {
            _ = config.RegisterServicesFromAssembly(assembly);
            _ = config.AddOpenBehavior(typeof(ValidationBehavior<,>));
            _ = config.AddOpenBehavior(typeof(LoggingBehavior<,>));
        });

        // FluentValidation
        _ = services.AddValidatorsFromAssembly(assembly);

        // Excepciones
        _ = services.AddExceptionHandler<CustomExceptionHandler>();

        // Swagger
        _ = services.AddEndpointsApiExplorer();
        _ = services.AddSwaggerGenWthAuth();

        // Health
        _ = services.AddHealthChecks()
            .AddNpgSql(configuration["ConnectionStrings:Database"]!);
    }

    public static void UseApiServices(this WebApplication app)
    {
        _ = app.UseMiddleware<ApiResponseMiddleware>();

        var rewriteOptions = new RewriteOptions()
            // Redirige la raíz "/" a Swagger UI (302).
            .AddRedirect("^$", "api/v1/docs/", StatusCodes.Status302Found)
            .AddRewrite("^api/v1/docs/?$", "api/v1/docs/index.html", true);
        _ = app.UseRewriter(rewriteOptions);

        if (app.Environment.IsDevelopment() || Environment.GetEnvironmentVariable("ENABLE_SWAGGER") == "true")
        {
            _ = app.UseSwagger(c => { c.RouteTemplate = "api/v1/swagger/{documentName}/swagger.json"; });

            _ = app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/api/v1/swagger/v1/swagger.json", "SAC API v1");
                c.RoutePrefix = "api/v1/docs";
            });
        }

        // Headers de seguridad
        app.Use(async (context, next) =>
        {
            context.Response.Headers.Append("X-Content-Type-Options", "nosniff");
            context.Response.Headers.Append("X-Frame-Options", "DENY");
            context.Response.Headers.Append("Referrer-Policy", "no-referrer");
            await next();
        });

        app.UseGlobalCorsPolicy();

        _ = app.UseAuthentication();
        _ = app.UseAuthorization();

        _ = app.MapControllers();

        _ = app.UseExceptionHandler(_ => { });

        _ = app.UseHealthChecks("/api/v1/health",
            new HealthCheckOptions
            {
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });
    }
}
