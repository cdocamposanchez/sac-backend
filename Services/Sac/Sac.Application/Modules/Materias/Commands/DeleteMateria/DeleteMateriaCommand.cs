#region

using BuildingBlocks.Source.Application.CQRS;

#endregion

namespace Sac.Application.Modules.Materias.Commands.DeleteMateria;

public record DeleteMateriaCommand(long Id) : ICommand<bool>;
