#region

using BuildingBlocks.Source.Application.CQRS;
using Sac.Domain.Dtos;

#endregion

namespace Sac.Application.Modules.Periodos.Queries.GetPeriodoById;

public record GetPeriodoByIdQuery(long Id) : IQuery<PeriodoDto>;
