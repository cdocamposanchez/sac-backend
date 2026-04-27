#region

using BuildingBlocks.Source.Application.CQRS;
using BuildingBlocks.Source.Application.Dtos.Pagination;
using Sac.Domain.Dtos;

#endregion

namespace Sac.Application.Modules.Estudiantes.Queries.GetEstudiantes;

public record GetEstudiantesQuery(PaginationRequest Pagination) : IQuery<PaginatedResult<EstudianteDto>>;
