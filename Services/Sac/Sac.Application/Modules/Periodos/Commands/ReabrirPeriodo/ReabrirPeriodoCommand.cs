#region

using BuildingBlocks.Source.Application.CQRS;
using Sac.Domain.Dtos;

#endregion

namespace Sac.Application.Modules.Periodos.Commands.ReabrirPeriodo;

/// <summary>RF-06 — Reabrir un periodo cerrado por autorización explícita del Director.</summary>
public record ReabrirPeriodoCommand(long Id) : ICommand<PeriodoDto>;
