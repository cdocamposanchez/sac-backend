#region

using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sac.Application.Modules.Grados.Commands.CreateGrado;
using Sac.Application.Modules.Grados.Commands.DeleteGrado;
using Sac.Application.Modules.Grados.Commands.UpdateGrado;
using Sac.Application.Modules.Grados.Queries.GetGradoById;
using Sac.Application.Modules.Grados.Queries.GetGrados;
using Sac.Domain.Dtos;

#endregion

namespace Sac.Api.Controllers;

[Route("grados")]
[ApiController]
[Authorize(Policy = "DirectorOrDocente")]
public class GradoController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    [Authorize(Policy = "DirectorOnly")]
    public async Task<ActionResult<GradoDto>> Create([FromBody] CreateGradoDto dto)
        => Ok(await mediator.Send(new CreateGradoCommand(dto)));

    [HttpPut("{id:long}")]
    [Authorize(Policy = "DirectorOnly")]
    public async Task<ActionResult<GradoDto>> Update(long id, [FromBody] CreateGradoDto dto)
        => Ok(await mediator.Send(new UpdateGradoCommand(id, dto)));

    [HttpDelete("{id:long}")]
    [Authorize(Policy = "DirectorOnly")]
    public async Task<ActionResult<bool>> Delete(long id)
        => Ok(await mediator.Send(new DeleteGradoCommand(id)));

    [HttpGet]
    public async Task<ActionResult<List<GradoDto>>> GetAll([FromQuery] bool soloActivos = true)
        => Ok(await mediator.Send(new GetGradosQuery(soloActivos)));

    [HttpGet("{id:long}")]
    public async Task<ActionResult<GradoDto>> GetById(long id)
        => Ok(await mediator.Send(new GetGradoByIdQuery(id)));
}
