#region

using BuildingBlocks.Source.Application.CQRS;

#endregion

namespace Sac.Application.Modules.Actividades.Commands.DeleteActividad;

public record DeleteActividadCommand(long Id) : ICommand<bool>;
