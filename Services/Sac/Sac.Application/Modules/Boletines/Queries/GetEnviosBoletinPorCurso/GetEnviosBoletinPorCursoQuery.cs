#region

using BuildingBlocks.Source.Application.CQRS;
using Sac.Domain.Dtos;

#endregion

namespace Sac.Application.Modules.Boletines.Queries.GetEnviosBoletinPorCurso;

public record GetEnviosBoletinPorCursoQuery(long CursoId) : IQuery<List<EnvioBoletinDto>>;
