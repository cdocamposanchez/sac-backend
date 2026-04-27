#region

using BuildingBlocks.Source.Application.CQRS;
using BuildingBlocks.Source.Domain.Exception;
using BuildingBlocks.Source.Infrastructure.Security;
using Microsoft.EntityFrameworkCore;
using Sac.Application.Persistance;

#endregion

namespace Sac.Application.Modules.Actividades.Commands.DeleteActividad;

internal sealed class DeleteActividadCommandHandler(
    IApplicationDbContext db,
    ICurrentUserService currentUser) : ICommandHandler<DeleteActividadCommand, bool>
{
    public async Task<bool> Handle(DeleteActividadCommand request, CancellationToken ct)
    {
        var actividad = await db.Actividades.FirstOrDefaultAsync(a => a.Id == request.Id, ct)
                        ?? throw new NotFoundException("Actividad", request.Id);

        var curso = await db.Cursos.FirstAsync(c => c.Id == actividad.CursoId, ct);
        var periodo = await db.Periodos.FirstAsync(p => p.Id == actividad.PeriodoId, ct);

        if (currentUser.IsDocente && currentUser.UserId.HasValue
            && !curso.PerteneceADocente(currentUser.UserId.Value))
            throw new UnauthorizedException("No tiene permisos para eliminar esta actividad.");

        if (periodo.EstaCerrado)
            throw new BadRequestException("No se pueden eliminar actividades en un periodo cerrado.");

        // Si tiene calificaciones registradas, no permitir borrado lógico
        var tieneNotas = await db.Calificaciones.AnyAsync(c => c.ActividadId == request.Id, ct);
        if (tieneNotas)
            throw new BadRequestException(
                "No se puede eliminar la actividad: tiene calificaciones registradas. " +
                "Elimine primero las calificaciones o no la elimine si tiene historial.");

        actividad.Desactivar();
        _ = await db.SaveChangesAsync(ct);
        return true;
    }
}
