#region

using Microsoft.EntityFrameworkCore;
using Sac.Application.Helpers;
using Sac.Application.Persistance;
using Sac.Domain.Dtos;
using Sac.Domain.Models;
using DomainCalificacion = Sac.Domain.Models.Calificacion;

#endregion

namespace Sac.Application.Modules.Calificaciones._Shared;

/// <summary>
/// Helper interno del módulo Calificaciones. Centraliza:
///  - construcción del CalificacionDto (con joins a Estudiante/Actividad).
///  - recálculo del promedio agregado del periodo (RF-09).
///
/// El porcentaje vive en la <see cref="Actividad"/> y se resuelve con un join.
/// </summary>
internal static class CalificacionShared
{
    public static async Task<CalificacionDto> BuildDtoAsync(
        IApplicationDbContext db,
        DomainCalificacion c,
        CancellationToken ct)
    {
        var est = await db.Estudiantes.AsNoTracking().FirstOrDefaultAsync(e => e.Id == c.EstudianteId, ct);
        var act = await db.Actividades.AsNoTracking().FirstOrDefaultAsync(a => a.Id == c.ActividadId, ct);

        return new CalificacionDto(
            c.Id,
            c.EstudianteId, est?.NombreCompleto,
            c.ActividadId, act?.Nombre, act?.Porcentaje ?? 0m,
            c.CursoId, c.PeriodoId, c.Nota);
    }

    /// <summary>
    /// RF-09: recalcula y persiste el promedio agregado del periodo
    /// (promedio de los promedios por estudiante).
    /// </summary>
    public static async Task RecalcularPromedioPeriodoAsync(
        IApplicationDbContext db,
        Periodo periodo,
        CancellationToken ct)
    {
        var calificaciones = await db.Calificaciones
            .Where(c => c.PeriodoId == periodo.Id)
            .ToListAsync(ct);

        if (calificaciones.Count == 0)
        {
            periodo.ActualizarPromedio(0);
            return;
        }

        // Cargar actividades involucradas en una sola consulta para resolver porcentajes
        var actividadIds = calificaciones.Select(c => c.ActividadId).Distinct().ToList();
        var actividadesPorId = await db.Actividades.AsNoTracking()
            .Where(a => actividadIds.Contains(a.Id))
            .ToDictionaryAsync(a => a.Id, ct);

        var promediosPorEstudiante = calificaciones
            .GroupBy(c => c.EstudianteId)
            .Select(g => PromedioCalculator.PromedioPeriodo(g, actividadesPorId) ?? 0m)
            .ToList();

        if (promediosPorEstudiante.Count > 0)
            periodo.ActualizarPromedio(promediosPorEstudiante.Average());
    }
}
