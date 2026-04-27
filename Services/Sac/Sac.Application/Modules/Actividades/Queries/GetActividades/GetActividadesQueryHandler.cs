#region

using BuildingBlocks.Source.Application.CQRS;
using BuildingBlocks.Source.Domain.Exception;
using BuildingBlocks.Source.Infrastructure.Security;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Sac.Application.Persistance;
using Sac.Domain.Dtos;

#endregion

namespace Sac.Application.Modules.Actividades.Queries.GetActividades;

internal sealed class GetActividadesQueryHandler(
    IApplicationDbContext db,
    ICurrentUserService currentUser) : IQueryHandler<GetActividadesQuery, List<ActividadDto>>
{
    public async Task<List<ActividadDto>> Handle(GetActividadesQuery request, CancellationToken ct)
    {
        var curso = await db.Cursos.AsNoTracking().FirstOrDefaultAsync(c => c.Id == request.CursoId, ct)
                    ?? throw new NotFoundException("Curso", request.CursoId);

        if (currentUser.IsDocente && currentUser.UserId.HasValue
            && !curso.PerteneceADocente(currentUser.UserId.Value))
            throw new UnauthorizedException("No tiene permisos para acceder a este curso.");

        var query = db.Actividades.AsNoTracking()
            .Where(a => a.CursoId == request.CursoId && a.Activo);

        if (request.PeriodoId.HasValue)
            query = query.Where(a => a.PeriodoId == request.PeriodoId.Value);

        var actividades = await query.OrderBy(a => a.PeriodoId).ThenBy(a => a.Nombre).ToListAsync(ct);
        return actividades.Adapt<List<ActividadDto>>();
    }
}
