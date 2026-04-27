#region

using BuildingBlocks.Source.Application.CQRS;
using BuildingBlocks.Source.Domain.Exception;
using Microsoft.EntityFrameworkCore;
using Sac.Application.Persistance;
using Sac.Domain.Dtos;
using Sac.Domain.Models;

#endregion

namespace Sac.Application.Modules.CursoEstudiantes.Commands.AsignarEstudiantesLote;

internal sealed class AsignarEstudiantesLoteCommandHandler(IApplicationDbContext db)
    : ICommandHandler<AsignarEstudiantesLoteCommand, List<CursoEstudianteDto>>
{
    public async Task<List<CursoEstudianteDto>> Handle(AsignarEstudiantesLoteCommand request, CancellationToken ct)
    {
        var datos = request.Datos;
        var curso = await db.Cursos.FirstOrDefaultAsync(c => c.Id == datos.CursoId, ct)
                    ?? throw new NotFoundException("Curso", datos.CursoId);

        if (curso.EstaCerrado)
            throw new BadRequestException("No se pueden asignar estudiantes a un curso cerrado.");

        var idsUnicos = datos.EstudianteIds.Distinct().ToList();
        var resultado = new List<CursoEstudianteDto>();

        await using var tx = await db.BeginTransactionAsync(ct);
        try
        {
            foreach (var estudianteId in idsUnicos)
            {
                var est = await db.Estudiantes.FirstOrDefaultAsync(e => e.Id == estudianteId, ct);
                if (est == null || !est.Activo) continue;

                var existente = await db.CursoEstudiantes
                    .FirstOrDefaultAsync(ce => ce.CursoId == datos.CursoId && ce.EstudianteId == estudianteId, ct);

                if (existente != null)
                {
                    if (!existente.Activo) existente.Activar();
                    resultado.Add(new CursoEstudianteDto(existente.Id, existente.CursoId, existente.EstudianteId,
                        est.NombreCompleto, existente.FechaAsignacion, existente.Activo));
                    continue;
                }

                var asignacion = CursoEstudiante.Create(datos.CursoId, estudianteId);
                _ = await db.CursoEstudiantes.AddAsync(asignacion, ct);
                resultado.Add(new CursoEstudianteDto(asignacion.Id, asignacion.CursoId, asignacion.EstudianteId,
                    est.NombreCompleto, asignacion.FechaAsignacion, asignacion.Activo));
            }

            _ = await db.SaveChangesAsync(ct);
            await tx.CommitAsync(ct);
            return resultado;
        }
        catch
        {
            await tx.RollbackAsync(ct);
            throw;
        }
    }
}
