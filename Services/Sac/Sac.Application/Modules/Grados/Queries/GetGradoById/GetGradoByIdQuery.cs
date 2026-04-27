#region

using BuildingBlocks.Source.Application.CQRS;
using Sac.Domain.Dtos;

#endregion

namespace Sac.Application.Modules.Grados.Queries.GetGradoById;

public record GetGradoByIdQuery(long Id) : IQuery<GradoDto>;
