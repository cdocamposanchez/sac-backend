#region

using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sac.Application.Modules.Materias.Commands.CreateMateria;
using Sac.Application.Modules.Materias.Commands.DeleteMateria;
using Sac.Application.Modules.Materias.Commands.UpdateMateria;
using Sac.Application.Modules.Materias.Queries.GetMateriaById;
using Sac.Application.Modules.Materias.Queries.GetMaterias;
using Sac.Domain.Dtos;

#endregion

namespace Sac.Api.Controllers;

[Route("materias")]
[ApiController]
[Authorize(Policy = "DirectorOrDocente")]
public class MateriaController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    [Authorize(Policy = "DirectorOnly")]
    public async Task<ActionResult<MateriaDto>> Create([FromBody] CreateMateriaDto dto)
        => Ok(await mediator.Send(new CreateMateriaCommand(dto)));

    [HttpPut("{id:long}")]
    [Authorize(Policy = "DirectorOnly")]
    public async Task<ActionResult<MateriaDto>> Update(long id, [FromBody] CreateMateriaDto dto)
        => Ok(await mediator.Send(new UpdateMateriaCommand(id, dto)));

    [HttpDelete("{id:long}")]
    [Authorize(Policy = "DirectorOnly")]
    public async Task<ActionResult<bool>> Delete(long id)
        => Ok(await mediator.Send(new DeleteMateriaCommand(id)));

    [HttpGet]
    public async Task<ActionResult<List<MateriaDto>>> GetAll([FromQuery] bool soloActivas = true)
        => Ok(await mediator.Send(new GetMateriasQuery(soloActivas)));

    [HttpGet("{id:long}")]
    public async Task<ActionResult<MateriaDto>> GetById(long id)
        => Ok(await mediator.Send(new GetMateriaByIdQuery(id)));
}
