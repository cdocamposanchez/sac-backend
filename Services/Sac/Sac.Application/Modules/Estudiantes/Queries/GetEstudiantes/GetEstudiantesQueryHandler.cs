#region

using BuildingBlocks.Source.Application.CQRS;
using BuildingBlocks.Source.Application.Dtos.Pagination;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Sac.Application.Persistance;
using Sac.Domain.Dtos;

#endregion

namespace Sac.Application.Modules.Estudiantes.Queries.GetEstudiantes;

internal sealed class GetEstudiantesQueryHandler(IApplicationDbContext db)
    : IQueryHandler<GetEstudiantesQuery, PaginatedResult<EstudianteDto>>
{
    public async Task<PaginatedResult<EstudianteDto>> Handle(GetEstudiantesQuery request, CancellationToken ct)
    {
        var query = db.Estudiantes.AsNoTracking().OrderBy(e => e.NombreCompleto);
        var total = await query.LongCountAsync(ct);
        var items = await query.Skip(request.Pagination.Skip).Take(request.Pagination.Take).ToListAsync(ct);

        return new PaginatedResult<EstudianteDto>(
            request.Pagination.PageIndex,
            request.Pagination.Take,
            total,
            items.Adapt<List<EstudianteDto>>());
    }
}
