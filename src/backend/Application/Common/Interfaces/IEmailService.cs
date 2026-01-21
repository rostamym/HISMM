namespace HospitalAppointmentSystem.Application.Common.Interfaces;

/// <summary>
/// Email service interface
/// </summary>
public interface IEmailService
{
    Task SendAsync(string to, string subject, string body, CancellationToken cancellationToken = default);
    Task SendRegistrationConfirmationAsync(string email, string firstName, string lastName, CancellationToken cancellationToken = default);
    Task SendAppointmentConfirmationAsync(Guid appointmentId, CancellationToken cancellationToken = default);
    Task SendAppointmentReminderAsync(Guid appointmentId, CancellationToken cancellationToken = default);
    Task SendAppointmentCancellationAsync(Guid appointmentId, CancellationToken cancellationToken = default);
}
