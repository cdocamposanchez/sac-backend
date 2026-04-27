#region

using BuildingBlocks.Source.Application.CQRS;
using Sac.Domain.Dtos;

#endregion

namespace Sac.Application.Modules.Docentes.Queries.GetDocenteById;

public record GetDocenteByIdQuery(long Id) : IQuery<DocenteDto>;
