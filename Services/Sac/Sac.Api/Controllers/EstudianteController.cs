#region

using BuildingBlocks.Source.Application.Dtos.Pagination;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sac.Application.Modules.Estudiantes.Commands.CreateEstudiante;
using Sac.Application.Modules.Estudiantes.Commands.CreateEstudiantesLote;
using Sac.Application.Modules.Estudiantes.Commands.DeleteEstudiante;
using Sac.Application.Modules.Estudiantes.Commands.UpdateEstudiante;
using Sac.Application.Modules.Estudiantes.Queries.GetEstudianteById;
using Sac.Application.Modules.Estudiantes.Queries.GetEstudiantes;
using Sac.Domain.Dtos;

#endregion

namespace Sac.Api.Controllers;

[Route("estudiantes")]
[ApiController]
[Authorize(Policy = "DirectorOnly")]
public class EstudianteController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<EstudianteDto>> Create([FromBody] CreateEstudianteDto dto)
        => Ok(await mediator.Send(new CreateEstudianteCommand(dto)));

    /// <summary>RF-02 — Carga en lote de estudiantes (transaccional).</summary>
    [HttpPost("lote")]
    public async Task<ActionResult<List<EstudianteDto>>> CreateLote([FromBody] List<CreateEstudianteDto> dtos)
        => Ok(await mediator.Send(new CreateEstudiantesLoteCommand(dtos)));

    [HttpPut("{id:long}")]
    public async Task<ActionResult<EstudianteDto>> Update(long id, [FromBody] UpdateEstudianteDto dto)
        => Ok(await mediator.Send(new UpdateEstudianteCommand(id, dto)));

    [HttpDelete("{id:long}")]
    public async Task<ActionResult<bool>> Delete(long id)
        => Ok(await mediator.Send(new DeleteEstudianteCommand(id)));

    [HttpGet]
    [Authorize(Policy = "DirectorOrDocente")]
    public async Task<ActionResult<PaginatedResult<EstudianteDto>>> GetAll(
        [FromQuery] PaginationRequest pagination)
        => Ok(await mediator.Send(new GetEstudiantesQuery(pagination)));

    [HttpGet("{id:long}")]
    [Authorize(Policy = "DirectorOrDocente")]
    public async Task<ActionResult<EstudianteDto>> GetById(long id)
        => Ok(await mediator.Send(new GetEstudianteByIdQuery(id)));
}
