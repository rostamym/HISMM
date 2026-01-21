using HospitalAppointmentSystem.Application.Common.Interfaces;
using HospitalAppointmentSystem.Application.Common.Models;
using HospitalAppointmentSystem.Domain.Entities;
using HospitalAppointmentSystem.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace HospitalAppointmentSystem.Application.Features.Appointments.Commands;

/// <summary>
/// Command to create a new appointment
/// </summary>
public record CreateAppointmentCommand : IRequest<Result<Guid>>
{
    public Guid PatientId { get; init; }
    public Guid DoctorId { get; init; }
    public DateTime ScheduledDate { get; init; }
    public string StartTime { get; init; } = string.Empty;
    public string EndTime { get; init; } = string.Empty;
    public string Reason { get; init; } = string.Empty;
}

/// <summary>
/// Handler for CreateAppointmentCommand
/// </summary>
public class CreateAppointmentCommandHandler : IRequestHandler<CreateAppointmentCommand, Result<Guid>>
{
    private readonly IApplicationDbContext _context;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<CreateAppointmentCommandHandler> _logger;

    public CreateAppointmentCommandHandler(
        IApplicationDbContext context,
        IServiceProvider serviceProvider,
        ILogger<CreateAppointmentCommandHandler> logger)
    {
        _context = context;
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public async Task<Result<Guid>> Handle(CreateAppointmentCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // 0. Convert time strings to TimeSpan
            if (!TimeSpan.TryParse(request.StartTime, out var startTime))
            {
                _logger.LogWarning("Invalid start time format: {StartTime}", request.StartTime);
                return Result<Guid>.Failure("Invalid start time format. Use HH:mm format (e.g., 09:00)");
            }

            if (!TimeSpan.TryParse(request.EndTime, out var endTime))
            {
                _logger.LogWarning("Invalid end time format: {EndTime}", request.EndTime);
                return Result<Guid>.Failure("Invalid end time format. Use HH:mm format (e.g., 09:30)");
            }

            // 1. Validate doctor exists
            var doctorExists = await _context.Doctors
                .AnyAsync(d => d.Id == request.DoctorId, cancellationToken);

            if (!doctorExists)
            {
                _logger.LogWarning("Doctor not found with ID: {DoctorId}", request.DoctorId);
                return Result<Guid>.Failure("Doctor not found");
            }

            // 2. Validate patient exists
            var patientExists = await _context.Patients
                .AnyAsync(p => p.Id == request.PatientId, cancellationToken);

            if (!patientExists)
            {
                _logger.LogWarning("Patient not found with ID: {PatientId}", request.PatientId);
                return Result<Guid>.Failure("Patient not found");
            }

            // 3. Check if doctor is available at this time slot
            var dayOfWeek = request.ScheduledDate.DayOfWeek;
            var availability = await _context.Availabilities
                .Where(a => a.DoctorId == request.DoctorId
                    && a.DayOfWeek == dayOfWeek
                    && a.IsActive)
                .FirstOrDefaultAsync(cancellationToken);

            if (availability == null)
            {
                _logger.LogInformation(
                    "Doctor {DoctorId} is not available on {DayOfWeek}",
                    request.DoctorId,
                    dayOfWeek);

                return Result<Guid>.Failure($"Doctor is not available on {dayOfWeek}");
            }

            // 4. Verify requested time is within doctor's availability hours
            if (startTime < availability.StartTime || endTime > availability.EndTime)
            {
                _logger.LogInformation(
                    "Requested time {StartTime}-{EndTime} is outside doctor's availability {AvailStart}-{AvailEnd}",
                    startTime,
                    endTime,
                    availability.StartTime,
                    availability.EndTime);

                return Result<Guid>.Failure(
                    $"Doctor is only available between {availability.StartTime:hh\\:mm} and {availability.EndTime:hh\\:mm} on {dayOfWeek}");
            }

            // 5. Check for conflicting appointments (double-booking prevention)
            var hasConflict = await _context.Appointments
                .AnyAsync(a => a.DoctorId == request.DoctorId
                    && a.ScheduledDate.Date == request.ScheduledDate.Date
                    && (a.Status == AppointmentStatus.Scheduled
                        || a.Status == AppointmentStatus.Confirmed
                        || a.Status == AppointmentStatus.InProgress)
                    && a.StartTime < endTime
                    && a.EndTime > startTime,
                    cancellationToken);

            if (hasConflict)
            {
                _logger.LogWarning(
                    "Appointment conflict detected for Doctor {DoctorId} on {Date} at {StartTime}-{EndTime}",
                    request.DoctorId,
                    request.ScheduledDate,
                    startTime,
                    endTime);

                return Result<Guid>.Failure("This time slot is already booked. Please select another time slot.");
            }

            // 6. Create appointment entity
            var appointment = Appointment.Create(
                request.PatientId,
                request.DoctorId,
                request.ScheduledDate,
                startTime,
                endTime,
                request.Reason
            );

            // 7. Add to database
            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "Appointment created successfully: {AppointmentId} for Patient {PatientId} with Doctor {DoctorId}",
                appointment.Id,
                request.PatientId,
                request.DoctorId);

            // 8. Send confirmation email in background with its own scope
            var appointmentId = appointment.Id;
            _ = Task.Run(async () =>
            {
                try
                {
                    using var scope = _serviceProvider.CreateScope();
                    var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();
                    await emailService.SendAppointmentConfirmationAsync(appointmentId, CancellationToken.None);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to send appointment confirmation email for appointment {AppointmentId}", appointmentId);
                }
            });

            return Result<Guid>.Success(appointment.Id);
        }
        catch (ArgumentException ex)
        {
            _logger.LogError(ex, "Validation error creating appointment");
            return Result<Guid>.Failure(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating appointment");
            return Result<Guid>.Failure($"Failed to create appointment: {ex.Message}");
        }
    }
}
