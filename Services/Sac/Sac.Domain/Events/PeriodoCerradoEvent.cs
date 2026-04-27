#region

using BuildingBlocks.Source.Domain.Abstractions;

#endregion

namespace Sac.Domain.Events;

public record PeriodoCerradoEvent(long PeriodoId, long CursoId) : IDomainEvent;
