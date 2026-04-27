#region

using BuildingBlocks.Source.Application.CQRS;
using BuildingBlocks.Source.Domain.Exception;
using Microsoft.EntityFrameworkCore;
using Sac.Application.Modules.Calificaciones._Shared;
using Sac.Application.Persistance;
using Sac.Domain.Dtos;

#endregion

namespace Sac.Application.Modules.Calificaciones.Queries.GetCalificacionById;

internal sealed class GetCalificacionByIdQueryHandler(IApplicationDbContext db)
    : IQueryHandler<GetCalificacionByIdQuery, CalificacionDto>
{
    public async Task<CalificacionDto> Handle(GetCalificacionByIdQuery request, CancellationToken ct)
    {
        var c = await db.Calificaciones.AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == request.Id, ct)
            ?? throw new NotFoundException("Calificación", request.Id);

        return await CalificacionShared.BuildDtoAsync(db, c, ct);
    }
}
