#region

using Microsoft.EntityFrameworkCore;
using Sac.Application.Helpers;
using Sac.Application.Persistance;
using Sac.Domain.Dtos;
using Sac.Domain.Models;

#endregion

namespace Sac.Application.Modules.Reportes._Shared;

/// <summary>
/// Helper interno del módulo Reportes. Construye la estructura jerárquica
/// del boletín de un estudiante: periodos → calificaciones → promedios.
///
/// El porcentaje de cada nota se resuelve desde la <see cref="Actividad"/>
/// asociada (ya no vive en la calificación).
/// </summary>
internal static class ReporteBuilder
{
    public static async Task<ReporteEstudianteDto> ConstruirReporteEstudianteAsync(
        IApplicationDbContext db,
        Curso curso,
        Estudiante estudiante,
        CancellationToken ct)
    {
        var periodos = await db.Periodos.AsNoTracking()
            .Where(p => p.CursoId == curso.Id)
            .OrderBy(p => p.NumeroCorte)
            .ToListAsync(ct);

        var calificaciones = await db.Calificaciones.AsNoTracking()
            .Where(c => c.CursoId == curso.Id && c.EstudianteId == estudiante.Id)
            .ToListAsync(ct);

        var actividadIds = calificaciones.Select(c => c.ActividadId).Distinct().ToList();
        var actividades = await db.Actividades.AsNoTracking()
            .Where(a => actividadIds.Contains(a.Id))
            .ToDictionaryAsync(a => a.Id, ct);

        var promediosPorPeriodo = new List<decimal?>();
        var reportePeriodos = new List<ReportePeriodoDto>();

        foreach (var p in periodos)
        {
            var calsPeriodo = calificaciones.Where(c => c.PeriodoId == p.Id).ToList();
            var promedio = PromedioCalculator.PromedioPeriodo(calsPeriodo, actividades);
            promediosPorPeriodo.Add(promedio);

            var detalle = calsPeriodo.Select(c =>
            {
                actividades.TryGetValue(c.ActividadId, out var act);
                return new ReporteCalificacionDto(
                    act?.Nombre ?? "N/A",
                    c.Nota,
                    act?.Porcentaje ?? 0m);
            }).ToList();

            reportePeriodos.Add(new ReportePeriodoDto(
                (int)p.NumeroCorte, p.Estado.ToString(), promedio, detalle));
        }

        // RF-09: el promedio final solo se calcula cuando los 3 periodos están cerrados
        decimal? promedioFinal = null;
        if (reportePeriodos.Count == 3
            && reportePeriodos.All(rp => rp.Estado == "Cerrado")
            && promediosPorPeriodo.All(x => x.HasValue))
        {
            promedioFinal = PromedioCalculator.PromedioFinalCurso(
                promediosPorPeriodo[0], promediosPorPeriodo[1], promediosPorPeriodo[2]);
        }

        return new ReporteEstudianteDto(
            estudiante.Id, estudiante.NombreCompleto, estudiante.Cedula, estudiante.Correo,
            curso.Id, curso.Nombre, reportePeriodos, promedioFinal);
    }
}
