using HospitalAppointmentSystem.Application.Common.Interfaces;
using Microsoft.Extensions.Logging;

namespace HospitalAppointmentSystem.Infrastructure.Services;

/// <summary>
/// Email service implementation (placeholder for now)
/// TODO: Implement actual email sending logic with SendGrid or SMTP
/// </summary>
public class EmailService : IEmailService
{
    private readonly ILogger<EmailService> _logger;

    public EmailService(ILogger<EmailService> logger)
    {
        _logger = logger;
    }

    public async Task SendAsync(string to, string subject, string body, CancellationToken cancellationToken = default)
    {
        // TODO: Implement actual email sending
        _logger.LogInformation("Sending email to {To} with subject {Subject}", to, subject);
        await Task.CompletedTask;
    }

    public async Task SendAppointmentConfirmationAsync(Guid appointmentId, CancellationToken cancellationToken = default)
    {
        // TODO: Implement appointment confirmation email
        _logger.LogInformation("Sending appointment confirmation for appointment {AppointmentId}", appointmentId);
        await Task.CompletedTask;
    }

    public async Task SendAppointmentReminderAsync(Guid appointmentId, CancellationToken cancellationToken = default)
    {
        // TODO: Implement appointment reminder email
        _logger.LogInformation("Sending appointment reminder for appointment {AppointmentId}", appointmentId);
        await Task.CompletedTask;
    }

    public async Task SendAppointmentCancellationAsync(Guid appointmentId, CancellationToken cancellationToken = default)
    {
        // TODO: Implement appointment cancellation email
        _logger.LogInformation("Sending appointment cancellation for appointment {AppointmentId}", appointmentId);
        await Task.CompletedTask;
    }
}
