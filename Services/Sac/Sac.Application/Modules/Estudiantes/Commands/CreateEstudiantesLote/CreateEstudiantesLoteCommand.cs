#region

using BuildingBlocks.Source.Application.CQRS;
using Sac.Domain.Dtos;

#endregion

namespace Sac.Application.Modules.Estudiantes.Commands.CreateEstudiantesLote;

/// <summary>
/// HU-03 / RF-02 — El Director carga estudiantes en lote (típicamente desde un archivo CSV).
/// La operación es transaccional: si uno falla, ninguno se persiste.
/// </summary>
public record CreateEstudiantesLoteCommand(List<CreateEstudianteDto> Estudiantes) : ICommand<List<EstudianteDto>>;
