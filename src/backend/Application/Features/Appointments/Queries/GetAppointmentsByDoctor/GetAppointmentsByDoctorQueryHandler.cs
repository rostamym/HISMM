using HospitalAppointmentSystem.Application.Common.Interfaces;
using HospitalAppointmentSystem.Application.Common.Models;
using HospitalAppointmentSystem.Application.Features.Appointments.Queries.GetAppointmentById.DTOs;
using HospitalAppointmentSystem.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HospitalAppointmentSystem.Application.Features.Appointments.Queries.GetAppointmentsByDoctor;

/// <summary>
/// Handler for GetAppointmentsByDoctorQuery
/// </summary>
public class GetAppointmentsByDoctorQueryHandler
    : IRequestHandler<GetAppointmentsByDoctorQuery, Result<List<AppointmentDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<GetAppointmentsByDoctorQueryHandler> _logger;

    public GetAppointmentsByDoctorQueryHandler(
        IApplicationDbContext context,
        ILogger<GetAppointmentsByDoctorQueryHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Result<List<AppointmentDto>>> Handle(
        GetAppointmentsByDoctorQuery request,
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
                return Result<List<AppointmentDto>>.Failure("Doctor not found");
            }

            var query = _context.Appointments
                .Include(a => a.Patient)
                    .ThenInclude(p => p.User)
                .Include(a => a.Doctor)
                    .ThenInclude(d => d.User)
                .Include(a => a.Doctor)
                    .ThenInclude(d => d.Specialty)
                .Where(a => a.DoctorId == request.DoctorId);

            // Apply date range filter
            if (request.FromDate.HasValue)
            {
                query = query.Where(a => a.ScheduledDate.Date >= request.FromDate.Value.Date);
            }

            if (request.ToDate.HasValue)
            {
                query = query.Where(a => a.ScheduledDate.Date <= request.ToDate.Value.Date);
            }

            // Apply status filter if provided
            if (!string.IsNullOrEmpty(request.StatusFilter)
                && Enum.TryParse<AppointmentStatus>(request.StatusFilter, true, out var status))
            {
                query = query.Where(a => a.Status == status);
            }

            var appointments = await query
                .OrderBy(a => a.ScheduledDate)
                .ThenBy(a => a.StartTime)
                .Select(a => new AppointmentDto
                {
                    Id = a.Id,
                    ScheduledDate = a.ScheduledDate,
                    StartTime = a.StartTime,
                    EndTime = a.EndTime,
                    Status = a.Status.ToString(),
                    Reason = a.Reason,
                    Notes = a.Notes,
                    CreatedAt = a.CreatedAt,
                    UpdatedAt = a.UpdatedAt,
                    Patient = new PatientInfoDto
                    {
                        Id = a.Patient.Id,
                        FirstName = a.Patient.User.FirstName,
                        LastName = a.Patient.User.LastName,
                        Email = a.Patient.User.Email,
                        PhoneNumber = a.Patient.User.PhoneNumber
                    },
                    Doctor = new DoctorInfoDto
                    {
                        Id = a.Doctor.Id,
                        FirstName = a.Doctor.User.FirstName,
                        LastName = a.Doctor.User.LastName,
                        Email = a.Doctor.User.Email,
                        PhoneNumber = a.Doctor.User.PhoneNumber,
                        SpecialtyName = a.Doctor.Specialty != null ? a.Doctor.Specialty.Name : "General",
                        LicenseNumber = a.Doctor.LicenseNumber,
                        ConsultationFee = a.Doctor.ConsultationFee ?? 0
                    }
                })
                .ToListAsync(cancellationToken);

            _logger.LogInformation(
                "Retrieved {Count} appointments for Doctor {DoctorId}",
                appointments.Count,
                request.DoctorId);

            return Result<List<AppointmentDto>>.Success(appointments);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error retrieving appointments for Doctor {DoctorId}",
                request.DoctorId);

            return Result<List<AppointmentDto>>.Failure($"Failed to retrieve appointments: {ex.Message}");
        }
    }
}
