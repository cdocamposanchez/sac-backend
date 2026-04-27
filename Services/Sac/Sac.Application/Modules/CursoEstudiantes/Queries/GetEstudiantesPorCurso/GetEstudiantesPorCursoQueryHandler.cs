#region

using BuildingBlocks.Source.Application.CQRS;
using BuildingBlocks.Source.Domain.Exception;
using Microsoft.EntityFrameworkCore;
using Sac.Application.Persistance;
using Sac.Domain.Dtos;

#endregion

namespace Sac.Application.Modules.CursoEstudiantes.Queries.GetEstudiantesPorCurso;

internal sealed class GetEstudiantesPorCursoQueryHandler(IApplicationDbContext db)
    : IQueryHandler<GetEstudiantesPorCursoQuery, List<CursoEstudianteDto>>
{
    public async Task<List<CursoEstudianteDto>> Handle(GetEstudiantesPorCursoQuery request, CancellationToken ct)
    {
        if (!await db.Cursos.AnyAsync(c => c.Id == request.CursoId, ct))
            throw new NotFoundException("Curso", request.CursoId);

        var asignaciones = await db.CursoEstudiantes.AsNoTracking()
            .Where(ce => ce.CursoId == request.CursoId && ce.Activo)
            .ToListAsync(ct);

        var ids = asignaciones.Select(a => a.EstudianteId).ToList();
        var estudiantes = await db.Estudiantes.AsNoTracking()
            .Where(e => ids.Contains(e.Id))
            .ToDictionaryAsync(e => e.Id, ct);

        return asignaciones
            .Select(a => new CursoEstudianteDto(
                a.Id, a.CursoId, a.EstudianteId,
                estudiantes.TryGetValue(a.EstudianteId, out var est) ? est.NombreCompleto : "Desconocido",
                a.FechaAsignacion, a.Activo))
            .OrderBy(d => d.EstudianteNombre)
            .ToList();
    }
}
