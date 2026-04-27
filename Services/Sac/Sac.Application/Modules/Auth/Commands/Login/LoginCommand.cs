#region

using BuildingBlocks.Source.Application.CQRS;
using Sac.Domain.Dtos;

#endregion

namespace Sac.Application.Modules.Auth.Commands.Login;

/// <summary>
/// HU-01 / RF-01 — Inicio de sesión por cédula y contraseña.
/// Autentica un Director o Docente y devuelve un JWT firmado con su rol como claim.
/// </summary>
public record LoginCommand(LoginRequestDto Credentials) : ICommand<LoginResponseDto>;
