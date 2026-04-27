#region

using BuildingBlocks.Source.Application.CQRS;
using Sac.Domain.Dtos;

#endregion

namespace Sac.Application.Modules.Reportes.Queries.GetReporteCurso;

/// <summary>RF-10 — Reporte de todos los estudiantes de un curso (en cualquier momento).</summary>
public record GetReporteCursoQuery(long CursoId) : IQuery<List<ReporteEstudianteDto>>;
