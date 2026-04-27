#region

using MediatR;

#endregion

namespace BuildingBlocks.Source.Application.CQRS;

public interface ICommand : ICommand<Unit>
{
}

public interface ICommand<out TResponse> : IRequest<TResponse>
{
}
