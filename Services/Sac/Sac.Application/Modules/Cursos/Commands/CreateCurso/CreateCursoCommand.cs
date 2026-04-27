#region

using BuildingBlocks.Source.Application.CQRS;
using Sac.Domain.Dtos;

#endregion

namespace Sac.Application.Modules.Cursos.Commands.CreateCurso;

/// <summary>
/// HU-06 / RF-04 — El Director crea un Curso combinando Grado + Materia + Docente.
/// Al crearse, el sistema genera automáticamente los 3 periodos (Corte 1, 2 y 3).
/// </summary>
public record CreateCursoCommand(CreateCursoDto Curso) : ICommand<CursoDto>;
