#region

using System.Diagnostics;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

#endregion

namespace BuildingBlocks.Source.Application.Behaviors;

public class LoggingBehavior<TRequest, TResponse>(
    ILogger<LoggingBehavior<TRequest, TResponse>> logger,
    IHttpContextAccessor httpContextAccessor)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull, IRequest<TResponse>
    where TResponse : notnull
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("[START] Handle Request={TRequest}", typeof(TRequest).Name);

        var timer = Stopwatch.StartNew();

        try
        {
            var response = await next();

            timer.Stop();

            if (httpContextAccessor.HttpContext != null)
                httpContextAccessor.HttpContext.Items["HandlerDuration"] = timer.Elapsed.TotalMilliseconds;

            if (timer.Elapsed.Seconds > 3)
                logger.LogWarning("[PERFORMANCE] Slow request: {Request} took {Time}s",
                    typeof(TRequest).Name, timer.Elapsed.Seconds);

            logger.LogInformation("[END] Handled {Request}", typeof(TRequest).Name);
            return response;
        }
        catch
        {
            timer.Stop();
            if (httpContextAccessor.HttpContext != null)
                httpContextAccessor.HttpContext.Items["HandlerDuration"] = timer.Elapsed.TotalMilliseconds;
            throw;
        }
    }
}
