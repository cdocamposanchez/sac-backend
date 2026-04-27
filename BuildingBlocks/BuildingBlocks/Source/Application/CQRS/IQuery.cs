#region

using MediatR;

#endregion

namespace BuildingBlocks.Source.Application.CQRS;

public interface IQuery<out TResponse> : IRequest<TResponse> where TResponse : notnull
{
}
