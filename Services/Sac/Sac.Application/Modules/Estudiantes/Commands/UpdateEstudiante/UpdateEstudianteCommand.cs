#region

using BuildingBlocks.Source.Application.CQRS;
using Sac.Domain.Dtos;

#endregion

namespace Sac.Application.Modules.Estudiantes.Commands.UpdateEstudiante;

public record UpdateEstudianteCommand(long Id, UpdateEstudianteDto Estudiante) : ICommand<EstudianteDto>;
