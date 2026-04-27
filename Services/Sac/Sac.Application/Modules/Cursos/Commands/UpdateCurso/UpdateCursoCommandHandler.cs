#region

using BuildingBlocks.Source.Application.CQRS;
using BuildingBlocks.Source.Domain.Exception;
using Microsoft.EntityFrameworkCore;
using Sac.Application.Modules.Cursos._Shared;
using Sac.Application.Persistance;
using Sac.Domain.Dtos;

#endregion

namespace Sac.Application.Modules.Cursos.Commands.UpdateCurso;

internal sealed class UpdateCursoCommandHandler(IApplicationDbContext db)
    : ICommandHandler<UpdateCursoCommand, CursoDto>
{
    public async Task<CursoDto> Handle(UpdateCursoCommand request, CancellationToken ct)
    {
        var curso = await db.Cursos.FirstOrDefaultAsync(c => c.Id == request.Id, ct)
                    ?? throw new NotFoundException("Curso", request.Id);

        var dto = request.Curso;

        if (!await db.Docentes.AnyAsync(d => d.Id == dto.DocenteId && d.Activo, ct))
            throw new NotFoundException("Docente", dto.DocenteId);
        if (!await db.Grados.AnyAsync(g => g.Id == dto.GradoId, ct))
            throw new NotFoundException("Grado", dto.GradoId);
        if (!await db.Materias.AnyAsync(m => m.Id == dto.MateriaId, ct))
            throw new NotFoundException("Materia", dto.MateriaId);

        // El propio agregado bloquea la modificación si el curso está cerrado
        curso.Actualizar(dto.Nombre, dto.GradoId, dto.MateriaId, dto.DocenteId);
        _ = await db.SaveChangesAsync(ct);

        return await CursoMapper.BuildAsync(db, curso, ct);
    }
}
