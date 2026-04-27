#region

using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sac.Application.Modules.Actividades.Commands.CreateActividad;
using Sac.Application.Modules.Actividades.Commands.DeleteActividad;
using Sac.Application.Modules.Actividades.Commands.UpdateActividad;
using Sac.Application.Modules.Actividades.Queries.GetActividadById;
using Sac.Application.Modules.Actividades.Queries.GetActividades;
using Sac.Domain.Dtos;

#endregion

namespace Sac.Api.Controllers;

[Route("actividades")]
[ApiController]
[Authorize(Policy = "DirectorOrDocente")]
public class ActividadController(IMediator mediator) : ControllerBase
{
    /// <summary>HU-08 / RF-07 — Crear actividad (Docente del curso, periodo abierto).</summary>
    [HttpPost]
    public async Task<ActionResult<ActividadDto>> Create([FromBody] CreateActividadDto dto)
        => Ok(await mediator.Send(new CreateActividadCommand(dto)));

    [HttpPut("{id:long}")]
    public async Task<ActionResult<ActividadDto>> Update(long id, [FromBody] UpdateActividadDto dto)
        => Ok(await mediator.Send(new UpdateActividadCommand(id, dto)));

    [HttpDelete("{id:long}")]
    public async Task<ActionResult<bool>> Delete(long id)
        => Ok(await mediator.Send(new DeleteActividadCommand(id)));

    [HttpGet]
    public async Task<ActionResult<List<ActividadDto>>> GetAll(
        [FromQuery] long cursoId, [FromQuery] long? periodoId = null)
        => Ok(await mediator.Send(new GetActividadesQuery(cursoId, periodoId)));

    [HttpGet("{id:long}")]
    public async Task<ActionResult<ActividadDto>> GetById(long id)
        => Ok(await mediator.Send(new GetActividadByIdQuery(id)));
}
