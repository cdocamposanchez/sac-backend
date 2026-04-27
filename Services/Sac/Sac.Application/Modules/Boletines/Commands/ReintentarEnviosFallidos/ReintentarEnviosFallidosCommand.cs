#region

using BuildingBlocks.Source.Application.CQRS;
using Sac.Domain.Dtos;

#endregion

namespace Sac.Application.Modules.Boletines.Commands.ReintentarEnviosFallidos;

/// <summary>RF-11 — Reintenta los envíos fallidos de un curso.</summary>
public record ReintentarEnviosFallidosCommand(long CursoId) : ICommand<EnviarBoletinesResultadoDto>;
