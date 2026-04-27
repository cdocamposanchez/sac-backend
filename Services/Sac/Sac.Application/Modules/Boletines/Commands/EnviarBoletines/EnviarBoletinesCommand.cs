#region

using BuildingBlocks.Source.Application.CQRS;
using Sac.Domain.Dtos;

#endregion

namespace Sac.Application.Modules.Boletines.Commands.EnviarBoletines;

/// <summary>
/// HU-11 / RF-11 — El Director envía boletines individuales por correo
/// a todos los estudiantes del curso. Solo permitido si el curso está CERRADO.
/// </summary>
public record EnviarBoletinesCommand(long CursoId) : ICommand<EnviarBoletinesResultadoDto>;
