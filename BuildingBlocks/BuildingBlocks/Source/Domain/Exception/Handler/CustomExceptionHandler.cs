#region

using System.Text.Json;
using BuildingBlocks.Source.Application.Utils;
using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

#endregion

namespace BuildingBlocks.Source.Domain.Exception.Handler;

/// <summary>
/// Manejador centralizado de excepciones. Convierte cualquier excepción capturada
/// en una respuesta <see cref="ProblemDetails"/> con el código HTTP adecuado.
/// </summary>
public class CustomExceptionHandler(ILogger<CustomExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        System.Exception exception,
        CancellationToken cancellationToken)
    {
        logger.LogError(
            "Error Message: {ExceptionMessage}, Time of occurrence {Time}",
            exception.Message,
            AppDateTime.Now);

        var problemDetails = new ProblemDetails();
        int statusCode;
        string title;
        string detail;

        if (exception is DbUpdateException dbEx)
        {
            statusCode = StatusCodes.Status500InternalServerError;
            title = "DbUpdateException";
            detail = dbEx.InnerException?.Message ?? dbEx.Message;

            problemDetails.Extensions.Add("dbError", dbEx.InnerException?.GetType().FullName);
        }
        else
        {
            statusCode = exception switch
            {
                InternalServerException => StatusCodes.Status500InternalServerError,
                ValidationException => StatusCodes.Status400BadRequest,
                BadRequestException => StatusCodes.Status400BadRequest,
                NotFoundException => StatusCodes.Status404NotFound,
                UnauthorizedException => StatusCodes.Status401Unauthorized,
                UnauthorizedAccessException => StatusCodes.Status401Unauthorized,
                KeyNotFoundException => StatusCodes.Status404NotFound,
                ArgumentException => StatusCodes.Status400BadRequest,
                InvalidOperationException => StatusCodes.Status409Conflict,
                InvalidCastException => StatusCodes.Status400BadRequest,
                NotSupportedException => StatusCodes.Status501NotImplemented,
                TimeoutException => StatusCodes.Status504GatewayTimeout,
                FormatException => StatusCodes.Status400BadRequest,
                JsonException => StatusCodes.Status400BadRequest,
                BadHttpRequestException => StatusCodes.Status400BadRequest,
                DomainException => StatusCodes.Status400BadRequest,
                _ => StatusCodes.Status500InternalServerError
            };

            title = exception.GetType().Name;
            detail = exception.Message;
        }

        problemDetails.Title = title;
        problemDetails.Detail = detail;
        problemDetails.Status = statusCode;
        problemDetails.Instance = httpContext.Request.Path;

        problemDetails.Extensions.Add("traceId", httpContext.TraceIdentifier);
        problemDetails.Extensions.Add("exceptionType", exception.GetType().FullName);

        if (exception is ValidationException validationException)
            problemDetails.Extensions.Add("validationErrors", validationException.Errors);

        if (httpContext.RequestServices.GetService<IHostEnvironment>()?.IsDevelopment() == true)
        {
            problemDetails.Extensions.Add("innerException", exception.InnerException?.Message);
        }

        httpContext.Response.StatusCode = statusCode;
        httpContext.Response.ContentType = "application/json";

        var json = JsonSerializer.Serialize(problemDetails, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });
        await httpContext.Response.WriteAsync(json, cancellationToken);

        return true;
    }
}
