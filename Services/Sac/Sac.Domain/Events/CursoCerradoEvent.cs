#region

using BuildingBlocks.Source.Domain.Abstractions;

#endregion

namespace Sac.Domain.Events;

public record CursoCerradoEvent(long CursoId) : IDomainEvent;
