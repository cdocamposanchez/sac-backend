#region

using BuildingBlocks.Source.Application.CQRS;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Sac.Application.Persistance;
using Sac.Domain.Dtos;

#endregion

namespace Sac.Application.Modules.Materias.Queries.GetMaterias;

internal sealed class GetMateriasQueryHandler(IApplicationDbContext db)
    : IQueryHandler<GetMateriasQuery, List<MateriaDto>>
{
    public async Task<List<MateriaDto>> Handle(GetMateriasQuery request, CancellationToken ct)
    {
        var query = db.Materias.AsNoTracking().AsQueryable();
        if (request.SoloActivas) query = query.Where(m => m.Activo);

        var materias = await query.OrderBy(m => m.Nombre).ToListAsync(ct);
        return materias.Adapt<List<MateriaDto>>();
    }
}
