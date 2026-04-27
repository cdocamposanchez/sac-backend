#region

using BuildingBlocks.Source.Application.CQRS;
using Sac.Domain.Dtos;

#endregion

namespace Sac.Application.Modules.Cursos.Queries.GetCursoById;

public record GetCursoByIdQuery(long Id) : IQuery<CursoDto>;
