#region

using BuildingBlocks.Source.Infrastructure.Config;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Sac.Domain.Models;
using Sac.Infrastructure.Persistance;

#endregion

namespace Sac.Infrastructure.Bootstrap;

/// <summary>
/// Tarea de arranque que se ejecuta una vez al levantar la API:
///
/// <list type="number">
///   <item>
///     Si hay migraciones EF Core pendientes, las aplica (<c>MigrateAsync</c>).
///     Si NO hay ninguna migración registrada en el proyecto pero la BD aún no tiene
///     el esquema, lo crea con <c>EnsureCreatedAsync</c>. Esto permite arrancar el
///     proyecto en una BD vacía sin necesidad de generar manualmente la migración.
///   </item>
///   <item>
///     Si NO existe ningún Director en la base de datos y las variables
///     <c>BOOTSTRAP_DIRECTOR_*</c> están seteadas, crea el primer Director.
///   </item>
/// </list>
///
/// Es idempotente: en arranques posteriores no hace nada si la BD ya está al día y
/// hay al menos un Director.
/// </summary>
public static class DatabaseBootstrapper
{
    public static async Task RunAsync(IServiceProvider services, CancellationToken cancellationToken = default)
    {
        await using var scope = services.CreateAsyncScope();
        var sp = scope.ServiceProvider;
        var logger = sp.GetRequiredService<ILogger<ApplicationDbContext>>();
        var db = sp.GetRequiredService<ApplicationDbContext>();

        // 1) Esquema: migraciones o EnsureCreated
        try
        {
            var pending = (await db.Database.GetPendingMigrationsAsync(cancellationToken)).ToList();

            if (pending.Count > 0)
            {
                logger.LogInformation(
                    "[ BOOTSTRAP ] Aplicando {Count} migración(es) pendiente(s): {Names}",
                    pending.Count, string.Join(", ", pending));
                await db.Database.MigrateAsync(cancellationToken);
            }
            else
            {
                // No hay migraciones registradas: si la BD está vacía, crearla a partir del modelo.
                // EnsureCreatedAsync no hace nada si las tablas ya existen.
                var allMigrations = db.Database.GetMigrations().ToList();
                if (allMigrations.Count == 0)
                {
                    var created = await db.Database.EnsureCreatedAsync(cancellationToken);
                    if (created)
                        logger.LogInformation(
                            "[ BOOTSTRAP ] Esquema creado vía EnsureCreated (no hay migraciones registradas).");
                    else
                        logger.LogInformation("[ BOOTSTRAP ] Esquema ya existente; no se requiere creación.");
                }
                else
                {
                    logger.LogInformation("[ BOOTSTRAP ] Base de datos al día (sin migraciones pendientes).");
                }
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "[ BOOTSTRAP ] Error preparando el esquema de la base de datos.");
            throw;
        }

        // 2) Crear primer Director si no existe ninguno
        try
        {
            await SeedPrimerDirectorAsync(db, logger, cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "[ BOOTSTRAP ] Error creando el primer Director.");
            throw;
        }
    }

    private static async Task SeedPrimerDirectorAsync(
        ApplicationDbContext db,
        ILogger logger,
        CancellationToken ct)
    {
        if (await db.Directores.AnyAsync(ct))
        {
            logger.LogInformation("[ BOOTSTRAP ] Ya existe al menos un Director registrado. No se crea ninguno.");
            return;
        }

        var nombre = EnvironmentHelper.GetEnvOrDefault("BOOTSTRAP_DIRECTOR_NOMBRE", string.Empty);
        var cedula = EnvironmentHelper.GetEnvOrDefault("BOOTSTRAP_DIRECTOR_CEDULA", string.Empty);
        var correo = EnvironmentHelper.GetEnvOrDefault("BOOTSTRAP_DIRECTOR_CORREO", string.Empty);
        var password = EnvironmentHelper.GetEnvOrDefault("BOOTSTRAP_DIRECTOR_PASSWORD", string.Empty);

        if (string.IsNullOrWhiteSpace(nombre) ||
            string.IsNullOrWhiteSpace(cedula) ||
            string.IsNullOrWhiteSpace(correo) ||
            string.IsNullOrWhiteSpace(password))
        {
            logger.LogWarning(
                "[ BOOTSTRAP ] No existe Director en la base de datos y las variables " +
                "BOOTSTRAP_DIRECTOR_NOMBRE/CEDULA/CORREO/PASSWORD no están todas configuradas. " +
                "No se podrá iniciar sesión hasta crear un Director manualmente.");
            return;
        }

        try
        {
            var director = Director.Create(nombre, cedula, correo, password);
            _ = await db.Directores.AddAsync(director, ct);
            _ = await db.SaveChangesAsync(ct);

            logger.LogInformation(
                "[ BOOTSTRAP ] Primer Director creado: {Nombre} (cédula {Cedula}, correo {Correo}). " +
                "Use estas credenciales para iniciar sesión.",
                director.NombreCompleto, director.Cedula, director.Correo);
        }
        catch (Exception ex)
        {
            logger.LogError(ex,
                "[ BOOTSTRAP ] Error al crear el primer Director con las variables de entorno proporcionadas. " +
                "Verifique que la cédula sea numérica, el correo válido y la contraseña tenga al menos 6 caracteres.");
            throw;
        }
    }
}
