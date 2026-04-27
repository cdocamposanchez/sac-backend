#region

using BuildingBlocks.Source.Application.CQRS;
using Sac.Domain.Dtos;

#endregion

namespace Sac.Application.Modules.Materias.Queries.GetMaterias;

public record GetMateriasQuery(bool SoloActivas = true) : IQuery<List<MateriaDto>>;
