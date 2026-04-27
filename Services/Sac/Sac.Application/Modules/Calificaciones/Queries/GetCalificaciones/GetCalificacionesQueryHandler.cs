#region

using BuildingBlocks.Source.Application.CQRS;
using BuildingBlocks.Source.Infrastructure.Security;
using Microsoft.EntityFrameworkCore;
using Sac.Application.Modules.Calificaciones._Shared;
using Sac.Application.Persistance;
using Sac.Domain.Dtos;

#endregion

namespace Sac.Application.Modules.Calificaciones.Queries.GetCalificaciones;

internal sealed class GetCalificacionesQueryHandler(
    IApplicationDbContext db,
    ICurrentUserService currentUser) : IQueryHandler<GetCalificacionesQuery, List<CalificacionDto>>
{
    public async Task<List<CalificacionDto>> Handle(GetCalificacionesQuery request, CancellationToken ct)
    {
        var query = db.Calificaciones.AsNoTracking().AsQueryable();

        if (request.CursoId.HasValue) query = query.Where(c => c.CursoId == request.CursoId.Value);
        if (request.PeriodoId.HasValue) query = query.Where(c => c.PeriodoId == request.PeriodoId.Value);
        if (request.ActividadId.HasValue) query = query.Where(c => c.ActividadId == request.ActividadId.Value);
        if (request.EstudianteId.HasValue) query = query.Where(c => c.EstudianteId == request.EstudianteId.Value);

        // RF-10: Docente solo ve calificaciones de sus cursos
        if (currentUser.IsDocente && currentUser.UserId.HasValue)
        {
            var docenteId = currentUser.UserId.Value;
            var misCursos = await db.Cursos
                .Where(c => c.DocenteId == docenteId)
                .Select(c => c.Id)
                .ToListAsync(ct);

            query = query.Where(c => misCursos.Contains(c.CursoId));
        }

        var lista = await query.OrderByDescending(c => c.FechaReg).ToListAsync(ct);

        var result = new List<CalificacionDto>(lista.Count);
        foreach (var c in lista)
            result.Add(await CalificacionShared.BuildDtoAsync(db, c, ct));

        return result;
    }
}
