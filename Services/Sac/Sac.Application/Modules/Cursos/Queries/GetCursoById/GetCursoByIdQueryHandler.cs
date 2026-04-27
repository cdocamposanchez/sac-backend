#region

using BuildingBlocks.Source.Application.CQRS;
using BuildingBlocks.Source.Domain.Exception;
using BuildingBlocks.Source.Infrastructure.Security;
using Microsoft.EntityFrameworkCore;
using Sac.Application.Modules.Cursos._Shared;
using Sac.Application.Persistance;
using Sac.Domain.Dtos;

#endregion

namespace Sac.Application.Modules.Cursos.Queries.GetCursoById;

internal sealed class GetCursoByIdQueryHandler(
    IApplicationDbContext db,
    ICurrentUserService currentUser) : IQueryHandler<GetCursoByIdQuery, CursoDto>
{
    public async Task<CursoDto> Handle(GetCursoByIdQuery request, CancellationToken ct)
    {
        var curso = await db.Cursos.AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == request.Id, ct)
            ?? throw new NotFoundException("Curso", request.Id);

        // RF-10: el docente solo puede consultar cursos suyos
        if (currentUser.IsDocente && currentUser.UserId.HasValue && !curso.PerteneceADocente(currentUser.UserId.Value))
            throw new UnauthorizedException("No tiene permisos para acceder a este curso.");

        return await CursoMapper.BuildAsync(db, curso, ct);
    }
}
