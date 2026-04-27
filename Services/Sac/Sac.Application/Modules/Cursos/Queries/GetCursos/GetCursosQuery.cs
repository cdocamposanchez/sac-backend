#region

using BuildingBlocks.Source.Application.CQRS;
using BuildingBlocks.Source.Application.Dtos.Pagination;
using Sac.Domain.Dtos;

#endregion

namespace Sac.Application.Modules.Cursos.Queries.GetCursos;

/// <summary>
/// Lista cursos paginada. RF-10: el Docente solo ve los cursos que tiene asignados;
/// el Director ve todos.
/// </summary>
public record GetCursosQuery(PaginationRequest Pagination) : IQuery<PaginatedResult<CursoDto>>;
