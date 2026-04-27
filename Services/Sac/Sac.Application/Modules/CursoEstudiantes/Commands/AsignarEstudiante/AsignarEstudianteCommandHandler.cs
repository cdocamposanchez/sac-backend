#region

using BuildingBlocks.Source.Application.CQRS;
using BuildingBlocks.Source.Domain.Exception;
using Microsoft.EntityFrameworkCore;
using Sac.Application.Persistance;
using Sac.Domain.Dtos;
using Sac.Domain.Models;

#endregion

namespace Sac.Application.Modules.CursoEstudiantes.Commands.AsignarEstudiante;

internal sealed class AsignarEstudianteCommandHandler(IApplicationDbContext db)
    : ICommandHandler<AsignarEstudianteCommand, CursoEstudianteDto>
{
    public async Task<CursoEstudianteDto> Handle(AsignarEstudianteCommand request, CancellationToken ct)
    {
        var dto = request.Datos;

        var curso = await db.Cursos.FirstOrDefaultAsync(c => c.Id == dto.CursoId, ct)
                    ?? throw new NotFoundException("Curso", dto.CursoId);

        var estudiante = await db.Estudiantes.FirstOrDefaultAsync(e => e.Id == dto.EstudianteId, ct)
                         ?? throw new NotFoundException("Estudiante", dto.EstudianteId);

        if (curso.EstaCerrado)
            throw new BadRequestException("No se pueden asignar estudiantes a un curso cerrado.");

        if (!estudiante.Activo)
            throw new BadRequestException("No se puede asignar un estudiante inactivo.");

        var existente = await db.CursoEstudiantes
            .FirstOrDefaultAsync(ce => ce.CursoId == dto.CursoId && ce.EstudianteId == dto.EstudianteId, ct);

        if (existente != null)
        {
            if (existente.Activo)
                throw new BadRequestException("El estudiante ya está asignado a este curso.");

            // Reactivar la asignación anterior
            existente.Activar();
            _ = await db.SaveChangesAsync(ct);

            return new CursoEstudianteDto(existente.Id, existente.CursoId, existente.EstudianteId,
                estudiante.NombreCompleto, existente.FechaAsignacion, existente.Activo);
        }

        var asignacion = CursoEstudiante.Create(dto.CursoId, dto.EstudianteId);
        _ = await db.CursoEstudiantes.AddAsync(asignacion, ct);
        _ = await db.SaveChangesAsync(ct);

        return new CursoEstudianteDto(asignacion.Id, asignacion.CursoId, asignacion.EstudianteId,
            estudiante.NombreCompleto, asignacion.FechaAsignacion, asignacion.Activo);
    }
}
