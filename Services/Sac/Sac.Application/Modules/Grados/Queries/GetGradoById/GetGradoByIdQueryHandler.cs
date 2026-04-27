#region

using BuildingBlocks.Source.Application.CQRS;
using BuildingBlocks.Source.Domain.Exception;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Sac.Application.Persistance;
using Sac.Domain.Dtos;

#endregion

namespace Sac.Application.Modules.Grados.Queries.GetGradoById;

internal sealed class GetGradoByIdQueryHandler(IApplicationDbContext db)
    : IQueryHandler<GetGradoByIdQuery, GradoDto>
{
    public async Task<GradoDto> Handle(GetGradoByIdQuery request, CancellationToken ct)
    {
        var grado = await db.Grados.AsNoTracking()
            .FirstOrDefaultAsync(g => g.Id == request.Id, ct)
            ?? throw new NotFoundException("Grado", request.Id);

        return grado.Adapt<GradoDto>();
    }
}
