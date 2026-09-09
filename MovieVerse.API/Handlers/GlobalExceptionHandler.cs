using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MovieVerse.Exceptions;

namespace MovieVerse.Handlers;

public class GlobalExceptionHandler(
    ILogger<GlobalExceptionHandler> logger,
    IHostEnvironment environment) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var (statusCode, title) = exception switch
        {
            AlreadyExistsException => (
                StatusCodes.Status409Conflict,
                "Conflict"),

            ConflictException => (
                StatusCodes.Status409Conflict,
                "Conflict"),

            NotFoundException => (
                StatusCodes.Status404NotFound,
                "Not Found"),

            BadRequestException => (
                StatusCodes.Status400BadRequest,
                "Bad Request"),

            UnauthorizedException => (
                StatusCodes.Status401Unauthorized,
                "Unauthorized"),

            _ => (
                StatusCodes.Status500InternalServerError,
                "Internal Server Error")
        };

        if (statusCode == StatusCodes.Status500InternalServerError)
        {
            logger.LogError(
                exception,
                "Unhandled exception while processing {Method} {Path}. TraceId: {TraceId}",
                httpContext.Request.Method,
                httpContext.Request.Path,
                httpContext.TraceIdentifier);
        }
        else
        {
            logger.LogWarning(
                exception,
                "Request failed with status {StatusCode} while processing {Method} {Path}. TraceId: {TraceId}",
                statusCode,
                httpContext.Request.Method,
                httpContext.Request.Path,
                httpContext.TraceIdentifier);
        }

        var detail = statusCode == StatusCodes.Status500InternalServerError
            ? environment.IsDevelopment()
                ? GetMostSpecificMessage(exception)
                : "Something went wrong while processing the request."
            : exception.Message;

        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = detail,
            Instance = httpContext.Request.Path
        };

        problemDetails.Extensions["traceId"] = httpContext.TraceIdentifier;

        httpContext.Response.StatusCode = statusCode;
        httpContext.Response.ContentType = "application/problem+json";

        await httpContext.Response.WriteAsJsonAsync(
            problemDetails,
            cancellationToken);

        return true;
    }

    private static string GetMostSpecificMessage(Exception exception)
    {
        var current = exception;

        while (current.InnerException is not null)
        {
            current = current.InnerException;
        }

        return string.IsNullOrWhiteSpace(current.Message)
            ? "An unexpected server error occurred."
            : current.Message;
    }
}
