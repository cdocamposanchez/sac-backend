#region

using BuildingBlocks.Source.Application.CQRS;
using Sac.Domain.Dtos;

#endregion

namespace Sac.Application.Modules.Grados.Commands.UpdateGrado;

public record UpdateGradoCommand(long Id, CreateGradoDto Grado) : ICommand<GradoDto>;
