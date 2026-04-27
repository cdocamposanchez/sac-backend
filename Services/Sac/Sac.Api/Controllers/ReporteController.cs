#region

using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sac.Application.Modules.Reportes.Queries.GetReporteCurso;
using Sac.Application.Modules.Reportes.Queries.GetReporteEstudiante;
using Sac.Application.Modules.Reportes.Queries.GetReporteFinalCurso;
using Sac.Domain.Dtos;

#endregion

namespace Sac.Api.Controllers;

[Route("reportes")]
[ApiController]
[Authorize(Policy = "DirectorOrDocente")]
public class ReporteController(IMediator mediator) : ControllerBase
{
    /// <summary>RF-10 — Reporte de un estudiante específico en un curso.</summary>
    [HttpGet("curso/{cursoId:long}/estudiante/{estudianteId:long}")]
    public async Task<ActionResult<ReporteEstudianteDto>> GetReporteEstudiante(long cursoId, long estudianteId)
        => Ok(await mediator.Send(new GetReporteEstudianteQuery(cursoId, estudianteId)));

    /// <summary>RF-10 — Reporte de todos los estudiantes de un curso.</summary>
    [HttpGet("curso/{cursoId:long}")]
    public async Task<ActionResult<List<ReporteEstudianteDto>>> GetReporteCurso(long cursoId)
        => Ok(await mediator.Send(new GetReporteCursoQuery(cursoId)));

    /// <summary>RF-10 — Reporte FINAL del curso (solo si está cerrado).</summary>
    [HttpGet("curso/{cursoId:long}/final")]
    public async Task<ActionResult<List<ReporteEstudianteDto>>> GetReporteFinal(long cursoId)
        => Ok(await mediator.Send(new GetReporteFinalCursoQuery(cursoId)));
}
