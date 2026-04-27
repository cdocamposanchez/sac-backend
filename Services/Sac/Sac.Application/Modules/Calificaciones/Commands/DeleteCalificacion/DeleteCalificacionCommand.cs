#region

using BuildingBlocks.Source.Application.CQRS;

#endregion

namespace Sac.Application.Modules.Calificaciones.Commands.DeleteCalificacion;

public record DeleteCalificacionCommand(long Id) : ICommand<bool>;
