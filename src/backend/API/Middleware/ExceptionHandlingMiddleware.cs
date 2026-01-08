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
            // Log detailed error information
            _logger.LogError(ex,
                "Unhandled exception occurred\n" +
                "Path: {Path}\n" +
                "Method: {Method}\n" +
                "User: {User}\n" +
                "RemoteIP: {RemoteIP}\n" +
                "Exception Type: {ExceptionType}\n" +
                "Message: {Message}\n" +
                "StackTrace: {StackTrace}",
                context.Request.Path,
                context.Request.Method,
                context.User?.Identity?.Name ?? "Anonymous",
                context.Connection.RemoteIpAddress,
                ex.GetType().Name,
                ex.Message,
                ex.StackTrace);

            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        HttpStatusCode statusCode;
        string message;
        object? details = null;

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

            case Microsoft.Data.SqlClient.SqlException sqlException:
                statusCode = HttpStatusCode.InternalServerError;
                message = "A database error occurred. Please try again later.";
                details = new { ErrorNumber = sqlException.Number, SqlState = sqlException.State };
                break;

            default:
                statusCode = HttpStatusCode.InternalServerError;
                message = "An internal server error occurred. Please try again later.";
                // In development, include more details
                if (context.RequestServices.GetService<IWebHostEnvironment>()?.IsDevelopment() == true)
                {
                    details = new
                    {
                        ExceptionType = exception.GetType().Name,
                        ExceptionMessage = exception.Message,
                        StackTrace = exception.StackTrace
                    };
                }
                break;
        }

        var response = new
        {
            StatusCode = (int)statusCode,
            Message = message,
            Details = details,
            Timestamp = DateTime.UtcNow,
            Path = context.Request.Path.ToString()
        };

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        var jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
        };

        return context.Response.WriteAsync(JsonSerializer.Serialize(response, jsonOptions));
    }
}
