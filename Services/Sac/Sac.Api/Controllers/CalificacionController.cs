#region

using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sac.Application.Modules.Calificaciones.Commands.CreateCalificacion;
using Sac.Application.Modules.Calificaciones.Commands.DeleteCalificacion;
using Sac.Application.Modules.Calificaciones.Commands.UpdateCalificacion;
using Sac.Application.Modules.Calificaciones.Queries.GetCalificacionById;
using Sac.Application.Modules.Calificaciones.Queries.GetCalificaciones;
using Sac.Domain.Dtos;

#endregion

namespace Sac.Api.Controllers;

[Route("calificaciones")]
[ApiController]
[Authorize(Policy = "DirectorOrDocente")]
public class CalificacionController(IMediator mediator) : ControllerBase
{
    /// <summary>HU-09 / RF-08 — Registrar nota (rango 1.0 - 5.0, periodo abierto).</summary>
    [HttpPost]
    public async Task<ActionResult<CalificacionDto>> Create([FromBody] CreateCalificacionDto dto)
        => Ok(await mediator.Send(new CreateCalificacionCommand(dto)));

    /// <summary>HU-09 / RF-08 — Editar calificación (solo si el periodo está abierto).</summary>
    [HttpPut("{id:long}")]
    public async Task<ActionResult<CalificacionDto>> Update(long id, [FromBody] UpdateCalificacionDto dto)
        => Ok(await mediator.Send(new UpdateCalificacionCommand(id, dto)));

    /// <summary>HU-09 / RF-08 — Eliminar calificación (solo si el periodo está abierto).</summary>
    [HttpDelete("{id:long}")]
    public async Task<ActionResult<bool>> Delete(long id)
        => Ok(await mediator.Send(new DeleteCalificacionCommand(id)));

    /// <summary>RF-10 — Listar calificaciones con filtros opcionales.</summary>
    [HttpGet]
    public async Task<ActionResult<List<CalificacionDto>>> GetAll(
        [FromQuery] long? cursoId = null,
        [FromQuery] long? periodoId = null,
        [FromQuery] long? actividadId = null,
        [FromQuery] long? estudianteId = null)
        => Ok(await mediator.Send(new GetCalificacionesQuery(cursoId, periodoId, actividadId, estudianteId)));

    [HttpGet("{id:long}")]
    public async Task<ActionResult<CalificacionDto>> GetById(long id)
        => Ok(await mediator.Send(new GetCalificacionByIdQuery(id)));
}
