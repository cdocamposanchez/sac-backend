#region

using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sac.Application.Modules.Periodos.Commands.CerrarPeriodo;
using Sac.Application.Modules.Periodos.Commands.ReabrirPeriodo;
using Sac.Application.Modules.Periodos.Queries.GetPeriodoById;
using Sac.Application.Modules.Periodos.Queries.GetPeriodosPorCurso;
using Sac.Domain.Dtos;

#endregion

namespace Sac.Api.Controllers;

[Route("periodos")]
[ApiController]
[Authorize(Policy = "DirectorOrDocente")]
public class PeriodoController(IMediator mediator) : ControllerBase
{
    /// <summary>HU-10 / RF-06 — Cerrar periodo (solo Director).</summary>
    [HttpPost("{id:long}/cerrar")]
    [Authorize(Policy = "DirectorOnly")]
    public async Task<ActionResult<PeriodoDto>> Cerrar(long id)
        => Ok(await mediator.Send(new CerrarPeriodoCommand(id)));

    /// <summary>RF-06 — Reabrir periodo (autorización explícita del Director).</summary>
    [HttpPost("{id:long}/reabrir")]
    [Authorize(Policy = "DirectorOnly")]
    public async Task<ActionResult<PeriodoDto>> Reabrir(long id)
        => Ok(await mediator.Send(new ReabrirPeriodoCommand(id)));

    [HttpGet("curso/{cursoId:long}")]
    public async Task<ActionResult<List<PeriodoDto>>> GetPorCurso(long cursoId)
        => Ok(await mediator.Send(new GetPeriodosPorCursoQuery(cursoId)));

    [HttpGet("{id:long}")]
    public async Task<ActionResult<PeriodoDto>> GetById(long id)
        => Ok(await mediator.Send(new GetPeriodoByIdQuery(id)));
}
