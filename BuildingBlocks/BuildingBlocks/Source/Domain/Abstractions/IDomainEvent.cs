#region

using BuildingBlocks.Source.Application.Utils;
using MediatR;

#endregion

namespace BuildingBlocks.Source.Domain.Abstractions;

public interface IDomainEvent : INotification
{
    Guid EventId => Guid.NewGuid();
    public DateTime OccurredOn => AppDateTime.Now;
    public string EventType => GetType().AssemblyQualifiedName ?? string.Empty;
}
