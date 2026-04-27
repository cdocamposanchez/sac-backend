#region

using BuildingBlocks.Source.Application.CQRS;
using Sac.Domain.Dtos;

#endregion

namespace Sac.Application.Modules.Estudiantes.Commands.CreateEstudiante;

/// <summary>HU-03 / RF-02 — El Director registra un Estudiante en el sistema.</summary>
public record CreateEstudianteCommand(CreateEstudianteDto Estudiante) : ICommand<EstudianteDto>;
