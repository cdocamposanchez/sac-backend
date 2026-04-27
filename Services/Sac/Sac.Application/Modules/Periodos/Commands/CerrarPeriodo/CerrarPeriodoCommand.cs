#region

using BuildingBlocks.Source.Application.CQRS;
using Sac.Domain.Dtos;

#endregion

namespace Sac.Application.Modules.Periodos.Commands.CerrarPeriodo;

/// <summary>
/// HU-10 / RF-06 — El Director cierra un Periodo Académico.
/// Al cerrarlo, se recalcula el promedio agregado y se bloquean modificaciones de notas.
/// </summary>
public record CerrarPeriodoCommand(long Id) : ICommand<PeriodoDto>;
