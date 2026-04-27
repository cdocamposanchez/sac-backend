#region

using BuildingBlocks.Source.Application.CQRS;
using Sac.Domain.Dtos;

#endregion

namespace Sac.Application.Modules.Actividades.Queries.GetActividadById;

public record GetActividadByIdQuery(long Id) : IQuery<ActividadDto>;
