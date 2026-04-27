#region

using Microsoft.Extensions.Configuration;

#endregion

namespace BuildingBlocks.Source.Infrastructure.Config;

public static class ConnectionStringProvider
{
    public static string GetPgConnectionString(IConfiguration configuration, string schema)
    {
        var dbUser = EnvironmentHelper.GetRequiredEnv("DB_USER");
        var dbPassword = EnvironmentHelper.GetRequiredEnv("DB_PASSWORD");
        var dbHost = EnvironmentHelper.GetRequiredEnv("DB_HOST");
        var dbName = EnvironmentHelper.GetRequiredEnv("DB_NAME");
        var dbPort = EnvironmentHelper.GetEnvOrDefault("DB_PORT", "5432");

        var connectionString =
            $"Host={dbHost};Port={dbPort};Database={dbName};Username={dbUser};Password={dbPassword};Search Path={schema}";

        configuration["ConnectionStrings:Database"] = connectionString;
        return connectionString;
    }
}
