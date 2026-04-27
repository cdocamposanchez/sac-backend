#region

using BuildingBlocks.Source.Application.CQRS;
using Sac.Domain.Dtos;

#endregion

namespace Sac.Application.Modules.Periodos.Queries.GetPeriodosPorCurso;

public record GetPeriodosPorCursoQuery(long CursoId) : IQuery<List<PeriodoDto>>;
