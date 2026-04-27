#region

using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sac.Application.Modules.Auth.Commands.Login;
using Sac.Domain.Dtos;

#endregion

namespace Sac.Api.Controllers;

[Route("auth")]
[ApiController]
[AllowAnonymous]
public class AuthController(IMediator mediator) : ControllerBase
{
    /// <summary>HU-01 / RF-01 — Inicio de sesión por cédula y contraseña.</summary>
    [HttpPost("login")]
    [ProducesResponseType(typeof(LoginResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<LoginResponseDto>> Login([FromBody] LoginRequestDto credentials)
    {
        var result = await mediator.Send(new LoginCommand(credentials));
        return Ok(result);
    }
}
