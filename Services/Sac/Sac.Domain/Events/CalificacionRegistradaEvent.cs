#region

using BuildingBlocks.Source.Domain.Abstractions;

#endregion

namespace Sac.Domain.Events;

public record CalificacionRegistradaEvent(
    long CalificacionId,
    long EstudianteId,
    long CursoId,
    long PeriodoId,
    decimal Nota) : IDomainEvent;
