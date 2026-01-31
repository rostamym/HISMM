using HospitalAppointmentSystem.Application.Common.Interfaces;
using HospitalAppointmentSystem.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HospitalAppointmentSystem.Infrastructure.BackgroundJobs;

/// <summary>
/// Background job to clean up old data from the database
/// Runs weekly to maintain database health and performance
/// </summary>
public class DatabaseCleanupJob
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<DatabaseCleanupJob> _logger;

    public DatabaseCleanupJob(
        IApplicationDbContext context,
        ILogger<DatabaseCleanupJob> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Execute the database cleanup job
    /// </summary>
    public async Task ExecuteAsync()
    {
        _logger.LogInformation("Starting DatabaseCleanupJob execution");

        try
        {
            var now = DateTime.UtcNow;
            var deletionThreshold = now.AddMonths(-6); // Delete cancelled appointments older than 6 months

            // Find old cancelled appointments to delete
            var oldCancelledAppointments = await _context.Appointments
                .Where(a =>
                    a.Status == AppointmentStatus.Cancelled &&
                    a.UpdatedAt < deletionThreshold)
                .ToListAsync();

            if (oldCancelledAppointments.Count == 0)
            {
                _logger.LogInformation("No old cancelled appointments to clean up");
                return;
            }

            _logger.LogInformation(
                "Found {Count} old cancelled appointments to delete (older than {Threshold})",
                oldCancelledAppointments.Count,
                deletionThreshold);

            // Remove the appointments
            _context.Appointments.RemoveRange(oldCancelledAppointments);

            // Save changes
            var deletedCount = await _context.SaveChangesAsync(CancellationToken.None);

            _logger.LogInformation(
                "DatabaseCleanupJob completed. Deleted {Count} old cancelled appointments",
                deletedCount);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing DatabaseCleanupJob");
            throw;
        }
    }
}
