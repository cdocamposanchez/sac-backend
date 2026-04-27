#region

using BuildingBlocks.Source.Application.CQRS;
using Sac.Domain.Dtos;

#endregion

namespace Sac.Application.Modules.Actividades.Queries.GetActividades;

public record GetActividadesQuery(long CursoId, long? PeriodoId = null) : IQuery<List<ActividadDto>>;
