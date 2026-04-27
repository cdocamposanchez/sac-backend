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

namespace Sac.Application.Modules.Actividades.Commands.UpdateActividad;

internal sealed class UpdateActividadCommandHandler(
    IApplicationDbContext db,
    ICurrentUserService currentUser) : ICommandHandler<UpdateActividadCommand, ActividadDto>
{
    public async Task<ActividadDto> Handle(UpdateActividadCommand request, CancellationToken ct)
    {
        var actividad = await db.Actividades.FirstOrDefaultAsync(a => a.Id == request.Id, ct)
                        ?? throw new NotFoundException("Actividad", request.Id);

        var curso = await db.Cursos.FirstAsync(c => c.Id == actividad.CursoId, ct);
        var periodo = await db.Periodos.FirstAsync(p => p.Id == actividad.PeriodoId, ct);

        if (currentUser.IsDocente && currentUser.UserId.HasValue
            && !curso.PerteneceADocente(currentUser.UserId.Value))
            throw new UnauthorizedException("No tiene permisos para modificar esta actividad.");

        if (periodo.EstaCerrado)
            throw new BadRequestException("No se pueden modificar actividades en un periodo cerrado.");

        // RF-08: validar suma de porcentajes excluyendo la actividad que se está editando
        var actividadesCurso = await db.Actividades.AsNoTracking()
            .Where(a => a.CursoId == actividad.CursoId)
            .ToListAsync(ct);

        var sumaSinEsta = PromedioCalculator.SumaPorcentajesActividadesCurso(
            actividadesCurso, excluirId: actividad.Id);
        DomainActividad.ValidarSumaPorcentajesCurso(sumaSinEsta, request.Actividad.Porcentaje);

        actividad.Actualizar(request.Actividad.Nombre, request.Actividad.Descripcion, request.Actividad.Porcentaje);
        _ = await db.SaveChangesAsync(ct);

        return actividad.Adapt<ActividadDto>();
    }
}
