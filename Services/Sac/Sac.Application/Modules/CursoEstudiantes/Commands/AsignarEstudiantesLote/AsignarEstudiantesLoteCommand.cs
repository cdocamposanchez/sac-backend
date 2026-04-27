#region

using BuildingBlocks.Source.Application.CQRS;
using Sac.Domain.Dtos;

#endregion

namespace Sac.Application.Modules.CursoEstudiantes.Commands.AsignarEstudiantesLote;

public record AsignarEstudiantesLoteCommand(AsignarEstudiantesLoteDto Datos) : ICommand<List<CursoEstudianteDto>>;
