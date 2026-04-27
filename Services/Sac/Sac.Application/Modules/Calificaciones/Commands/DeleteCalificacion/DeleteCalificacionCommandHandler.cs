#region

using BuildingBlocks.Source.Application.CQRS;
using BuildingBlocks.Source.Domain.Exception;
using BuildingBlocks.Source.Infrastructure.Security;
using Microsoft.EntityFrameworkCore;
using Sac.Application.Modules.Calificaciones._Shared;
using Sac.Application.Persistance;

#endregion

namespace Sac.Application.Modules.Calificaciones.Commands.DeleteCalificacion;

internal sealed class DeleteCalificacionCommandHandler(
    IApplicationDbContext db,
    ICurrentUserService currentUser) : ICommandHandler<DeleteCalificacionCommand, bool>
{
    public async Task<bool> Handle(DeleteCalificacionCommand request, CancellationToken ct)
    {
        await using var tx = await db.BeginTransactionAsync(ct);
        try
        {
            var calificacion = await db.Calificaciones.FirstOrDefaultAsync(c => c.Id == request.Id, ct)
                               ?? throw new NotFoundException("Calificación", request.Id);

            var curso = await db.Cursos.FirstAsync(c => c.Id == calificacion.CursoId, ct);
            var periodo = await db.Periodos.FirstAsync(p => p.Id == calificacion.PeriodoId, ct);

            if (currentUser is { IsDocente: true, UserId: not null }
                && !curso.PerteneceADocente(currentUser.UserId.Value))
                throw new UnauthorizedException("No tiene permisos para eliminar esta calificación.");

            if (!calificacion.PuedeEliminarse(periodo.EstaAbierto))
                throw new BadRequestException("No se pueden eliminar calificaciones en un periodo cerrado.");

            db.Calificaciones.Remove(calificacion);
            _ = await db.SaveChangesAsync(ct);

            await CalificacionShared.RecalcularPromedioPeriodoAsync(db, periodo, ct);
            _ = await db.SaveChangesAsync(ct);

            await tx.CommitAsync(ct);
            return true;
        }
        catch
        {
            await tx.RollbackAsync(ct);
            throw;
        }
    }
}
