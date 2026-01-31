using Hangfire.Dashboard;

namespace HospitalAppointmentSystem.API.Middleware;

/// <summary>
/// Authorization filter for Hangfire Dashboard
/// In production, this should check for proper authentication and authorization
/// </summary>
public class HangfireAuthorizationFilter : IDashboardAuthorizationFilter
{
    public bool Authorize(DashboardContext context)
    {
        var httpContext = context.GetHttpContext();

        // In development, allow all access
        // TODO: In production, add proper authentication check
        // Example: return httpContext.User.IsInRole("Administrator");

        if (httpContext.Request.Host.Host == "localhost" ||
            httpContext.Request.Host.Host == "127.0.0.1")
        {
            return true;
        }

        // For production, check if user is authenticated and has Administrator role
        return httpContext.User.Identity?.IsAuthenticated == true &&
               httpContext.User.IsInRole("Administrator");
    }
}
