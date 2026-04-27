#region

using System.Text.Json;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

#endregion

namespace BuildingBlocks.Source.Infrastructure.Config;

public static class InvalidModelStateResponseConfigurator
{
    public static void Configure(ApiBehaviorOptions options)
    {
        options.InvalidModelStateResponseFactory = context =>
        {
            var httpContext = context.HttpContext;

            var errors = context.ModelState
                .Where(e => e.Value?.Errors.Count > 0)
                .ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value!.Errors.Select(e => e.ErrorMessage).ToArray()
                );

            var problemDetails = new ValidationProblemDetails(errors)
            {
                Title = "Error de validación de modelo o JSON mal formado",
                Detail = "Error de validación de modelo o JSON mal formado",
                Status = StatusCodes.Status400BadRequest,
                Instance = httpContext.Request.Path,
                Type = "https://tools.ietf.org/html/rfc9110#section-15.5.1",
                Extensions =
                {
                    ["traceId"] = httpContext.TraceIdentifier,
                    ["tipoExcepcion"] = "JsonModelBindingException",
                    ["httpMethod"] = httpContext.Request.Method,
                    ["contentType"] = httpContext.Request.ContentType ?? "N/A"
                }
            };

            var exceptionFeature = httpContext.Features.Get<IExceptionHandlerFeature>();
            if (exceptionFeature?.Error is not JsonException jsonEx) return new BadRequestObjectResult(problemDetails);

            problemDetails.Extensions["jsonExceptionMessage"] = jsonEx.Message;
            problemDetails.Extensions["jsonExceptionPath"] = jsonEx.Path;

            return new BadRequestObjectResult(problemDetails);
        };
    }
}
