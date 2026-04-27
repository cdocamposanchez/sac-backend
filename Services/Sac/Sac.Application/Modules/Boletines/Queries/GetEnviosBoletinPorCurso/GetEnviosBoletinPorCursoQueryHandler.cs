#region

using BuildingBlocks.Source.Application.CQRS;
using BuildingBlocks.Source.Domain.Exception;
using Microsoft.EntityFrameworkCore;
using Sac.Application.Persistance;
using Sac.Domain.Dtos;

#endregion

namespace Sac.Application.Modules.Boletines.Queries.GetEnviosBoletinPorCurso;

internal sealed class GetEnviosBoletinPorCursoQueryHandler(IApplicationDbContext db)
    : IQueryHandler<GetEnviosBoletinPorCursoQuery, List<EnvioBoletinDto>>
{
    public async Task<List<EnvioBoletinDto>> Handle(GetEnviosBoletinPorCursoQuery request, CancellationToken ct)
    {
        if (!await db.Cursos.AnyAsync(c => c.Id == request.CursoId, ct))
            throw new NotFoundException("Curso", request.CursoId);

        var envios = await db.EnviosBoletin.AsNoTracking()
            .Where(e => e.CursoId == request.CursoId)
            .OrderByDescending(e => e.FechaReg)
            .ToListAsync(ct);

        return envios.Select(e => new EnvioBoletinDto(
            e.Id, e.CursoId, e.EstudianteId, e.CorreoDestino,
            e.Estado.ToString(), e.FechaEnvio, e.MotivoFallo, e.Intentos)).ToList();
    }
}
