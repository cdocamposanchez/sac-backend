#region

using BuildingBlocks.Source.Application.CQRS;

#endregion

namespace Sac.Application.Modules.Docentes.Commands.DeleteDocente;

/// <summary>
/// HU-02 / RF-02 — Inactivar un Docente (eliminación lógica).
/// No se permite eliminar si el docente tiene cursos activos asignados.
/// </summary>
public record DeleteDocenteCommand(long Id) : ICommand<bool>;
