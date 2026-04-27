#region

using BuildingBlocks.Source.Application.CQRS;
using BuildingBlocks.Source.Domain.Exception;
using Microsoft.EntityFrameworkCore;
using Sac.Application.Persistance;

#endregion

namespace Sac.Application.Modules.CursoEstudiantes.Commands.QuitarEstudiante;

internal sealed class QuitarEstudianteCommandHandler(IApplicationDbContext db)
    : ICommandHandler<QuitarEstudianteCommand, bool>
{
    public async Task<bool> Handle(QuitarEstudianteCommand request, CancellationToken ct)
    {
        var asignacion = await db.CursoEstudiantes
            .FirstOrDefaultAsync(ce => ce.CursoId == request.CursoId && ce.EstudianteId == request.EstudianteId, ct)
            ?? throw new NotFoundException(
                $"No existe asignación del estudiante {request.EstudianteId} en el curso {request.CursoId}.");

        var curso = await db.Cursos.FirstAsync(c => c.Id == request.CursoId, ct);
        if (curso.EstaCerrado)
            throw new BadRequestException("No se puede modificar un curso cerrado.");

        // Validación de integridad: si ya existen calificaciones del estudiante en el curso,
        // no permitir quitar (rompería la trazabilidad)
        var tieneCalificaciones = await db.Calificaciones
            .AnyAsync(c => c.CursoId == request.CursoId && c.EstudianteId == request.EstudianteId, ct);

        if (tieneCalificaciones)
            throw new BadRequestException(
                "No se puede quitar el estudiante: ya tiene calificaciones registradas en el curso.");

        asignacion.Desactivar();
        _ = await db.SaveChangesAsync(ct);
        return true;
    }
}
