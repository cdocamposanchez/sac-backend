#region

using BuildingBlocks.Source.Application.CQRS;
using BuildingBlocks.Source.Domain.Exception;
using Microsoft.EntityFrameworkCore;
using Sac.Application.Modules.Calificaciones._Shared;
using Sac.Application.Persistance;
using Sac.Domain.Dtos;

#endregion

namespace Sac.Application.Modules.Periodos.Commands.CerrarPeriodo;

internal sealed class CerrarPeriodoCommandHandler(IApplicationDbContext db)
    : ICommandHandler<CerrarPeriodoCommand, PeriodoDto>
{
    public async Task<PeriodoDto> Handle(CerrarPeriodoCommand request, CancellationToken ct)
    {
        var periodo = await db.Periodos.FirstOrDefaultAsync(p => p.Id == request.Id, ct)
                      ?? throw new NotFoundException("Periodo", request.Id);

        await using var tx = await db.BeginTransactionAsync(ct);
        try
        {
            // RF-09: recalcular el promedio agregado del periodo (resuelve porcentajes vía actividades)
            await CalificacionShared.RecalcularPromedioPeriodoAsync(db, periodo, ct);

            periodo.Cerrar();
            _ = await db.SaveChangesAsync(ct);
            await tx.CommitAsync(ct);

            return new PeriodoDto(periodo.Id, periodo.CursoId, (int)periodo.NumeroCorte,
                periodo.Estado.ToString(), periodo.FechaCierre, periodo.PromedioPeriodo);
        }
        catch
        {
            await tx.RollbackAsync(ct);
            throw;
        }
    }
}
