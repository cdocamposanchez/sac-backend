#region

using BuildingBlocks.Source.Application.Dtos.Pagination;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sac.Application.Modules.Docentes.Commands.CreateDocente;
using Sac.Application.Modules.Docentes.Commands.DeleteDocente;
using Sac.Application.Modules.Docentes.Commands.UpdateDocente;
using Sac.Application.Modules.Docentes.Queries.GetDocenteById;
using Sac.Application.Modules.Docentes.Queries.GetDocentes;
using Sac.Domain.Dtos;

#endregion

namespace Sac.Api.Controllers;

[Route("docentes")]
[ApiController]
[Authorize(Policy = "DirectorOnly")]
public class DocenteController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<DocenteDto>> Create([FromBody] CreateDocenteDto dto)
        => Ok(await mediator.Send(new CreateDocenteCommand(dto)));

    [HttpPut("{id:long}")]
    public async Task<ActionResult<DocenteDto>> Update(long id, [FromBody] UpdateDocenteDto dto)
        => Ok(await mediator.Send(new UpdateDocenteCommand(id, dto)));

    [HttpDelete("{id:long}")]
    public async Task<ActionResult<bool>> Delete(long id)
        => Ok(await mediator.Send(new DeleteDocenteCommand(id)));

    [HttpGet]
    [Authorize(Policy = "DirectorOrDocente")]
    public async Task<ActionResult<PaginatedResult<DocenteDto>>> GetAll(
        [FromQuery] PaginationRequest pagination)
        => Ok(await mediator.Send(new GetDocentesQuery(pagination)));

    [HttpGet("{id:long}")]
    [Authorize(Policy = "DirectorOrDocente")]
    public async Task<ActionResult<DocenteDto>> GetById(long id)
        => Ok(await mediator.Send(new GetDocenteByIdQuery(id)));
}
