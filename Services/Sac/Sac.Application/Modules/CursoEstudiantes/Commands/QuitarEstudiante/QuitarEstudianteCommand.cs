#region

using BuildingBlocks.Source.Application.CQRS;

#endregion

namespace Sac.Application.Modules.CursoEstudiantes.Commands.QuitarEstudiante;

public record QuitarEstudianteCommand(long CursoId, long EstudianteId) : ICommand<bool>;
