#region

using BuildingBlocks.Source.Application.CQRS;
using BuildingBlocks.Source.Application.Dtos.Pagination;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Sac.Application.Persistance;
using Sac.Domain.Dtos;

#endregion

namespace Sac.Application.Modules.Docentes.Queries.GetDocentes;

internal sealed class GetDocentesQueryHandler(IApplicationDbContext db)
    : IQueryHandler<GetDocentesQuery, PaginatedResult<DocenteDto>>
{
    public async Task<PaginatedResult<DocenteDto>> Handle(GetDocentesQuery request, CancellationToken ct)
    {
        var query = db.Docentes.AsNoTracking().OrderBy(d => d.NombreCompleto);
        var total = await query.LongCountAsync(ct);

        var items = await query
            .Skip(request.Pagination.Skip)
            .Take(request.Pagination.Take)
            .ToListAsync(ct);

        return new PaginatedResult<DocenteDto>(
            request.Pagination.PageIndex,
            request.Pagination.Take,
            total,
            items.Adapt<List<DocenteDto>>());
    }
}
