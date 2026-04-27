#region

using BuildingBlocks.Source.Application.CQRS;

#endregion

namespace Sac.Application.Modules.Estudiantes.Commands.DeleteEstudiante;

/// <summary>Eliminación lógica del estudiante. No afecta cursos ya cerrados (mantiene historial).</summary>
public record DeleteEstudianteCommand(long Id) : ICommand<bool>;
