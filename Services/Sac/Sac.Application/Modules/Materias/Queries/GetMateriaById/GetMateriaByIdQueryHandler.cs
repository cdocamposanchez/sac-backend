#region

using BuildingBlocks.Source.Application.CQRS;
using BuildingBlocks.Source.Domain.Exception;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Sac.Application.Persistance;
using Sac.Domain.Dtos;

#endregion

namespace Sac.Application.Modules.Materias.Queries.GetMateriaById;

internal sealed class GetMateriaByIdQueryHandler(IApplicationDbContext db)
    : IQueryHandler<GetMateriaByIdQuery, MateriaDto>
{
    public async Task<MateriaDto> Handle(GetMateriaByIdQuery request, CancellationToken ct)
    {
        var materia = await db.Materias.AsNoTracking()
            .FirstOrDefaultAsync(m => m.Id == request.Id, ct)
            ?? throw new NotFoundException("Materia", request.Id);

        return materia.Adapt<MateriaDto>();
    }
}
