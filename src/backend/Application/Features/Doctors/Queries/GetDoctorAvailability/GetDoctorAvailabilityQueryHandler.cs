using HospitalAppointmentSystem.Application.Common.Interfaces;
using HospitalAppointmentSystem.Application.Common.Models;
using HospitalAppointmentSystem.Application.Features.Doctors.Queries.GetDoctorAvailability.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HospitalAppointmentSystem.Application.Features.Doctors.Queries.GetDoctorAvailability;

/// <summary>
/// Handler for GetDoctorAvailabilityQuery
/// </summary>
public class GetDoctorAvailabilityQueryHandler : IRequestHandler<GetDoctorAvailabilityQuery, Result<List<DoctorAvailabilityDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<GetDoctorAvailabilityQueryHandler> _logger;

    public GetDoctorAvailabilityQueryHandler(
        IApplicationDbContext context,
        ILogger<GetDoctorAvailabilityQueryHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Result<List<DoctorAvailabilityDto>>> Handle(
        GetDoctorAvailabilityQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            // Check if doctor exists
            var doctorExists = await _context.Doctors
                .AnyAsync(d => d.Id == request.DoctorId, cancellationToken);

            if (!doctorExists)
            {
                _logger.LogWarning("Doctor not found with ID: {DoctorId}", request.DoctorId);
                return Result<List<DoctorAvailabilityDto>>.Failure("Doctor not found");
            }

            // Get all availabilities for the doctor
            var availabilities = await _context.Availabilities
                .Where(a => a.DoctorId == request.DoctorId)
                .OrderBy(a => a.DayOfWeek)
                .ThenBy(a => a.StartTime)
                .Select(a => new DoctorAvailabilityDto
                {
                    Id = a.Id,
                    DoctorId = a.DoctorId,
                    DayOfWeek = a.DayOfWeek,
                    DayOfWeekName = a.DayOfWeek.ToString(),
                    StartTime = a.StartTime,
                    EndTime = a.EndTime,
                    StartTimeFormatted = a.StartTime.ToString(@"hh\:mm"),
                    EndTimeFormatted = a.EndTime.ToString(@"hh\:mm"),
                    SlotDurationMinutes = a.SlotDurationMinutes,
                    IsActive = a.IsActive
                })
                .ToListAsync(cancellationToken);

            _logger.LogInformation(
                "Retrieved {Count} availability schedules for Doctor {DoctorId}",
                availabilities.Count,
                request.DoctorId);

            return Result<List<DoctorAvailabilityDto>>.Success(availabilities);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving availability for Doctor {DoctorId}", request.DoctorId);
            return Result<List<DoctorAvailabilityDto>>.Failure("An error occurred while retrieving availability");
        }
    }
}
