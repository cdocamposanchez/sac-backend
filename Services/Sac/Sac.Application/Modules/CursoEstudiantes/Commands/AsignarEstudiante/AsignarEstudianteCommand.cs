#region

using BuildingBlocks.Source.Application.CQRS;
using Sac.Domain.Dtos;

#endregion

namespace Sac.Application.Modules.CursoEstudiantes.Commands.AsignarEstudiante;

/// <summary>HU-07 / RF-05 — El Director asigna un estudiante a un curso.</summary>
public record AsignarEstudianteCommand(AsignarEstudianteDto Datos) : ICommand<CursoEstudianteDto>;
