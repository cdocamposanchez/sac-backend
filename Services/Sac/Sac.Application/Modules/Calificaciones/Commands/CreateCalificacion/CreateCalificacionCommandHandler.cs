#region

using BuildingBlocks.Source.Application.CQRS;
using BuildingBlocks.Source.Domain.Exception;
using BuildingBlocks.Source.Infrastructure.Security;
using Microsoft.EntityFrameworkCore;
using Sac.Application.Modules.Calificaciones._Shared;
using Sac.Application.Persistance;
using Sac.Domain.Dtos;
using DomainCalificacion = Sac.Domain.Models.Calificacion;

#endregion

namespace Sac.Application.Modules.Calificaciones.Commands.CreateCalificacion;

internal sealed class CreateCalificacionCommandHandler(
    IApplicationDbContext db,
    ICurrentUserService currentUser) : ICommandHandler<CreateCalificacionCommand, CalificacionDto>
{
    public async Task<CalificacionDto> Handle(CreateCalificacionCommand request, CancellationToken ct)
    {
        var dto = request.Calificacion;

        await using var tx = await db.BeginTransactionAsync(ct);
        try
        {
            // Validaciones de existencia y consistencia
            var curso = await db.Cursos.FirstOrDefaultAsync(c => c.Id == dto.CursoId, ct)
                        ?? throw new NotFoundException("Curso", dto.CursoId);

            var actividad = await db.Actividades.FirstOrDefaultAsync(a => a.Id == dto.ActividadId, ct)
                            ?? throw new NotFoundException("Actividad", dto.ActividadId);

            var periodo = await db.Periodos.FirstOrDefaultAsync(p => p.Id == dto.PeriodoId, ct)
                          ?? throw new NotFoundException("Periodo", dto.PeriodoId);

            if (actividad.CursoId != dto.CursoId || actividad.PeriodoId != dto.PeriodoId)
                throw new BadRequestException("La actividad no pertenece al curso/periodo indicados.");

            // RF-08: solo el docente titular del curso puede registrar notas
            if (currentUser.IsDocente && currentUser.UserId.HasValue
                && !curso.PerteneceADocente(currentUser.UserId.Value))
                throw new UnauthorizedException("No tiene permisos para registrar notas en este curso.");

            if (curso.EstaCerrado)
                throw new BadRequestException("No se pueden registrar notas en un curso cerrado.");

            if (periodo.EstaCerrado)
                throw new BadRequestException("No se pueden registrar notas en un periodo cerrado.");

            // Estudiante debe estar inscrito y activo en el curso
            var inscripcion = await db.CursoEstudiantes
                .FirstOrDefaultAsync(ce => ce.CursoId == dto.CursoId && ce.EstudianteId == dto.EstudianteId, ct);

            if (inscripcion == null || !inscripcion.Activo)
                throw new BadRequestException("El estudiante no está asignado a este curso.");

            // RNF-05: una calificación por (estudiante, actividad)
            var yaExiste = await db.Calificaciones.AnyAsync(
                c => c.EstudianteId == dto.EstudianteId && c.ActividadId == dto.ActividadId, ct);

            if (yaExiste)
                throw new BadRequestException(
                    "Ya existe una calificación para este estudiante en esta actividad. " +
                    "Edítela en lugar de crearla.");

            // Crear (el agregado valida nota ∈ [1.0, 5.0]).
            // El porcentaje ya no se pasa: vive en la actividad asociada.
            var calificacion = DomainCalificacion.Create(
                dto.EstudianteId, dto.ActividadId, dto.CursoId, dto.PeriodoId, dto.Nota);

            _ = await db.Calificaciones.AddAsync(calificacion, ct);
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
