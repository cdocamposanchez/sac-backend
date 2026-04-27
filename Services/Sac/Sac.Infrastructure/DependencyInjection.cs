#region

using BuildingBlocks.Source.Infrastructure.Config;
using BuildingBlocks.Source.Infrastructure.Email;
using BuildingBlocks.Source.Infrastructure.Persistance.Interceptors;
using BuildingBlocks.Source.Infrastructure.Security;
using Sac.Application.Persistance;
using Sac.Infrastructure.Persistance;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

#endregion

namespace Sac.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services, IConfiguration configuration)
    {
        var schema = Environment.GetEnvironmentVariable("CALIFICACIONES_DB_SCHEMA") ?? "public";

        // ============= PostgreSQL (EF Core) =============
        var connectionString = ConnectionStringProvider.GetPgConnectionString(configuration, schema);
        configuration["ConnectionStrings:Database"] = connectionString;

        _ = services.AddScoped<ISaveChangesInterceptor, AuditableEntityInterceptor>();
        _ = services.AddScoped<ISaveChangesInterceptor, DispatchDomainEventsInterceptor>();

        _ = services.AddDbContext<ApplicationDbContext>((sp, options) =>
        {
            _ = options.AddInterceptors(sp.GetServices<ISaveChangesInterceptor>());

            _ = Environment.GetEnvironmentVariable("ENABLE_SENSITIVE_DATA_LOGGING") == "true"
                ? options.UseNpgsql(connectionString).EnableSensitiveDataLogging()
                : options.UseNpgsql(connectionString);
        });

        _ = services.AddScoped<IApplicationDbContext, ApplicationDbContext>();

        // ============= JWT (autenticación propia, sustituye Keycloak) =============
        // Las opciones JWT se cargan desde la configuración mediante variables de entorno.
        configuration["Jwt:Issuer"] = Environment.GetEnvironmentVariable("JWT_ISSUER") ?? "sac-api";
        configuration["Jwt:Audience"] = Environment.GetEnvironmentVariable("JWT_AUDIENCE") ?? "sac-clients";
        configuration["Jwt:SecretKey"] = EnvironmentHelper.GetRequiredEnv("JWT_SECRET_KEY");
        configuration["Jwt:ExpirationMinutes"] =
            Environment.GetEnvironmentVariable("JWT_EXPIRATION_MINUTES") ?? "60";

        _ = services.AddJwtAuthentication(configuration);
        _ = services.AddHttpContextAccessor();
        _ = services.AddScoped<ICurrentUserService, CurrentUserService>();

        // ============= Email (SMTP Google) =============
        configuration["Email:SmtpHost"] = EnvironmentHelper.GetEnvOrDefault("SMTP_HOST", "smtp.gmail.com");
        configuration["Email:SmtpPort"] = EnvironmentHelper.GetEnvOrDefault("SMTP_PORT", "587");
        configuration["Email:UseStartTls"] = EnvironmentHelper.GetEnvOrDefault("SMTP_USE_STARTTLS", "true");
        configuration["Email:FromAddress"] = EnvironmentHelper.GetRequiredEnv("SMTP_FROM_ADDRESS");
        configuration["Email:FromName"] =
            EnvironmentHelper.GetEnvOrDefault("SMTP_FROM_NAME", "Sistema Académico Cayzedo (SAC)");
        configuration["Email:Username"] = EnvironmentHelper.GetRequiredEnv("SMTP_USERNAME");
        configuration["Email:Password"] = EnvironmentHelper.GetRequiredEnv("SMTP_PASSWORD");

        _ = services.Configure<EmailOptions>(configuration.GetSection(EmailOptions.SectionName));
        _ = services.AddSingleton<IEmailService, SmtpEmailService>();

        return services;
    }
}
