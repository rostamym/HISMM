using HospitalAppointmentSystem.Application.Common.Interfaces;
using HospitalAppointmentSystem.Application.Common.Models;
using HospitalAppointmentSystem.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HospitalAppointmentSystem.Application.Features.Appointments.Commands.CancelAppointment;

/// <summary>
/// Handler for CancelAppointmentCommand
/// </summary>
public class CancelAppointmentCommandHandler : IRequestHandler<CancelAppointmentCommand, Result<bool>>
{
    private readonly IApplicationDbContext _context;
    private readonly IEmailService _emailService;
    private readonly IEmailTemplateService _emailTemplateService;
    private readonly ILogger<CancelAppointmentCommandHandler> _logger;

    public CancelAppointmentCommandHandler(
        IApplicationDbContext context,
        IEmailService emailService,
        IEmailTemplateService emailTemplateService,
        ILogger<CancelAppointmentCommandHandler> logger)
    {
        _context = context;
        _emailService = emailService;
        _emailTemplateService = emailTemplateService;
        _logger = logger;
    }

    public async Task<Result<bool>> Handle(
        CancelAppointmentCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            // Retrieve the appointment
            var appointment = await _context.Appointments
                .FirstOrDefaultAsync(a => a.Id == request.AppointmentId, cancellationToken);

            if (appointment == null)
            {
                _logger.LogWarning("Appointment not found with ID: {AppointmentId}", request.AppointmentId);
                return Result<bool>.Failure("Appointment not found");
            }

            // Check if appointment is already cancelled
            if (appointment.Status == AppointmentStatus.Cancelled)
            {
                _logger.LogInformation("Appointment {AppointmentId} is already cancelled", request.AppointmentId);
                return Result<bool>.Failure("Appointment is already cancelled");
            }

            // Check if appointment is in the past
            var appointmentDateTime = appointment.ScheduledDate.Add(appointment.StartTime);
            if (appointmentDateTime < DateTime.Now)
            {
                _logger.LogWarning(
                    "Cannot cancel past appointment {AppointmentId} scheduled for {DateTime}",
                    request.AppointmentId,
                    appointmentDateTime);

                return Result<bool>.Failure("Cannot cancel appointments in the past");
            }

            // Cancel the appointment using domain method
            appointment.Cancel(request.CancellationReason);

            // Save changes
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "Appointment {AppointmentId} cancelled successfully. Reason: {Reason}",
                request.AppointmentId,
                request.CancellationReason);

            // Send cancellation email in background
            var appointmentId = appointment.Id;
            _ = Task.Run(async () =>
            {
                try
                {
                    // Get full appointment details
                    var appointmentDetails = await _context.Appointments
                        .Include(a => a.Patient)
                            .ThenInclude(p => p.User)
                        .Include(a => a.Doctor)
                            .ThenInclude(d => d.User)
                        .FirstOrDefaultAsync(a => a.Id == appointmentId);

                    if (appointmentDetails == null)
                    {
                        _logger.LogWarning("Appointment not found for cancellation email: {AppointmentId}", appointmentId);
                        return;
                    }

                    var formattedTime = $"{appointmentDetails.StartTime:hh\\:mm} - {appointmentDetails.EndTime:hh\\:mm}";
                    var patientName = appointmentDetails.Patient.User.GetFullName();
                    var doctorName = appointmentDetails.Doctor.User.GetFullName();

                    // Generate and send cancellation email to patient
                    var emailContent = _emailTemplateService.GenerateAppointmentCancellation(
                        patientName,
                        doctorName,
                        appointmentDetails.ScheduledDate,
                        formattedTime,
                        request.CancellationReason
                    );

                    await _emailService.SendAsync(
                        appointmentDetails.Patient.User.Email,
                        "Appointment Cancelled",
                        emailContent
                    );

                    _logger.LogInformation(
                        "Cancellation email sent to patient {PatientEmail} for appointment {AppointmentId}",
                        appointmentDetails.Patient.User.Email,
                        appointmentId);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to send cancellation email for appointment {AppointmentId}", appointmentId);
                }
            });

            return Result<bool>.Success(true);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex, "Invalid operation while cancelling appointment {AppointmentId}", request.AppointmentId);
            return Result<bool>.Failure(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error cancelling appointment {AppointmentId}", request.AppointmentId);
            return Result<bool>.Failure($"Failed to cancel appointment: {ex.Message}");
        }
    }
}
