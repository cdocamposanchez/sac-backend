#region

using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sac.Application.Modules.Boletines.Commands.EnviarBoletines;
using Sac.Application.Modules.Boletines.Commands.ReintentarEnviosFallidos;
using Sac.Application.Modules.Boletines.Queries.GetEnviosBoletinPorCurso;
using Sac.Domain.Dtos;

#endregion

namespace Sac.Api.Controllers;

[Route("boletines")]
[ApiController]
[Authorize(Policy = "DirectorOnly")]
public class BoletinController(IMediator mediator) : ControllerBase
{
    /// <summary>HU-11 / RF-11 — Envía boletines a todos los estudiantes (curso CERRADO).</summary>
    [HttpPost("curso/{cursoId:long}/enviar")]
    public async Task<ActionResult<EnviarBoletinesResultadoDto>> Enviar(long cursoId)
        => Ok(await mediator.Send(new EnviarBoletinesCommand(cursoId)));

    /// <summary>RF-11 — Reintenta los envíos fallidos del curso.</summary>
    [HttpPost("curso/{cursoId:long}/reintentar")]
    public async Task<ActionResult<EnviarBoletinesResultadoDto>> Reintentar(long cursoId)
        => Ok(await mediator.Send(new ReintentarEnviosFallidosCommand(cursoId)));

    /// <summary>Histórico de envíos del curso.</summary>
    [HttpGet("curso/{cursoId:long}")]
    public async Task<ActionResult<List<EnvioBoletinDto>>> GetEnviosCurso(long cursoId)
        => Ok(await mediator.Send(new GetEnviosBoletinPorCursoQuery(cursoId)));
}
