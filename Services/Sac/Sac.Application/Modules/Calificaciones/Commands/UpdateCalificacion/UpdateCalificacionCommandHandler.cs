#region

using BuildingBlocks.Source.Application.CQRS;
using BuildingBlocks.Source.Domain.Exception;
using BuildingBlocks.Source.Infrastructure.Security;
using Microsoft.EntityFrameworkCore;
using Sac.Application.Modules.Calificaciones._Shared;
using Sac.Application.Persistance;
using Sac.Domain.Dtos;

#endregion

namespace Sac.Application.Modules.Calificaciones.Commands.UpdateCalificacion;

internal sealed class UpdateCalificacionCommandHandler(
    IApplicationDbContext db,
    ICurrentUserService currentUser) : ICommandHandler<UpdateCalificacionCommand, CalificacionDto>
{
    public async Task<CalificacionDto> Handle(UpdateCalificacionCommand request, CancellationToken ct)
    {
        await using var tx = await db.BeginTransactionAsync(ct);
        try
        {
            var calificacion = await db.Calificaciones.FirstOrDefaultAsync(c => c.Id == request.Id, ct)
                               ?? throw new NotFoundException("Calificación", request.Id);

            var curso = await db.Cursos.FirstAsync(c => c.Id == calificacion.CursoId, ct);
            var periodo = await db.Periodos.FirstAsync(p => p.Id == calificacion.PeriodoId, ct);

            if (currentUser.IsDocente && currentUser.UserId.HasValue
                && !curso.PerteneceADocente(currentUser.UserId.Value))
                throw new UnauthorizedException("No tiene permisos para modificar esta calificación.");

            // El agregado lanza DomainException si el periodo está cerrado o la nota está fuera de rango
            calificacion.Actualizar(request.Calificacion.Nota, periodo.EstaAbierto);
            _ = await db.SaveChangesAsync(ct);

            // RF-09: recalcular promedio del periodo
            await CalificacionShared.RecalcularPromedioPeriodoAsync(db, periodo, ct);
            _ = await db.SaveChangesAsync(ct);

            await tx.CommitAsync(ct);
            return await CalificacionShared.BuildDtoAsync(db, calificacion, ct);
        }
        catch
        {
            await tx.RollbackAsync(ct);
            throw;
        }
    }
}
