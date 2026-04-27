#region

using BuildingBlocks.Source.Application.CQRS;
using Sac.Domain.Dtos;

#endregion

namespace Sac.Application.Modules.Calificaciones.Commands.CreateCalificacion;

/// <summary>
/// HU-09 / RF-08 — El Docente registra una calificación (rango 1.0 - 5.0).
/// El sistema valida nota, porcentaje, suma de porcentajes ≤100%, periodo abierto,
/// y recalcula el promedio del periodo (RF-09).
/// </summary>
public record CreateCalificacionCommand(CreateCalificacionDto Calificacion) : ICommand<CalificacionDto>;
