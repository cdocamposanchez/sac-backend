#region

using DotNetEnv;

#endregion

namespace BuildingBlocks.Source.Infrastructure.Config;

public static class EnvironmentHelper
{
    public static string GetRequiredEnv(string key)
    {
        var value = Environment.GetEnvironmentVariable(key);

        return string.IsNullOrWhiteSpace(value)
            ? throw new InvalidOperationException(
                $"No se encontró la variable de entorno '{key}'. Configúrala en tu .env o variables del sistema.")
            : value;
    }

    public static string GetEnvOrDefault(string key, string defaultValue)
    {
        var value = Environment.GetEnvironmentVariable(key);
        return string.IsNullOrWhiteSpace(value) ? defaultValue : value;
    }

    public static void LoadEnv()
    {
        var paths = new[]
        {
            Path.Combine(Directory.GetCurrentDirectory(), ".env"),
            Path.Combine(Directory.GetCurrentDirectory(), "..", ".env"),
            Path.Combine(Directory.GetCurrentDirectory(), "..", "..", ".env"),
            Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..", ".env")
        };

        foreach (var path in paths)
        {
            if (!File.Exists(path)) continue;

            _ = Env.Load(path);
            return;
        }
    }
}
