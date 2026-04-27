#region

using BuildingBlocks.Source.Application.CQRS;
using BuildingBlocks.Source.Domain.Exception;
using BuildingBlocks.Source.Infrastructure.Security;
using Microsoft.EntityFrameworkCore;
using Sac.Application.Modules.Reportes._Shared;
using Sac.Application.Persistance;
using Sac.Domain.Dtos;

#endregion

namespace Sac.Application.Modules.Reportes.Queries.GetReporteFinalCurso;

internal sealed class GetReporteFinalCursoQueryHandler(
    IApplicationDbContext db,
    ICurrentUserService currentUser) : IQueryHandler<GetReporteFinalCursoQuery, List<ReporteEstudianteDto>>
{
    public async Task<List<ReporteEstudianteDto>> Handle(GetReporteFinalCursoQuery request, CancellationToken ct)
    {
        var curso = await db.Cursos.AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == request.CursoId, ct)
            ?? throw new NotFoundException("Curso", request.CursoId);

        // RF-10: el reporte final solo está disponible cuando el curso está cerrado
        if (!curso.EstaCerrado)
            throw new BadRequestException(
                "El reporte final solo está disponible cuando el curso está CERRADO.");

        if (currentUser.IsDocente && currentUser.UserId.HasValue
            && !curso.PerteneceADocente(currentUser.UserId.Value))
            throw new UnauthorizedException("No tiene permisos para acceder a este curso.");

        var estudianteIds = await db.CursoEstudiantes.AsNoTracking()
            .Where(ce => ce.CursoId == request.CursoId && ce.Activo)
            .Select(ce => ce.EstudianteId)
            .ToListAsync(ct);

        var estudiantes = await db.Estudiantes.AsNoTracking()
            .Where(e => estudianteIds.Contains(e.Id))
            .OrderBy(e => e.NombreCompleto)
            .ToListAsync(ct);

        var reportes = new List<ReporteEstudianteDto>(estudiantes.Count);
        foreach (var est in estudiantes)
            reportes.Add(await ReporteBuilder.ConstruirReporteEstudianteAsync(db, curso, est, ct));

        return reportes;
    }
}
