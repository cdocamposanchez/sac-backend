#region

using System.Text.Json;
using BuildingBlocks.Source.Application.Dtos;
using BuildingBlocks.Source.Application.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

#endregion

namespace BuildingBlocks.Source.Infrastructure.Middleware;

public class ApiResponseMiddleware(
    RequestDelegate next,
    ILogger<ApiResponseMiddleware> logger)
{
    private static readonly JsonSerializerOptions jsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true
    };

    public async Task InvokeAsync(HttpContext context)
    {
        var path = context.Request.Path.Value ?? string.Empty;

        logger.LogDebug("[ RESPONSE MIDDLEWARE ] Incoming request: {Method} {Path}",
            context.Request.Method, path);

        if (ShouldBypass(path))
        {
            logger.LogDebug("[ RESPONSE MIDDLEWARE ] Bypassing middleware for path: {Path}", path);
            await next(context);
            return;
        }

        var originalBodyStream = context.Response.Body;
        await using var memoryStream = new MemoryStream();
        context.Response.Body = memoryStream;

        await next(context);

        if (context.Response.HasStarted)
        {
            logger.LogWarning("[ RESPONSE MIDDLEWARE ] Response already started, skipping wrap. Path: {Path}", path);
            context.Response.Body = originalBodyStream;
            return;
        }

        if (context.Response.StatusCode == StatusCodes.Status204NoContent)
        {
            logger.LogDebug("[ RESPONSE MIDDLEWARE ] 204 NoContent, skipping wrap. Path: {Path}", path);
            context.Response.Body = originalBodyStream;
            return;
        }

        var contentType = context.Response.ContentType ?? string.Empty;

        if (!contentType.StartsWith("application/json", StringComparison.OrdinalIgnoreCase))
        {
            logger.LogDebug(
                "[ RESPONSE MIDDLEWARE ] Non-JSON response detected ({ContentType}), passing through. Path: {Path}",
                contentType, path);

            context.Response.Body = originalBodyStream;
            _ = memoryStream.Seek(0, SeekOrigin.Begin);
            await memoryStream.CopyToAsync(originalBodyStream);
            return;
        }

        _ = memoryStream.Seek(0, SeekOrigin.Begin);
        var responseBody = await new StreamReader(memoryStream).ReadToEndAsync();

        object? responseData = null;

        if (!string.IsNullOrWhiteSpace(responseBody))
            try
            {
                responseData = JsonSerializer.Deserialize<object>(responseBody);
            }
            catch (JsonException ex)
            {
                logger.LogWarning(ex,
                    "[ RESPONSE MIDDLEWARE ] Failed to deserialize JSON body, returning raw string. Path: {Path}",
                    path);

                responseData = responseBody;
            }

        var message = context.Response.StatusCode switch
        {
            StatusCodes.Status401Unauthorized => "Unauthorized",
            StatusCodes.Status403Forbidden => "Forbidden",
            StatusCodes.Status404NotFound => "Not Found",
            StatusCodes.Status500InternalServerError => "Internal Server Error",
            _ => "Success"
        };

        var extraDetails = context.Items["ResponseDetails"] as Dictionary<string, object>;

        var wrappedResponse = new ApiResponseDto<object>
        {
            Message = message,
            ResponseTime = AppDateTime.Now,
            Data = responseData,
            Detalles = extraDetails
        };

        var wrappedJson = JsonSerializer.Serialize(wrappedResponse, jsonOptions);

        context.Response.Body = originalBodyStream;
        context.Response.ContentType = "application/json";

        logger.LogInformation(
            "[ RESPONSE MIDDLEWARE ] Response wrapped successfully. Path: {Path}, StatusCode: {StatusCode}",
            path, context.Response.StatusCode);

        await context.Response.WriteAsync(wrappedJson);
    }

    private static bool ShouldBypass(string path)
    {
        return
            path.Contains("/swagger", StringComparison.OrdinalIgnoreCase) ||
            path.Contains("/docs", StringComparison.OrdinalIgnoreCase) ||
            path.Contains("/health", StringComparison.OrdinalIgnoreCase) ||
            path.EndsWith(".css", StringComparison.OrdinalIgnoreCase) ||
            path.EndsWith(".js", StringComparison.OrdinalIgnoreCase) ||
            path.EndsWith(".map", StringComparison.OrdinalIgnoreCase) ||
            path.EndsWith(".png", StringComparison.OrdinalIgnoreCase) ||
            path.EndsWith(".ico", StringComparison.OrdinalIgnoreCase) ||
            path.EndsWith(".html", StringComparison.OrdinalIgnoreCase) ||
            path.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase) ||
            path.EndsWith(".csv", StringComparison.OrdinalIgnoreCase);
    }
}
