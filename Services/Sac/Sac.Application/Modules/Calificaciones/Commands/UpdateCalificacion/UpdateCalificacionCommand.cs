#region

using BuildingBlocks.Source.Application.CQRS;
using Sac.Domain.Dtos;

#endregion

namespace Sac.Application.Modules.Calificaciones.Commands.UpdateCalificacion;

public record UpdateCalificacionCommand(long Id, UpdateCalificacionDto Calificacion) : ICommand<CalificacionDto>;
