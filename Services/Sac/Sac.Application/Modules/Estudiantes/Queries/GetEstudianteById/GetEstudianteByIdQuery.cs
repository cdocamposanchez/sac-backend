#region

using BuildingBlocks.Source.Application.CQRS;
using Sac.Domain.Dtos;

#endregion

namespace Sac.Application.Modules.Estudiantes.Queries.GetEstudianteById;

public record GetEstudianteByIdQuery(long Id) : IQuery<EstudianteDto>;
