#region

using BuildingBlocks.Source.Application.CQRS;
using BuildingBlocks.Source.Domain.Exception;
using Microsoft.EntityFrameworkCore;
using Sac.Application.Persistance;
using Sac.Domain.Dtos;

#endregion

namespace Sac.Application.Modules.Periodos.Queries.GetPeriodosPorCurso;

internal sealed class GetPeriodosPorCursoQueryHandler(IApplicationDbContext db)
    : IQueryHandler<GetPeriodosPorCursoQuery, List<PeriodoDto>>
{
    public async Task<List<PeriodoDto>> Handle(GetPeriodosPorCursoQuery request, CancellationToken ct)
    {
        if (!await db.Cursos.AnyAsync(c => c.Id == request.CursoId, ct))
            throw new NotFoundException("Curso", request.CursoId);

        var periodos = await db.Periodos.AsNoTracking()
            .Where(p => p.CursoId == request.CursoId)
            .OrderBy(p => p.NumeroCorte)
            .ToListAsync(ct);

        return periodos
            .Select(p => new PeriodoDto(
                p.Id, p.CursoId, (int)p.NumeroCorte,
                p.Estado.ToString(), p.FechaCierre, p.PromedioPeriodo))
            .ToList();
    }
}
