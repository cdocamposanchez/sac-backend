#region

using BuildingBlocks.Source.Application.CQRS;
using Sac.Domain.Dtos;

#endregion

namespace Sac.Application.Modules.CursoEstudiantes.Queries.GetEstudiantesPorCurso;

public record GetEstudiantesPorCursoQuery(long CursoId) : IQuery<List<CursoEstudianteDto>>;
