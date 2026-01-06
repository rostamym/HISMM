using HospitalAppointmentSystem.Domain.Exceptions;
using System.Net;
using System.Text.Json;

namespace HospitalAppointmentSystem.API.Middleware;

/// <summary>
/// Global exception handling middleware
/// Catches all unhandled exceptions and returns appropriate HTTP responses
/// </summary>
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
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        HttpStatusCode statusCode;
        string message;

        switch (exception)
        {
            case DomainException domainException:
                statusCode = HttpStatusCode.BadRequest;
                message = domainException.Message;
                break;

            case UnauthorizedAccessException:
                statusCode = HttpStatusCode.Unauthorized;
                message = "Unauthorized access";
                break;

            case KeyNotFoundException:
                statusCode = HttpStatusCode.NotFound;
                message = "Resource not found";
                break;

            case ArgumentException:
                statusCode = HttpStatusCode.BadRequest;
                message = exception.Message;
                break;

            case InvalidOperationException:
                statusCode = HttpStatusCode.BadRequest;
                message = exception.Message;
                break;

            default:
                statusCode = HttpStatusCode.InternalServerError;
                message = "An internal server error occurred. Please try again later.";
                break;
        }

        var response = new
        {
            StatusCode = (int)statusCode,
            Message = message,
            Timestamp = DateTime.UtcNow
        };

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        var jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        return context.Response.WriteAsync(JsonSerializer.Serialize(response, jsonOptions));
    }
}
