#region

using BuildingBlocks.Source.Application.CQRS;
using Sac.Domain.Dtos;

#endregion

namespace Sac.Application.Modules.Materias.Queries.GetMateriaById;

public record GetMateriaByIdQuery(long Id) : IQuery<MateriaDto>;
