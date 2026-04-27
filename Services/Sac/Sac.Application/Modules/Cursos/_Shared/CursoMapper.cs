#region

using Microsoft.EntityFrameworkCore;
using Sac.Application.Persistance;
using Sac.Domain.Dtos;
using Sac.Domain.Models;

#endregion

namespace Sac.Application.Modules.Cursos._Shared;

/// <summary>
/// Helper interno del módulo Cursos. Centraliza la construcción del CursoDto
/// (que requiere joins con Grado, Materia y Docente).
/// </summary>
internal static class CursoMapper
{
    public static async Task<CursoDto> BuildAsync(IApplicationDbContext db, Curso curso, CancellationToken ct)
    {
        var grado = await db.Grados.AsNoTracking().FirstAsync(g => g.Id == curso.GradoId, ct);
        var materia = await db.Materias.AsNoTracking().FirstAsync(m => m.Id == curso.MateriaId, ct);
        var docente = await db.Docentes.AsNoTracking().FirstAsync(d => d.Id == curso.DocenteId, ct);

        return new CursoDto(
            curso.Id, curso.Nombre,
            grado.Id, grado.Nombre,
            materia.Id, materia.Nombre,
            docente.Id, docente.NombreCompleto,
            curso.AnioAcademico,
            curso.Estado.ToString());
    }
}
