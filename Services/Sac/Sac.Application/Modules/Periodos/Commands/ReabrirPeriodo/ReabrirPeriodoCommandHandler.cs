#region

using BuildingBlocks.Source.Application.CQRS;
using BuildingBlocks.Source.Domain.Exception;
using Microsoft.EntityFrameworkCore;
using Sac.Application.Persistance;
using Sac.Domain.Dtos;

#endregion

namespace Sac.Application.Modules.Periodos.Commands.ReabrirPeriodo;

internal sealed class ReabrirPeriodoCommandHandler(IApplicationDbContext db)
    : ICommandHandler<ReabrirPeriodoCommand, PeriodoDto>
{
    public async Task<PeriodoDto> Handle(ReabrirPeriodoCommand request, CancellationToken ct)
    {
        var periodo = await db.Periodos.FirstOrDefaultAsync(p => p.Id == request.Id, ct)
                      ?? throw new NotFoundException("Periodo", request.Id);

        var curso = await db.Cursos.FirstAsync(c => c.Id == periodo.CursoId, ct);
        if (curso.EstaCerrado)
            throw new BadRequestException("No se puede reabrir un periodo de un curso que ya está cerrado.");

        periodo.Reabrir();
        _ = await db.SaveChangesAsync(ct);

        return new PeriodoDto(periodo.Id, periodo.CursoId, (int)periodo.NumeroCorte,
            periodo.Estado.ToString(), periodo.FechaCierre, periodo.PromedioPeriodo);
    }
}
