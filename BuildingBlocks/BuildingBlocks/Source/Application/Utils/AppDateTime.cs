#region

using System.Runtime.InteropServices;

#endregion

namespace BuildingBlocks.Source.Application.Utils;

/// <summary>
/// Helper centralizado de fechas. Todas las marcas de tiempo del sistema
/// se obtienen aquí para mantener consistencia.
///
/// <para>
/// <b>Persistencia (PostgreSQL <c>timestamptz</c>):</b> use <see cref="Now"/>.
/// Devuelve UTC con <c>Kind=Utc</c> — es lo que Npgsql exige para columnas
/// <c>timestamp with time zone</c>.
/// </para>
///
/// <para>
/// <b>Presentación / logs / reportes:</b> use <see cref="BogotaNow"/> para
/// mostrar la hora local de Cali/Bogotá (UTC-5).
/// </para>
/// </summary>
public static class AppDateTime
{
    private static readonly TimeZoneInfo bogotaTimeZone =
        TimeZoneInfo.FindSystemTimeZoneById(
            RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
                ? "SA Pacific Standard Time"
                : "America/Bogota"
        );

    /// <summary>
    /// Instante actual en UTC. Es el valor seguro para guardar en columnas
    /// <c>timestamptz</c> de PostgreSQL (<c>Kind=Utc</c>).
    /// </summary>
    public static DateTime Now => DateTime.UtcNow;

    /// <summary>
    /// Hora actual en la zona horaria de Bogotá/Cali (Colombia, UTC-5).
    /// Pensado solo para mostrar a usuarios o para logs.
    /// </summary>
    public static DateTime BogotaNow =>
        TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, bogotaTimeZone);

    /// <summary>
    /// Convierte un <see cref="DateTime"/> arbitrario a la zona de Bogotá.
    /// Útil para presentar fechas leídas desde la BD (que vienen en UTC).
    /// </summary>
    public static DateTime ToBogota(DateTime dt)
    {
        if (dt.Kind == DateTimeKind.Unspecified)
            dt = DateTime.SpecifyKind(dt, DateTimeKind.Utc);

        return TimeZoneInfo.ConvertTime(dt, bogotaTimeZone);
    }
}
