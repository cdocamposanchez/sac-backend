#region

using BuildingBlocks.Source.Application.CQRS;
using BuildingBlocks.Source.Domain.Exception;
using Microsoft.EntityFrameworkCore;
using Sac.Application.Persistance;
using Sac.Domain.Enums;

#endregion

namespace Sac.Application.Modules.Docentes.Commands.DeleteDocente;

internal sealed class DeleteDocenteCommandHandler(IApplicationDbContext db)
    : ICommandHandler<DeleteDocenteCommand, bool>
{
    public async Task<bool> Handle(DeleteDocenteCommand request, CancellationToken ct)
    {
        var docente = await db.Docentes.FirstOrDefaultAsync(d => d.Id == request.Id, ct)
                      ?? throw new NotFoundException("Docente", request.Id);

        // No permitir desactivar si tiene cursos activos asignados (RF-04)
        var tieneCursosActivos = await db.Cursos
            .AnyAsync(c => c.DocenteId == request.Id && c.Estado == EstadoCurso.Activo, ct);

        if (tieneCursosActivos)
            throw new BadRequestException(
                "No se puede inactivar el docente: tiene cursos activos asignados. " +
                "Reasigne los cursos a otro docente antes de inactivarlo.");

        docente.Desactivar();
        _ = await db.SaveChangesAsync(ct);
        return true;
    }
}
