#region

using BuildingBlocks.Source.Application.Dtos.Pagination;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sac.Application.Modules.CursoEstudiantes.Commands.AsignarEstudiante;
using Sac.Application.Modules.CursoEstudiantes.Commands.AsignarEstudiantesLote;
using Sac.Application.Modules.CursoEstudiantes.Commands.QuitarEstudiante;
using Sac.Application.Modules.CursoEstudiantes.Queries.GetEstudiantesPorCurso;
using Sac.Application.Modules.Cursos.Commands.CerrarCurso;
using Sac.Application.Modules.Cursos.Commands.CreateCurso;
using Sac.Application.Modules.Cursos.Commands.UpdateCurso;
using Sac.Application.Modules.Cursos.Queries.GetCursoById;
using Sac.Application.Modules.Cursos.Queries.GetCursos;
using Sac.Domain.Dtos;

#endregion

namespace Sac.Api.Controllers;

[Route("cursos")]
[ApiController]
[Authorize(Policy = "DirectorOrDocente")]
public class CursoController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    [Authorize(Policy = "DirectorOnly")]
    public async Task<ActionResult<CursoDto>> Create([FromBody] CreateCursoDto dto)
        => Ok(await mediator.Send(new CreateCursoCommand(dto)));

    [HttpPut("{id:long}")]
    [Authorize(Policy = "DirectorOnly")]
    public async Task<ActionResult<CursoDto>> Update(long id, [FromBody] UpdateCursoDto dto)
        => Ok(await mediator.Send(new UpdateCursoCommand(id, dto)));

    /// <summary>HU-10 / RF-04 — Cierra el curso (requiere los 3 periodos cerrados).</summary>
    [HttpPost("{id:long}/cerrar")]
    [Authorize(Policy = "DirectorOnly")]
    public async Task<ActionResult<CursoDto>> Cerrar(long id)
        => Ok(await mediator.Send(new CerrarCursoCommand(id)));

    [HttpGet]
    public async Task<ActionResult<PaginatedResult<CursoDto>>> GetAll(
        [FromQuery] PaginationRequest pagination)
        => Ok(await mediator.Send(new GetCursosQuery(pagination)));

    [HttpGet("{id:long}")]
    public async Task<ActionResult<CursoDto>> GetById(long id)
        => Ok(await mediator.Send(new GetCursoByIdQuery(id)));

    // ===== Asignación de estudiantes (HU-07 / RF-05) =====

    [HttpPost("{id:long}/estudiantes")]
    [Authorize(Policy = "DirectorOnly")]
    public async Task<ActionResult<CursoEstudianteDto>> AsignarEstudiante(
        long id, [FromBody] AsignarEstudianteRequest request)
        => Ok(await mediator.Send(new AsignarEstudianteCommand(
            new AsignarEstudianteDto(id, request.EstudianteId))));

    [HttpPost("{id:long}/estudiantes/lote")]
    [Authorize(Policy = "DirectorOnly")]
    public async Task<ActionResult<List<CursoEstudianteDto>>> AsignarEstudiantesLote(
        long id, [FromBody] AsignarLoteRequest request)
        => Ok(await mediator.Send(new AsignarEstudiantesLoteCommand(
            new AsignarEstudiantesLoteDto(id, request.EstudianteIds))));

    [HttpDelete("{id:long}/estudiantes/{estudianteId:long}")]
    [Authorize(Policy = "DirectorOnly")]
    public async Task<ActionResult<bool>> QuitarEstudiante(long id, long estudianteId)
        => Ok(await mediator.Send(new QuitarEstudianteCommand(id, estudianteId)));

    [HttpGet("{id:long}/estudiantes")]
    public async Task<ActionResult<List<CursoEstudianteDto>>> GetEstudiantesCurso(long id)
        => Ok(await mediator.Send(new GetEstudiantesPorCursoQuery(id)));
}

public record AsignarEstudianteRequest(long EstudianteId);
public record AsignarLoteRequest(List<long> EstudianteIds);
