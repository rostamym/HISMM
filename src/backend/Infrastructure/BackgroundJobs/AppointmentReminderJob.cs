using HospitalAppointmentSystem.Application.Common.Interfaces;
using HospitalAppointmentSystem.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HospitalAppointmentSystem.Infrastructure.BackgroundJobs;

/// <summary>
/// Background job to send appointment reminders 24 hours before scheduled time
/// </summary>
public class AppointmentReminderJob
{
    private readonly IApplicationDbContext _context;
    private readonly IEmailService _emailService;
    private readonly IEmailTemplateService _emailTemplateService;
    private readonly ILogger<AppointmentReminderJob> _logger;

    public AppointmentReminderJob(
        IApplicationDbContext context,
        IEmailService emailService,
        IEmailTemplateService emailTemplateService,
        ILogger<AppointmentReminderJob> logger)
    {
        _context = context;
        _emailService = emailService;
        _emailTemplateService = emailTemplateService;
        _logger = logger;
    }

    /// <summary>
    /// Execute the reminder job - finds appointments 24h away and sends reminders
    /// </summary>
    public async Task ExecuteAsync()
    {
        _logger.LogInformation("Starting AppointmentReminderJob execution");

        try
        {
            var now = DateTime.UtcNow;
            var tomorrow = now.AddHours(24);

            // Find appointments that are scheduled for tomorrow (24h window)
            var reminderWindowStart = tomorrow.Date;
            var reminderWindowEnd = reminderWindowStart.AddDays(1);

            var appointmentsToRemind = await _context.Appointments
                .Include(a => a.Patient)
                    .ThenInclude(p => p.User)
                .Include(a => a.Doctor)
                    .ThenInclude(d => d.User)
                .Include(a => a.Doctor)
                    .ThenInclude(d => d.Specialty)
                .Where(a =>
                    a.ScheduledDate >= reminderWindowStart &&
                    a.ScheduledDate < reminderWindowEnd &&
                    (a.Status == AppointmentStatus.Scheduled || a.Status == AppointmentStatus.Confirmed))
                .ToListAsync();

            _logger.LogInformation(
                "Found {Count} appointments to send reminders for (date range: {Start} to {End})",
                appointmentsToRemind.Count,
                reminderWindowStart,
                reminderWindowEnd);

            var successCount = 0;
            var failureCount = 0;

            foreach (var appointment in appointmentsToRemind)
            {
                try
                {
                    var patientName = appointment.Patient.User.GetFullName();
                    var doctorName = appointment.Doctor.User.GetFullName();
                    var specialty = appointment.Doctor.Specialty?.Name ?? "General";
                    var formattedTime = $"{appointment.StartTime:hh\\:mm} - {appointment.EndTime:hh\\:mm}";

                    // Generate reminder email
                    var emailContent = _emailTemplateService.GenerateAppointmentReminder(
                        patientName,
                        doctorName,
                        specialty,
                        appointment.ScheduledDate,
                        formattedTime,
                        appointment.Reason
                    );

                    // Send reminder email
                    await _emailService.SendAsync(
                        appointment.Patient.User.Email,
                        "Appointment Reminder - Tomorrow",
                        emailContent
                    );

                    _logger.LogInformation(
                        "Reminder email sent successfully for appointment {AppointmentId} to {PatientEmail}",
                        appointment.Id,
                        appointment.Patient.User.Email);

                    successCount++;
                }
                catch (Exception ex)
                {
                    _logger.LogError(
                        ex,
                        "Failed to send reminder email for appointment {AppointmentId}",
                        appointment.Id);
                    failureCount++;
                }
            }

            _logger.LogInformation(
                "AppointmentReminderJob completed. Success: {Success}, Failures: {Failures}",
                successCount,
                failureCount);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing AppointmentReminderJob");
            throw;
        }
    }
}
