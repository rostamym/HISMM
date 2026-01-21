using HospitalAppointmentSystem.Application.Common.Interfaces;
using HospitalAppointmentSystem.Application.Common.Models;
using HospitalAppointmentSystem.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HospitalAppointmentSystem.Application.Features.Doctors.Commands.SetAvailability;

/// <summary>
/// Handler for SetAvailabilityCommand
/// </summary>
public class SetAvailabilityCommandHandler : IRequestHandler<SetAvailabilityCommand, Result<Guid>>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<SetAvailabilityCommandHandler> _logger;

    public SetAvailabilityCommandHandler(
        IApplicationDbContext context,
        ILogger<SetAvailabilityCommandHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Result<Guid>> Handle(SetAvailabilityCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // 1. Check if doctor exists
            var doctor = await _context.Doctors
                .FirstOrDefaultAsync(d => d.Id == request.DoctorId, cancellationToken);

            if (doctor == null)
            {
                _logger.LogWarning("Doctor not found with ID: {DoctorId}", request.DoctorId);
                return Result<Guid>.Failure("Doctor not found");
            }

            // 2. Check for conflicting availability (same day, overlapping times)
            var hasConflict = await _context.Availabilities
                .Where(a => a.DoctorId == request.DoctorId
                    && a.DayOfWeek == request.DayOfWeek
                    && a.IsActive)
                .AnyAsync(a =>
                    // Check if new availability overlaps with existing
                    (request.StartTime < a.EndTime && request.EndTime > a.StartTime),
                    cancellationToken);

            if (hasConflict)
            {
                _logger.LogWarning(
                    "Conflicting availability found for Doctor {DoctorId} on {DayOfWeek}",
                    request.DoctorId,
                    request.DayOfWeek);

                return Result<Guid>.Failure(
                    $"Doctor already has availability on {request.DayOfWeek} that overlaps with the requested time");
            }

            // 3. Create new availability
            var availability = Availability.Create(
                request.DoctorId,
                request.DayOfWeek,
                request.StartTime,
                request.EndTime,
                request.SlotDurationMinutes
            );

            _context.Availabilities.Add(availability);
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "Created availability for Doctor {DoctorId} on {DayOfWeek} from {StartTime} to {EndTime}",
                request.DoctorId,
                request.DayOfWeek,
                request.StartTime,
                request.EndTime);

            return Result<Guid>.Success(availability.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating availability for Doctor {DoctorId}", request.DoctorId);
            return Result<Guid>.Failure("An error occurred while setting availability");
        }
    }
}
