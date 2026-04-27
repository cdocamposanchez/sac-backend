#region

using BuildingBlocks.Source.Application.CQRS;
using BuildingBlocks.Source.Application.Dtos.Pagination;
using Sac.Domain.Dtos;

#endregion

namespace Sac.Application.Modules.Docentes.Queries.GetDocentes;

/// <summary>Lista paginada de Docentes. Accesible para Director y Docentes.</summary>
public record GetDocentesQuery(PaginationRequest Pagination) : IQuery<PaginatedResult<DocenteDto>>;
