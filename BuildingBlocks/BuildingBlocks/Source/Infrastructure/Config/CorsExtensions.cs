#region

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

#endregion

namespace BuildingBlocks.Source.Infrastructure.Config;

public static class CorsExtensions
{
    private const string corsPolicyName = "GlobalCorsPolicy";

    public static void AddGlobalCorsPolicy(this IServiceCollection services)
    {
        var allowedOriginsEnv = EnvironmentHelper.GetEnvOrDefault("CORS_ALLOWED_ORIGINS", "*");

        var allowedOrigins = allowedOriginsEnv
            .Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        services.AddCors(options =>
        {
            options.AddPolicy(corsPolicyName, builder =>
            {
                if (allowedOrigins.Contains("*"))
                    builder
                        .AllowAnyOrigin()
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                else
                    builder
                        .WithOrigins(allowedOrigins)
                        .AllowAnyHeader()
                        .AllowAnyMethod();
            });
        });
    }

    public static void UseGlobalCorsPolicy(this IApplicationBuilder app)
    {
        app.UseCors(corsPolicyName);
    }
}
