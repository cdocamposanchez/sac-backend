#region

using BuildingBlocks.Source.Application.CQRS;
using BuildingBlocks.Source.Domain.Exception;
using BuildingBlocks.Source.Infrastructure.Security;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Sac.Application.Helpers;
using Sac.Application.Persistance;
using Sac.Domain.Dtos;
using DomainActividad = Sac.Domain.Models.Actividad;

#endregion

namespace Sac.Application.Modules.Actividades.Commands.CreateActividad;

internal sealed class CreateActividadCommandHandler(
    IApplicationDbContext db,
    ICurrentUserService currentUser) : ICommandHandler<CreateActividadCommand, ActividadDto>
{
    public async Task<ActividadDto> Handle(CreateActividadCommand request, CancellationToken ct)
    {
        var dto = request.Actividad;

        var curso = await db.Cursos.FirstOrDefaultAsync(c => c.Id == dto.CursoId, ct)
                    ?? throw new NotFoundException("Curso", dto.CursoId);

        var periodo = await db.Periodos.FirstOrDefaultAsync(p => p.Id == dto.PeriodoId && p.CursoId == dto.CursoId, ct)
                      ?? throw new NotFoundException(
                          $"El periodo {dto.PeriodoId} no pertenece al curso {dto.CursoId}.");

        // RF-07: solo el Docente titular del curso puede crear actividades
        if (currentUser.IsDocente && currentUser.UserId.HasValue
            && !curso.PerteneceADocente(currentUser.UserId.Value))
            throw new UnauthorizedException("No tiene permisos para crear actividades en este curso.");

        if (curso.EstaCerrado)
            throw new BadRequestException("No se pueden crear actividades en un curso cerrado.");

        if (periodo.EstaCerrado)
            throw new BadRequestException("No se pueden crear actividades en un periodo cerrado.");

        // RF-08: la suma de porcentajes de las actividades activas del curso no puede exceder 100%
        var actividadesCurso = await db.Actividades.AsNoTracking()
            .Where(a => a.CursoId == dto.CursoId)
            .ToListAsync(ct);

        var sumaActual = PromedioCalculator.SumaPorcentajesActividadesCurso(actividadesCurso);
        DomainActividad.ValidarSumaPorcentajesCurso(sumaActual, dto.Porcentaje);

        var actividad = DomainActividad.Create(
            dto.Nombre, dto.Descripcion, dto.CursoId, dto.PeriodoId, dto.Porcentaje);

        _ = await db.Actividades.AddAsync(actividad, ct);
        _ = await db.SaveChangesAsync(ct);

        return actividad.Adapt<ActividadDto>();
    }
}
