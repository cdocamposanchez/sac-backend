#region

using BuildingBlocks.Source.Application.CQRS;
using Sac.Domain.Dtos;

#endregion

namespace Sac.Application.Modules.Reportes.Queries.GetReporteFinalCurso;

/// <summary>
/// RF-10 — Reporte FINAL del curso, exportable.
/// Solo está disponible cuando el curso está CERRADO.
/// </summary>
public record GetReporteFinalCursoQuery(long CursoId) : IQuery<List<ReporteEstudianteDto>>;
