#region

using BuildingBlocks.Source.Application.CQRS;
using BuildingBlocks.Source.Application.Dtos.Pagination;
using BuildingBlocks.Source.Infrastructure.Security;
using Microsoft.EntityFrameworkCore;
using Sac.Application.Modules.Cursos._Shared;
using Sac.Application.Persistance;
using Sac.Domain.Dtos;
using Sac.Domain.Models;

#endregion

namespace Sac.Application.Modules.Cursos.Queries.GetCursos;

internal sealed class GetCursosQueryHandler(
    IApplicationDbContext db,
    ICurrentUserService currentUser) : IQueryHandler<GetCursosQuery, PaginatedResult<CursoDto>>
{
    public async Task<PaginatedResult<CursoDto>> Handle(GetCursosQuery request, CancellationToken ct)
    {
        IQueryable<Curso> baseQuery = db.Cursos.AsNoTracking();

        // RF-10: Docente solo ve sus cursos. Director ve todos.
        if (currentUser.IsDocente && currentUser.UserId.HasValue)
            baseQuery = baseQuery.Where(c => c.DocenteId == currentUser.UserId.Value);

        var total = await baseQuery.LongCountAsync(ct);

        var cursos = await baseQuery
            .OrderByDescending(c => c.AnioAcademico).ThenBy(c => c.Nombre)
            .Skip(request.Pagination.Skip)
            .Take(request.Pagination.Take)
            .ToListAsync(ct);

        var data = new List<CursoDto>(cursos.Count);
        foreach (var c in cursos)
            data.Add(await CursoMapper.BuildAsync(db, c, ct));

        return new PaginatedResult<CursoDto>(
            request.Pagination.PageIndex, request.Pagination.Take, total, data);
    }
}
