using HospitalAppointmentSystem.Application.Common.Interfaces;
using HospitalAppointmentSystem.Domain.Entities;
using MailKit.Net.Smtp;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;

namespace HospitalAppointmentSystem.Infrastructure.Services.Email;

/// <summary>
/// Email service implementation with support for both SMTP and File-based modes
/// </summary>
public class EmailService : IEmailService
{
    private readonly EmailSettings _settings;
    private readonly IApplicationDbContext _context;
    private readonly ILogger<EmailService> _logger;

    public EmailService(
        IOptions<EmailSettings> settings,
        IApplicationDbContext context,
        ILogger<EmailService> logger)
    {
        _settings = settings.Value;
        _context = context;
        _logger = logger;
    }

    public async Task SendAsync(string to, string subject, string body, CancellationToken cancellationToken = default)
    {
        try
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_settings.FromName, _settings.FromEmail));
            message.To.Add(new MailboxAddress(to, to));
            message.Subject = subject;

            var bodyBuilder = new BodyBuilder { HtmlBody = body };
            message.Body = bodyBuilder.ToMessageBody();

            if (_settings.Mode.Equals("Smtp", StringComparison.OrdinalIgnoreCase))
            {
                await SendViaSmtpAsync(message, cancellationToken);
            }
            else
            {
                await SaveToFileAsync(message, cancellationToken);
            }

            _logger.LogInformation("Email sent successfully to {To} with subject: {Subject}", to, subject);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email to {To} with subject: {Subject}", to, subject);
            throw;
        }
    }

    public async Task SendRegistrationConfirmationAsync(
        string email,
        string firstName,
        string lastName,
        CancellationToken cancellationToken = default)
    {
        var subject = "Welcome to Hospital Appointment System";
        var body = EmailTemplates.GetRegistrationConfirmationTemplate(firstName, lastName);

        await SendAsync(email, subject, body, cancellationToken);
    }

    public async Task SendAppointmentConfirmationAsync(Guid appointmentId, CancellationToken cancellationToken = default)
    {
        var appointment = await _context.Appointments
            .Include(a => a.Patient)
                .ThenInclude(p => p.User)
            .Include(a => a.Doctor)
                .ThenInclude(d => d.User)
            .Include(a => a.Doctor)
                .ThenInclude(d => d.Specialty)
            .FirstOrDefaultAsync(a => a.Id == appointmentId, cancellationToken);

        if (appointment == null)
        {
            _logger.LogWarning("Appointment {AppointmentId} not found for confirmation email", appointmentId);
            return;
        }

        var patientName = $"{appointment.Patient.User.FirstName} {appointment.Patient.User.LastName}";
        var doctorName = $"{appointment.Doctor.User.FirstName} {appointment.Doctor.User.LastName}";
        var specialty = appointment.Doctor.Specialty?.Name ?? "General";
        var appointmentDateTime = appointment.ScheduledDate.Add(appointment.StartTime);

        var subject = "Appointment Confirmation";
        var body = EmailTemplates.GetAppointmentConfirmationTemplate(
            patientName,
            doctorName,
            specialty,
            appointmentDateTime,
            appointment.Reason);

        await SendAsync(appointment.Patient.User.Email, subject, body, cancellationToken);

        // Also notify the doctor
        var doctorSubject = "New Appointment Booked";
        var doctorBody = $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset=""utf-8"">
    <style>
        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; max-width: 600px; margin: 0 auto; padding: 20px; }}
        .header {{ background-color: #2196F3; color: white; padding: 20px; text-align: center; }}
        .content {{ background-color: #f9f9f9; padding: 30px; border: 1px solid #ddd; }}
    </style>
</head>
<body>
    <div class=""header"">
        <h1>New Appointment</h1>
    </div>
    <div class=""content"">
        <h2>Hello Dr. {doctorName},</h2>
        <p>A new appointment has been booked with you:</p>
        <p><strong>Patient:</strong> {patientName}</p>
        <p><strong>Date & Time:</strong> {appointmentDateTime:dddd, MMMM dd, yyyy 'at' hh:mm tt}</p>
        <p><strong>Reason:</strong> {appointment.Reason}</p>
    </div>
</body>
</html>";

        await SendAsync(appointment.Doctor.User.Email, doctorSubject, doctorBody, cancellationToken);
    }

    public async Task SendAppointmentReminderAsync(Guid appointmentId, CancellationToken cancellationToken = default)
    {
        var appointment = await _context.Appointments
            .Include(a => a.Patient)
                .ThenInclude(p => p.User)
            .Include(a => a.Doctor)
                .ThenInclude(d => d.User)
            .FirstOrDefaultAsync(a => a.Id == appointmentId, cancellationToken);

        if (appointment == null)
        {
            _logger.LogWarning("Appointment {AppointmentId} not found for reminder email", appointmentId);
            return;
        }

        var patientName = $"{appointment.Patient.User.FirstName} {appointment.Patient.User.LastName}";
        var doctorName = $"{appointment.Doctor.User.FirstName} {appointment.Doctor.User.LastName}";
        var appointmentDateTime = appointment.ScheduledDate.Add(appointment.StartTime);

        var hoursUntil = (int)(appointmentDateTime - DateTime.UtcNow).TotalHours;

        var subject = $"Appointment Reminder - {hoursUntil} hours";
        var body = EmailTemplates.GetAppointmentReminderTemplate(
            patientName,
            doctorName,
            appointmentDateTime,
            hoursUntil);

        await SendAsync(appointment.Patient.User.Email, subject, body, cancellationToken);
    }

    public async Task SendAppointmentCancellationAsync(Guid appointmentId, CancellationToken cancellationToken = default)
    {
        var appointment = await _context.Appointments
            .Include(a => a.Patient)
                .ThenInclude(p => p.User)
            .Include(a => a.Doctor)
                .ThenInclude(d => d.User)
            .FirstOrDefaultAsync(a => a.Id == appointmentId, cancellationToken);

        if (appointment == null)
        {
            _logger.LogWarning("Appointment {AppointmentId} not found for cancellation email", appointmentId);
            return;
        }

        var patientName = $"{appointment.Patient.User.FirstName} {appointment.Patient.User.LastName}";
        var doctorName = $"{appointment.Doctor.User.FirstName} {appointment.Doctor.User.LastName}";
        var appointmentDateTime = appointment.ScheduledDate.Add(appointment.StartTime);

        var subject = "Appointment Cancelled";
        var body = EmailTemplates.GetAppointmentCancellationTemplate(
            patientName,
            doctorName,
            appointmentDateTime);

        await SendAsync(appointment.Patient.User.Email, subject, body, cancellationToken);

        // Also notify the doctor
        var doctorSubject = "Appointment Cancelled";
        var doctorBody = $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset=""utf-8"">
    <style>
        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; max-width: 600px; margin: 0 auto; padding: 20px; }}
        .header {{ background-color: #F44336; color: white; padding: 20px; text-align: center; }}
        .content {{ background-color: #f9f9f9; padding: 30px; border: 1px solid #ddd; }}
    </style>
</head>
<body>
    <div class=""header"">
        <h1>Appointment Cancelled</h1>
    </div>
    <div class=""content"">
        <h2>Hello Dr. {doctorName},</h2>
        <p>An appointment has been cancelled:</p>
        <p><strong>Patient:</strong> {patientName}</p>
        <p><strong>Date & Time:</strong> {appointmentDateTime:dddd, MMMM dd, yyyy 'at' hh:mm tt}</p>
    </div>
</body>
</html>";

        await SendAsync(appointment.Doctor.User.Email, doctorSubject, doctorBody, cancellationToken);
    }

    /// <summary>
    /// Send email via SMTP server
    /// </summary>
    private async Task SendViaSmtpAsync(MimeMessage message, CancellationToken cancellationToken)
    {
        using var client = new SmtpClient();

        try
        {
            await client.ConnectAsync(_settings.SmtpServer, _settings.SmtpPort, _settings.EnableSsl, cancellationToken);

            if (!string.IsNullOrEmpty(_settings.SmtpUsername))
            {
                await client.AuthenticateAsync(_settings.SmtpUsername, _settings.SmtpPassword, cancellationToken);
            }

            await client.SendAsync(message, cancellationToken);
            await client.DisconnectAsync(true, cancellationToken);

            _logger.LogInformation("Email sent via SMTP to {To}", string.Join(", ", message.To));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email via SMTP");
            throw;
        }
    }

    /// <summary>
    /// Save email to file for development mode
    /// </summary>
    private async Task SaveToFileAsync(MimeMessage message, CancellationToken cancellationToken)
    {
        try
        {
            // Create output directory if it doesn't exist
            var outputPath = Path.Combine(Directory.GetCurrentDirectory(), _settings.FileOutputPath);
            Directory.CreateDirectory(outputPath);

            // Create filename with timestamp
            var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            var safeSubject = string.Join("_", message.Subject.Split(Path.GetInvalidFileNameChars()));
            var fileName = $"{timestamp}_{safeSubject}.html";
            var filePath = Path.Combine(outputPath, fileName);

            // Extract HTML body
            var htmlBody = message.HtmlBody ?? message.TextBody ?? "No content";

            // Create email metadata header
            var emailMetadata = $@"
<!-- Email Metadata
To: {string.Join(", ", message.To)}
From: {string.Join(", ", message.From)}
Subject: {message.Subject}
Date: {DateTime.Now:yyyy-MM-dd HH:mm:ss}
-->
";

            // Save to file
            await File.WriteAllTextAsync(filePath, emailMetadata + htmlBody, cancellationToken);

            _logger.LogInformation("Email saved to file: {FilePath}", filePath);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to save email to file");
            throw;
        }
    }
}
