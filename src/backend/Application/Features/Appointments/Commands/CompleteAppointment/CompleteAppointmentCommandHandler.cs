using HospitalAppointmentSystem.Application.Common.Interfaces;
using HospitalAppointmentSystem.Application.Common.Models;
using HospitalAppointmentSystem.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HospitalAppointmentSystem.Application.Features.Appointments.Commands.CompleteAppointment;

public class CompleteAppointmentCommandHandler : IRequestHandler<CompleteAppointmentCommand, Result<bool>>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<CompleteAppointmentCommandHandler> _logger;

    public CompleteAppointmentCommandHandler(
        IApplicationDbContext context,
        ILogger<CompleteAppointmentCommandHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Result<bool>> Handle(
        CompleteAppointmentCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation(
                "Marking appointment {AppointmentId} as completed",
                request.AppointmentId);

            var appointment = await _context.Appointments
                .FirstOrDefaultAsync(a => a.Id == request.AppointmentId, cancellationToken);

            if (appointment == null)
            {
                _logger.LogWarning("Appointment not found: {AppointmentId}", request.AppointmentId);
                return Result<bool>.Failure("Appointment not found");
            }

            // Check if can be completed
            if (appointment.Status == AppointmentStatus.Cancelled)
            {
                _logger.LogWarning(
                    "Cannot complete cancelled appointment: {AppointmentId}",
                    request.AppointmentId);
                return Result<bool>.Failure("Cannot complete a cancelled appointment");
            }

            if (appointment.Status == AppointmentStatus.Completed)
            {
                _logger.LogWarning(
                    "Appointment already completed: {AppointmentId}",
                    request.AppointmentId);
                return Result<bool>.Failure("Appointment is already completed");
            }

            // Mark as completed
            appointment.Complete(request.Notes);

            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "Appointment {AppointmentId} marked as completed successfully",
                request.AppointmentId);

            return Result<bool>.Success(true);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(
                ex,
                "Invalid operation completing appointment {AppointmentId}",
                request.AppointmentId);
            return Result<bool>.Failure(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error completing appointment {AppointmentId}",
                request.AppointmentId);
            return Result<bool>.Failure($"Failed to complete appointment: {ex.Message}");
        }
    }
}
