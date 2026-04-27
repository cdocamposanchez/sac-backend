#region

using BuildingBlocks.Source.Application.CQRS;

#endregion

namespace Sac.Application.Modules.Grados.Commands.DeleteGrado;

public record DeleteGradoCommand(long Id) : ICommand<bool>;
