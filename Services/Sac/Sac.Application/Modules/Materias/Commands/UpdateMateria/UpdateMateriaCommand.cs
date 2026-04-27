#region

using BuildingBlocks.Source.Application.CQRS;
using Sac.Domain.Dtos;

#endregion

namespace Sac.Application.Modules.Materias.Commands.UpdateMateria;

public record UpdateMateriaCommand(long Id, CreateMateriaDto Materia) : ICommand<MateriaDto>;
