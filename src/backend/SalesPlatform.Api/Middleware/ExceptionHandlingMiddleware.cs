using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using ApplicationValidationException = SalesPlatform.Application.Common.Exceptions.ValidationException;
using ForbiddenAccessException = SalesPlatform.Application.Common.Exceptions.ForbiddenAccessException;
using NotFoundException = SalesPlatform.Application.Common.Exceptions.NotFoundException;

namespace SalesPlatform.Api.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception exception)
        {
            await HandleExceptionAsync(context, exception);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var (statusCode, problemDetails) = exception switch
        {
            ApplicationValidationException validationException => (StatusCodes.Status400BadRequest, (ProblemDetails)new ValidationProblemDetails(validationException.Errors)
            {
                Title = "One or more validation errors occurred.",
                Status = StatusCodes.Status400BadRequest
            }),

            NotFoundException notFoundException => (StatusCodes.Status404NotFound, new ProblemDetails
            {
                Title = "The requested resource was not found.",
                Detail = notFoundException.Message,
                Status = StatusCodes.Status404NotFound
            }),

            ForbiddenAccessException forbiddenException => (StatusCodes.Status403Forbidden, new ProblemDetails
            {
                Title = "You do not have permission to access this resource.",
                Detail = forbiddenException.Message,
                Status = StatusCodes.Status403Forbidden
            }),

            UnauthorizedAccessException unauthorizedException => (StatusCodes.Status401Unauthorized, new ProblemDetails
            {
                Title = "Unauthorized.",
                Detail = unauthorizedException.Message,
                Status = StatusCodes.Status401Unauthorized
            }),

            _ => (StatusCodes.Status500InternalServerError, LogAndBuildUnexpectedErrorDetails(exception, context))
        };

        context.Response.ContentType = "application/problem+json";
        context.Response.StatusCode = statusCode;

        // Serialize by the runtime type (problemDetails.GetType()), not the static ProblemDetails
        // type — otherwise ValidationProblemDetails.Errors gets sliced off by the generic overload.
        await context.Response.WriteAsync(JsonSerializer.Serialize(problemDetails, problemDetails.GetType()));
    }

    private ProblemDetails LogAndBuildUnexpectedErrorDetails(Exception exception, HttpContext context)
    {
        var correlationId = context.TraceIdentifier;

        // The full stack trace goes only to the logs; it is never exposed to the client.
        _logger.LogError(exception, "Unhandled exception. CorrelationId: {CorrelationId}", correlationId);

        return new ProblemDetails
        {
            Title = "An unexpected error occurred. Please try again or contact support.",
            Detail = $"Correlation ID: {correlationId}",
            Status = StatusCodes.Status500InternalServerError
        };
    }
}
