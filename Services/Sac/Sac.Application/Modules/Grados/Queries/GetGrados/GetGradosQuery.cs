#region

using BuildingBlocks.Source.Application.CQRS;
using Sac.Domain.Dtos;

#endregion

namespace Sac.Application.Modules.Grados.Queries.GetGrados;

public record GetGradosQuery(bool SoloActivos = true) : IQuery<List<GradoDto>>;
