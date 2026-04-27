#region

using BuildingBlocks.Source.Application.CQRS;
using BuildingBlocks.Source.Domain.Exception;
using Microsoft.EntityFrameworkCore;
using Sac.Application.Persistance;
using Sac.Domain.Dtos;

#endregion

namespace Sac.Application.Modules.Periodos.Queries.GetPeriodoById;

internal sealed class GetPeriodoByIdQueryHandler(IApplicationDbContext db)
    : IQueryHandler<GetPeriodoByIdQuery, PeriodoDto>
{
    public async Task<PeriodoDto> Handle(GetPeriodoByIdQuery request, CancellationToken ct)
    {
        var p = await db.Periodos.AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == request.Id, ct)
            ?? throw new NotFoundException("Periodo", request.Id);

        return new PeriodoDto(p.Id, p.CursoId, (int)p.NumeroCorte,
            p.Estado.ToString(), p.FechaCierre, p.PromedioPeriodo);
    }
}
