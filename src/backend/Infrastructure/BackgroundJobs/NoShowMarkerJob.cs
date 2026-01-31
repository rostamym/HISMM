using HospitalAppointmentSystem.Application.Common.Interfaces;
using HospitalAppointmentSystem.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HospitalAppointmentSystem.Infrastructure.BackgroundJobs;

/// <summary>
/// Background job to automatically mark appointments as No-Show if patient didn't arrive
/// Runs every 30 minutes to check for appointments that are 30+ minutes past start time
/// </summary>
public class NoShowMarkerJob
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<NoShowMarkerJob> _logger;

    public NoShowMarkerJob(
        IApplicationDbContext context,
        ILogger<NoShowMarkerJob> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Execute the no-show marker job
    /// </summary>
    public async Task ExecuteAsync()
    {
        _logger.LogInformation("Starting NoShowMarkerJob execution");

        try
        {
            var now = DateTime.UtcNow;
            var cutoffTime = now.AddMinutes(-30); // 30 minutes grace period

            // Find appointments that should be marked as no-show
            // Criteria:
            // - Status is still Scheduled or Confirmed
            // - Scheduled date/time is more than 30 minutes in the past
            var appointmentsToMark = await _context.Appointments
                .Where(a =>
                    (a.Status == AppointmentStatus.Scheduled || a.Status == AppointmentStatus.Confirmed) &&
                    (a.ScheduledDate < cutoffTime.Date ||
                     (a.ScheduledDate == cutoffTime.Date && a.StartTime < cutoffTime.TimeOfDay)))
                .ToListAsync();

            if (appointmentsToMark.Count == 0)
            {
                _logger.LogInformation("No appointments to mark as no-show");
                return;
            }

            _logger.LogInformation(
                "Found {Count} appointments to mark as no-show",
                appointmentsToMark.Count);

            var markedCount = 0;

            foreach (var appointment in appointmentsToMark)
            {
                try
                {
                    // Mark as no-show using domain method
                    appointment.MarkAsNoShow();

                    _logger.LogInformation(
                        "Marked appointment {AppointmentId} as no-show (scheduled: {ScheduledDate} {StartTime})",
                        appointment.Id,
                        appointment.ScheduledDate,
                        appointment.StartTime);

                    markedCount++;
                }
                catch (Exception ex)
                {
                    _logger.LogError(
                        ex,
                        "Failed to mark appointment {AppointmentId} as no-show",
                        appointment.Id);
                }
            }

            // Save all changes
            await _context.SaveChangesAsync(CancellationToken.None);

            _logger.LogInformation(
                "NoShowMarkerJob completed. Marked {Count} appointments as no-show",
                markedCount);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing NoShowMarkerJob");
            throw;
        }
    }
}
