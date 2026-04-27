#region

using BuildingBlocks.Source.Application.CQRS;
using Sac.Domain.Dtos;

#endregion

namespace Sac.Application.Modules.Docentes.Commands.UpdateDocente;

/// <summary>HU-02 / RF-02 — El Director actualiza nombre y correo de un Docente.</summary>
public record UpdateDocenteCommand(long Id, UpdateDocenteDto Docente) : ICommand<DocenteDto>;
