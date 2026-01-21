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
    private readonly ILogger<CancelAppointmentCommandHandler> _logger;

    public CancelAppointmentCommandHandler(
        IApplicationDbContext context,
        IEmailService emailService,
        ILogger<CancelAppointmentCommandHandler> logger)
    {
        _context = context;
        _emailService = emailService;
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

            // Send cancellation emails (fire and forget)
            _ = _emailService.SendAppointmentCancellationAsync(appointment.Id, cancellationToken);

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
