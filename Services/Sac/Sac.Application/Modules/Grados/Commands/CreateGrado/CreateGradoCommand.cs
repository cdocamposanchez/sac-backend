#region

using BuildingBlocks.Source.Application.CQRS;
using Sac.Domain.Dtos;

#endregion

namespace Sac.Application.Modules.Grados.Commands.CreateGrado;

/// <summary>HU-04 / RF-03 — El Director crea un Grado en el catálogo (ej: 6-1, 7-2).</summary>
public record CreateGradoCommand(CreateGradoDto Grado) : ICommand<GradoDto>;
