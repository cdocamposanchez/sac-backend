#region

using BuildingBlocks.Source.Application.CQRS;
using Sac.Domain.Dtos;

#endregion

namespace Sac.Application.Modules.Actividades.Commands.CreateActividad;

/// <summary>
/// HU-08 / RF-07 — El Docente crea una Actividad en uno de sus cursos.
/// Solo permitido si el periodo está abierto y el docente es titular del curso.
/// </summary>
public record CreateActividadCommand(CreateActividadDto Actividad) : ICommand<ActividadDto>;
