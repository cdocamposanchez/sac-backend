#region

using BuildingBlocks.Source.Application.CQRS;
using BuildingBlocks.Source.Domain.Exception;
using Microsoft.EntityFrameworkCore;
using Sac.Application.Modules.Cursos._Shared;
using Sac.Application.Persistance;
using Sac.Domain.Dtos;
using Sac.Domain.Enums;
using Sac.Domain.Models;

#endregion

namespace Sac.Application.Modules.Cursos.Commands.CreateCurso;

internal sealed class CreateCursoCommandHandler(IApplicationDbContext db)
    : ICommandHandler<CreateCursoCommand, CursoDto>
{
    public async Task<CursoDto> Handle(CreateCursoCommand request, CancellationToken ct)
    {
        var dto = request.Curso;

        if (!await db.Grados.AnyAsync(g => g.Id == dto.GradoId && g.Activo, ct))
            throw new NotFoundException("Grado", dto.GradoId);
        if (!await db.Materias.AnyAsync(m => m.Id == dto.MateriaId && m.Activo, ct))
            throw new NotFoundException("Materia", dto.MateriaId);
        if (!await db.Docentes.AnyAsync(d => d.Id == dto.DocenteId && d.Activo, ct))
            throw new NotFoundException("Docente", dto.DocenteId);

        // RF-04: combinación grado+materia+año debe ser única
        if (await db.Cursos.AnyAsync(c =>
                c.GradoId == dto.GradoId &&
                c.MateriaId == dto.MateriaId &&
                c.AnioAcademico == dto.AnioAcademico, ct))
            throw new BadRequestException(
                $"Ya existe un curso para esta combinación de grado, materia y año académico ({dto.AnioAcademico}).");

        await using var tx = await db.BeginTransactionAsync(ct);
        try
        {
            var curso = Curso.Create(dto.Nombre, dto.GradoId, dto.MateriaId, dto.DocenteId, dto.AnioAcademico);
            _ = await db.Cursos.AddAsync(curso, ct);
            _ = await db.SaveChangesAsync(ct);

            // RF-06: crear los 3 periodos (Corte 1, 2 y 3) automáticamente al crear el curso
            _ = await db.Periodos.AddAsync(Periodo.Create(curso.Id, NumeroCorte.Corte1), ct);
            _ = await db.Periodos.AddAsync(Periodo.Create(curso.Id, NumeroCorte.Corte2), ct);
            _ = await db.Periodos.AddAsync(Periodo.Create(curso.Id, NumeroCorte.Corte3), ct);
            _ = await db.SaveChangesAsync(ct);

            await tx.CommitAsync(ct);
            return await CursoMapper.BuildAsync(db, curso, ct);
        }
        catch
        {
            await tx.RollbackAsync(ct);
            throw;
        }
    }
}
