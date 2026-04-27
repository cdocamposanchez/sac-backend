#region

using BuildingBlocks.Source.Application.CQRS;
using Sac.Domain.Dtos;

#endregion

namespace Sac.Application.Modules.Docentes.Commands.CreateDocente;

/// <summary>HU-02 / RF-02 — El Director crea un nuevo Docente.</summary>
public record CreateDocenteCommand(CreateDocenteDto Docente) : ICommand<DocenteDto>;
