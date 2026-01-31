namespace HospitalAppointmentSystem.Application.Common.Interfaces;

/// <summary>
/// Service for generating email templates
/// </summary>
public interface IEmailTemplateService
{
    /// <summary>
    /// Generate appointment confirmation email
    /// </summary>
    string GenerateAppointmentConfirmation(
        string patientName,
        string doctorName,
        string specialty,
        DateTime appointmentDate,
        string appointmentTime,
        string reason);

    /// <summary>
    /// Generate appointment reminder email (24h before)
    /// </summary>
    string GenerateAppointmentReminder(
        string patientName,
        string doctorName,
        string specialty,
        DateTime appointmentDate,
        string appointmentTime,
        string reason);

    /// <summary>
    /// Generate appointment cancellation email
    /// </summary>
    string GenerateAppointmentCancellation(
        string patientName,
        string doctorName,
        DateTime appointmentDate,
        string appointmentTime,
        string cancellationReason);

    /// <summary>
    /// Generate appointment rescheduled email
    /// </summary>
    string GenerateAppointmentRescheduled(
        string patientName,
        string doctorName,
        DateTime oldDate,
        string oldTime,
        DateTime newDate,
        string newTime);

    /// <summary>
    /// Generate new appointment notification for doctor
    /// </summary>
    string GenerateDoctorNewAppointmentNotification(
        string doctorName,
        string patientName,
        DateTime appointmentDate,
        string appointmentTime,
        string reason);

    /// <summary>
    /// Generate appointment completion confirmation
    /// </summary>
    string GenerateAppointmentCompleted(
        string patientName,
        string doctorName,
        DateTime appointmentDate,
        string notes);

    /// <summary>
    /// Generate welcome email for new users
    /// </summary>
    string GenerateWelcomeEmail(
        string userName,
        string userRole);
}
