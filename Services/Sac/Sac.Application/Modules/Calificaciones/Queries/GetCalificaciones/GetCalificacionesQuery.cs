#region

using BuildingBlocks.Source.Application.CQRS;
using Sac.Domain.Dtos;

#endregion

namespace Sac.Application.Modules.Calificaciones.Queries.GetCalificaciones;

public record GetCalificacionesQuery(
    long? CursoId = null,
    long? PeriodoId = null,
    long? ActividadId = null,
    long? EstudianteId = null) : IQuery<List<CalificacionDto>>;
