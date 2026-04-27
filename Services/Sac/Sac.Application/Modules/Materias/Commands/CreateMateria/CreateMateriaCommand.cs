#region

using BuildingBlocks.Source.Application.CQRS;
using Sac.Domain.Dtos;

#endregion

namespace Sac.Application.Modules.Materias.Commands.CreateMateria;

/// <summary>HU-05 / RF-03 — El Director crea una Materia (ej: Matemáticas, Español).</summary>
public record CreateMateriaCommand(CreateMateriaDto Materia) : ICommand<MateriaDto>;
