#region

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

#endregion

namespace BuildingBlocks.Source.Infrastructure.Config;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSwaggerGenWthAuth(this IServiceCollection services)
    {
        _ = services.AddSwaggerGen(o =>
        {
            o.CustomSchemaIds(id => id.FullName!.Replace("+", "-"));

            o.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Sistema Académico Cayzedo (SAC)",
                Version = "v1",
                Description = "API REST del Sistema Académico Cayzedo (SAC) — Institución Educativa Joaquín de Cayzedo y Cuero."
            });

            var securityScheme = new OpenApiSecurityScheme
            {
                Name = "JWT Authentication",
                Description = "Ingresa el token JWT en este campo",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = JwtBearerDefaults.AuthenticationScheme,
                BearerFormat = "JWT"
            };

            o.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, securityScheme);

            var securityRequirement = new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = JwtBearerDefaults.AuthenticationScheme
                        }
                    },
                    []
                }
            };
            o.AddSecurityRequirement(securityRequirement);
        });

        return services;
    }
}
