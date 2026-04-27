#region

using BuildingBlocks.Source.Application.CQRS;
using BuildingBlocks.Source.Domain.Exception;
using Microsoft.EntityFrameworkCore;
using Sac.Application.Modules.Cursos._Shared;
using Sac.Application.Persistance;
using Sac.Domain.Dtos;

#endregion

namespace Sac.Application.Modules.Cursos.Commands.CerrarCurso;

internal sealed class CerrarCursoCommandHandler(IApplicationDbContext db)
    : ICommandHandler<CerrarCursoCommand, CursoDto>
{
    public async Task<CursoDto> Handle(CerrarCursoCommand request, CancellationToken ct)
    {
        var curso = await db.Cursos.FirstOrDefaultAsync(c => c.Id == request.Id, ct)
                    ?? throw new NotFoundException("Curso", request.Id);

        var periodos = await db.Periodos
            .Where(p => p.CursoId == request.Id)
            .ToListAsync(ct);

        // El agregado valida la regla: los 3 periodos deben estar cerrados
        curso.Cerrar(periodos);

        _ = await db.SaveChangesAsync(ct);
        return await CursoMapper.BuildAsync(db, curso, ct);
    }
}
