#region

using BuildingBlocks.Source.Application.CQRS;
using Sac.Domain.Dtos;

#endregion

namespace Sac.Application.Modules.Calificaciones.Queries.GetCalificacionById;

public record GetCalificacionByIdQuery(long Id) : IQuery<CalificacionDto>;
