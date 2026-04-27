#region

using BuildingBlocks.Source.Application.CQRS;
using Sac.Domain.Dtos;

#endregion

namespace Sac.Application.Modules.Cursos.Commands.CerrarCurso;

/// <summary>
/// HU-10 / RF-04 — El Director cierra un Curso.
/// La validación de los 3 periodos cerrados está en el agregado <c>Curso.Cerrar()</c>.
/// Una vez cerrado, no se permiten modificaciones de actividades ni notas.
/// </summary>
public record CerrarCursoCommand(long Id) : ICommand<CursoDto>;
