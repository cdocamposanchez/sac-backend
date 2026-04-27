#region

using BuildingBlocks.Source.Application.CQRS;
using Sac.Domain.Dtos;

#endregion

namespace Sac.Application.Modules.Actividades.Commands.UpdateActividad;

public record UpdateActividadCommand(long Id, UpdateActividadDto Actividad) : ICommand<ActividadDto>;
