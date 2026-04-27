#region

using BuildingBlocks.Source.Application.CQRS;
using BuildingBlocks.Source.Domain.Exception;
using BuildingBlocks.Source.Infrastructure.Security;
using Microsoft.EntityFrameworkCore;
using Sac.Application.Modules.Reportes._Shared;
using Sac.Application.Persistance;
using Sac.Domain.Dtos;

#endregion

namespace Sac.Application.Modules.Reportes.Queries.GetReporteEstudiante;

internal sealed class GetReporteEstudianteQueryHandler(
    IApplicationDbContext db,
    ICurrentUserService currentUser) : IQueryHandler<GetReporteEstudianteQuery, ReporteEstudianteDto>
{
    public async Task<ReporteEstudianteDto> Handle(GetReporteEstudianteQuery request, CancellationToken ct)
    {
        var curso = await db.Cursos.AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == request.CursoId, ct)
            ?? throw new NotFoundException("Curso", request.CursoId);

        if (currentUser.IsDocente && currentUser.UserId.HasValue
            && !curso.PerteneceADocente(currentUser.UserId.Value))
            throw new UnauthorizedException("No tiene permisos para acceder a este curso.");

        var estudiante = await db.Estudiantes.AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == request.EstudianteId, ct)
            ?? throw new NotFoundException("Estudiante", request.EstudianteId);

        return await ReporteBuilder.ConstruirReporteEstudianteAsync(db, curso, estudiante, ct);
    }
}
