#region

using BuildingBlocks.Source.Application.CQRS;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Sac.Application.Persistance;
using Sac.Domain.Dtos;

#endregion

namespace Sac.Application.Modules.Grados.Queries.GetGrados;

internal sealed class GetGradosQueryHandler(IApplicationDbContext db)
    : IQueryHandler<GetGradosQuery, List<GradoDto>>
{
    public async Task<List<GradoDto>> Handle(GetGradosQuery request, CancellationToken ct)
    {
        var query = db.Grados.AsNoTracking().AsQueryable();
        if (request.SoloActivos) query = query.Where(g => g.Activo);

        var grados = await query.OrderBy(g => g.Nombre).ToListAsync(ct);
        return grados.Adapt<List<GradoDto>>();
    }
}
