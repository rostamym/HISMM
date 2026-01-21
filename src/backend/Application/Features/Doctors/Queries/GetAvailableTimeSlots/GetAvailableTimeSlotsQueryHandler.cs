using HospitalAppointmentSystem.Application.Common.Interfaces;
using HospitalAppointmentSystem.Application.Common.Models;
using HospitalAppointmentSystem.Application.Features.Doctors.Queries.GetAvailableTimeSlots.DTOs;
using HospitalAppointmentSystem.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HospitalAppointmentSystem.Application.Features.Doctors.Queries.GetAvailableTimeSlots;

/// <summary>
/// Handler for GetAvailableTimeSlotsQuery
/// </summary>
public class GetAvailableTimeSlotsQueryHandler : IRequestHandler<GetAvailableTimeSlotsQuery, Result<List<TimeSlotDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<GetAvailableTimeSlotsQueryHandler> _logger;

    public GetAvailableTimeSlotsQueryHandler(
        IApplicationDbContext context,
        ILogger<GetAvailableTimeSlotsQueryHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Result<List<TimeSlotDto>>> Handle(
        GetAvailableTimeSlotsQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            // 1. Validate date (must be in the future)
            if (request.Date.Date < DateTime.Today)
            {
                return Result<List<TimeSlotDto>>.Failure("Cannot get availability for past dates");
            }

            // 2. Check if doctor exists
            var doctorExists = await _context.Doctors
                .AnyAsync(d => d.Id == request.DoctorId, cancellationToken);

            if (!doctorExists)
            {
                _logger.LogWarning("Doctor not found with ID: {DoctorId}", request.DoctorId);
                return Result<List<TimeSlotDto>>.Failure("Doctor not found");
            }

            // 3. Get doctor's availability for this day of week
            var dayOfWeek = request.Date.DayOfWeek;
            var availability = await _context.Availabilities
                .Where(a => a.DoctorId == request.DoctorId
                    && a.DayOfWeek == dayOfWeek
                    && a.IsActive)
                .FirstOrDefaultAsync(cancellationToken);

            if (availability == null)
            {
                _logger.LogInformation(
                    "No availability found for Doctor {DoctorId} on {DayOfWeek}",
                    request.DoctorId,
                    dayOfWeek);

                return Result<List<TimeSlotDto>>.Success(new List<TimeSlotDto>());
            }

            // 4. Get existing appointments for this date
            var existingAppointments = await _context.Appointments
                .Where(a => a.DoctorId == request.DoctorId
                    && a.ScheduledDate.Date == request.Date.Date
                    && (a.Status == AppointmentStatus.Scheduled
                        || a.Status == AppointmentStatus.Confirmed
                        || a.Status == AppointmentStatus.InProgress))
                .Select(a => new { a.StartTime, a.EndTime })
                .ToListAsync(cancellationToken);

            // 5. Generate time slots based on availability
            var timeSlots = new List<TimeSlotDto>();
            var currentTime = availability.StartTime;
            var slotDuration = TimeSpan.FromMinutes(availability.SlotDurationMinutes);

            while (currentTime.Add(slotDuration) <= availability.EndTime)
            {
                var slotEndTime = currentTime.Add(slotDuration);

                // Check if this slot conflicts with any existing appointment
                var isBooked = existingAppointments.Any(apt =>
                    currentTime < apt.EndTime && slotEndTime > apt.StartTime);

                // If it's today, also check if the slot is in the past
                var isPast = request.Date.Date == DateTime.Today &&
                             currentTime < TimeSpan.FromHours(DateTime.Now.Hour)
                                             .Add(TimeSpan.FromMinutes(DateTime.Now.Minute));

                var isAvailable = !isBooked && !isPast;

                timeSlots.Add(new TimeSlotDto
                {
                    StartTime = currentTime,
                    EndTime = slotEndTime,
                    StartTimeFormatted = currentTime.ToString(@"hh\:mm"),
                    EndTimeFormatted = slotEndTime.ToString(@"hh\:mm"),
                    IsAvailable = isAvailable,
                    DisplayText = $"{currentTime:hh\\:mm} - {slotEndTime:hh\\:mm}"
                });

                currentTime = slotEndTime;
            }

            _logger.LogInformation(
                "Generated {TotalSlots} time slots for Doctor {DoctorId} on {Date} ({AvailableSlots} available)",
                timeSlots.Count,
                request.DoctorId,
                request.Date.Date,
                timeSlots.Count(s => s.IsAvailable));

            return Result<List<TimeSlotDto>>.Success(timeSlots);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error retrieving available time slots for Doctor {DoctorId} on {Date}",
                request.DoctorId,
                request.Date);

            return Result<List<TimeSlotDto>>.Failure("An error occurred while retrieving available time slots");
        }
    }
}
