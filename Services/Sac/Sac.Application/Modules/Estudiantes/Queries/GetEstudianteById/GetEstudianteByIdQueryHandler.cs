#region

using BuildingBlocks.Source.Application.CQRS;
using BuildingBlocks.Source.Domain.Exception;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Sac.Application.Persistance;
using Sac.Domain.Dtos;

#endregion

namespace Sac.Application.Modules.Estudiantes.Queries.GetEstudianteById;

internal sealed class GetEstudianteByIdQueryHandler(IApplicationDbContext db)
    : IQueryHandler<GetEstudianteByIdQuery, EstudianteDto>
{
    public async Task<EstudianteDto> Handle(GetEstudianteByIdQuery request, CancellationToken ct)
    {
        var estudiante = await db.Estudiantes.AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == request.Id, ct)
            ?? throw new NotFoundException("Estudiante", request.Id);

        return estudiante.Adapt<EstudianteDto>();
    }
}
