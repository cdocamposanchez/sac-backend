#region

using BuildingBlocks.Source.Application.CQRS;
using Sac.Domain.Dtos;

#endregion

namespace Sac.Application.Modules.Reportes.Queries.GetReporteEstudiante;

/// <summary>RF-10 — Reporte detallado de un estudiante en un curso (cerrado o no).</summary>
public record GetReporteEstudianteQuery(long CursoId, long EstudianteId) : IQuery<ReporteEstudianteDto>;
