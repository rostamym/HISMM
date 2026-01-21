using HospitalAppointmentSystem.Application.Common.Interfaces;
using HospitalAppointmentSystem.Application.Common.Models;
using HospitalAppointmentSystem.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HospitalAppointmentSystem.Application.Features.Appointments.Commands.RescheduleAppointment;

/// <summary>
/// Handler for RescheduleAppointmentCommand
/// </summary>
public class RescheduleAppointmentCommandHandler : IRequestHandler<RescheduleAppointmentCommand, Result<bool>>
{
    private readonly IApplicationDbContext _context;
    private readonly IEmailService _emailService;
    private readonly ILogger<RescheduleAppointmentCommandHandler> _logger;

    public RescheduleAppointmentCommandHandler(
        IApplicationDbContext context,
        IEmailService emailService,
        ILogger<RescheduleAppointmentCommandHandler> logger)
    {
        _context = context;
        _emailService = emailService;
        _logger = logger;
    }

    public async Task<Result<bool>> Handle(
        RescheduleAppointmentCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            // Parse time strings to TimeSpan
            if (!TimeSpan.TryParse(request.NewStartTime, out var newStartTime))
            {
                _logger.LogWarning("Invalid start time format: {StartTime}", request.NewStartTime);
                return Result<bool>.Failure("Invalid start time format. Use HH:mm format (e.g., 09:00)");
            }

            if (!TimeSpan.TryParse(request.NewEndTime, out var newEndTime))
            {
                _logger.LogWarning("Invalid end time format: {EndTime}", request.NewEndTime);
                return Result<bool>.Failure("Invalid end time format. Use HH:mm format (e.g., 09:30)");
            }

            // Retrieve the appointment
            var appointment = await _context.Appointments
                .FirstOrDefaultAsync(a => a.Id == request.AppointmentId, cancellationToken);

            if (appointment == null)
            {
                _logger.LogWarning("Appointment not found with ID: {AppointmentId}", request.AppointmentId);
                return Result<bool>.Failure("Appointment not found");
            }

            // Check if appointment can be rescheduled
            if (appointment.Status == AppointmentStatus.Cancelled)
            {
                return Result<bool>.Failure("Cannot reschedule a cancelled appointment");
            }

            if (appointment.Status == AppointmentStatus.Completed)
            {
                return Result<bool>.Failure("Cannot reschedule a completed appointment");
            }

            // Check if doctor is available at the new time slot
            var dayOfWeek = request.NewScheduledDate.DayOfWeek;
            var availability = await _context.Availabilities
                .Where(a => a.DoctorId == appointment.DoctorId
                    && a.DayOfWeek == dayOfWeek
                    && a.IsActive)
                .FirstOrDefaultAsync(cancellationToken);

            if (availability == null)
            {
                _logger.LogInformation(
                    "Doctor {DoctorId} is not available on {DayOfWeek}",
                    appointment.DoctorId,
                    dayOfWeek);

                return Result<bool>.Failure($"Doctor is not available on {dayOfWeek}");
            }

            // Verify new time is within doctor's availability hours
            if (newStartTime < availability.StartTime || newEndTime > availability.EndTime)
            {
                _logger.LogInformation(
                    "Requested time {StartTime}-{EndTime} is outside doctor's availability {AvailStart}-{AvailEnd}",
                    newStartTime,
                    newEndTime,
                    availability.StartTime,
                    availability.EndTime);

                return Result<bool>.Failure(
                    $"Doctor is only available between {availability.StartTime:hh\\:mm} and {availability.EndTime:hh\\:mm} on {dayOfWeek}");
            }

            // Check for conflicting appointments at the new time
            var hasConflict = await _context.Appointments
                .AnyAsync(a => a.DoctorId == appointment.DoctorId
                    && a.Id != appointment.Id // Exclude current appointment
                    && a.ScheduledDate.Date == request.NewScheduledDate.Date
                    && (a.Status == AppointmentStatus.Scheduled
                        || a.Status == AppointmentStatus.Confirmed
                        || a.Status == AppointmentStatus.InProgress)
                    && a.StartTime < newEndTime
                    && a.EndTime > newStartTime,
                    cancellationToken);

            if (hasConflict)
            {
                _logger.LogWarning(
                    "Appointment conflict detected for Doctor {DoctorId} on {Date} at {StartTime}-{EndTime}",
                    appointment.DoctorId,
                    request.NewScheduledDate,
                    newStartTime,
                    newEndTime);

                return Result<bool>.Failure("The new time slot is already booked. Please select another time slot.");
            }

            // Reschedule the appointment using domain method
            appointment.Reschedule(
                request.NewScheduledDate,
                newStartTime,
                newEndTime);

            // Save changes
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "Appointment {AppointmentId} rescheduled successfully to {NewDate} {NewStartTime}-{NewEndTime}",
                request.AppointmentId,
                request.NewScheduledDate,
                newStartTime,
                newEndTime);

            // Send notification emails (fire and forget)
            _ = _emailService.SendAppointmentConfirmationAsync(appointment.Id, cancellationToken);

            return Result<bool>.Success(true);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex, "Invalid operation while rescheduling appointment {AppointmentId}", request.AppointmentId);
            return Result<bool>.Failure(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error rescheduling appointment {AppointmentId}", request.AppointmentId);
            return Result<bool>.Failure($"Failed to reschedule appointment: {ex.Message}");
        }
    }
}
