#region

using BuildingBlocks.Source.Application.CQRS;
using BuildingBlocks.Source.Domain.Exception;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Sac.Application.Persistance;
using Sac.Domain.Dtos;

#endregion

namespace Sac.Application.Modules.Actividades.Queries.GetActividadById;

internal sealed class GetActividadByIdQueryHandler(IApplicationDbContext db)
    : IQueryHandler<GetActividadByIdQuery, ActividadDto>
{
    public async Task<ActividadDto> Handle(GetActividadByIdQuery request, CancellationToken ct)
    {
        var actividad = await db.Actividades.AsNoTracking()
            .FirstOrDefaultAsync(a => a.Id == request.Id, ct)
            ?? throw new NotFoundException("Actividad", request.Id);

        return actividad.Adapt<ActividadDto>();
    }
}
