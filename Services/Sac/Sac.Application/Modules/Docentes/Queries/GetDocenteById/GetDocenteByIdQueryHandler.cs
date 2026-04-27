#region

using BuildingBlocks.Source.Application.CQRS;
using BuildingBlocks.Source.Domain.Exception;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Sac.Application.Persistance;
using Sac.Domain.Dtos;

#endregion

namespace Sac.Application.Modules.Docentes.Queries.GetDocenteById;

internal sealed class GetDocenteByIdQueryHandler(IApplicationDbContext db)
    : IQueryHandler<GetDocenteByIdQuery, DocenteDto>
{
    public async Task<DocenteDto> Handle(GetDocenteByIdQuery request, CancellationToken ct)
    {
        var docente = await db.Docentes.AsNoTracking()
            .FirstOrDefaultAsync(d => d.Id == request.Id, ct)
            ?? throw new NotFoundException("Docente", request.Id);

        return docente.Adapt<DocenteDto>();
    }
}
