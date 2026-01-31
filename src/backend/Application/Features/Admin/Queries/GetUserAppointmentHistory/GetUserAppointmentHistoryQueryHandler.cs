using HospitalAppointmentSystem.Application.Common.Interfaces;
using HospitalAppointmentSystem.Application.Common.Models;
using HospitalAppointmentSystem.Application.Features.Appointments.Queries.GetAppointmentById.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace HospitalAppointmentSystem.Application.Features.Admin.Queries.GetUserAppointmentHistory;

/// <summary>
/// Handler for GetUserAppointmentHistoryQuery
/// </summary>
public class GetUserAppointmentHistoryQueryHandler : IRequestHandler<GetUserAppointmentHistoryQuery, Result<List<AppointmentDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<GetUserAppointmentHistoryQueryHandler> _logger;

    public GetUserAppointmentHistoryQueryHandler(
        IApplicationDbContext context,
        ILogger<GetUserAppointmentHistoryQueryHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Result<List<AppointmentDto>>> Handle(
        GetUserAppointmentHistoryQuery request,
        CancellationToken cancellationToken)
    {
        var stopwatch = Stopwatch.StartNew();
        _logger.LogInformation(
            "Getting appointment history for user: {UserId}",
            request.UserId);

        try
        {
            // Find the user
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);

            if (user == null)
            {
                _logger.LogWarning("User not found: {UserId}", request.UserId);
                return Result<List<AppointmentDto>>.Failure("User not found");
            }

            List<AppointmentDto> appointments;

            // Check if user is a patient
            var patient = await _context.Patients
                .FirstOrDefaultAsync(p => p.UserId == request.UserId, cancellationToken);

            if (patient != null)
            {
                _logger.LogInformation("Fetching appointments for patient: {UserId}", request.UserId);

                appointments = await _context.Appointments
                    .Include(a => a.Patient)
                        .ThenInclude(p => p.User)
                    .Include(a => a.Doctor)
                        .ThenInclude(d => d.User)
                    .Include(a => a.Doctor)
                        .ThenInclude(d => d.Specialty)
                    .Where(a => a.PatientId == patient.Id)
                    .OrderByDescending(a => a.ScheduledDate)
                    .ThenByDescending(a => a.StartTime)
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
            }
            else
            {
                // Check if user is a doctor
                var doctor = await _context.Doctors
                    .FirstOrDefaultAsync(d => d.UserId == request.UserId, cancellationToken);

                if (doctor != null)
                {
                    _logger.LogInformation("Fetching appointments for doctor: {UserId}", request.UserId);

                    appointments = await _context.Appointments
                    .Include(a => a.Patient)
                        .ThenInclude(p => p.User)
                    .Include(a => a.Doctor)
                        .ThenInclude(d => d.User)
                    .Include(a => a.Doctor)
                        .ThenInclude(d => d.Specialty)
                    .Where(a => a.DoctorId == doctor.Id)
                    .OrderByDescending(a => a.ScheduledDate)
                    .ThenByDescending(a => a.StartTime)
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
                }
                else
                {
                    _logger.LogInformation("User is admin (no appointments): {UserId}", request.UserId);
                    appointments = new List<AppointmentDto>();
                }
            }

            stopwatch.Stop();
            _logger.LogInformation(
                "Retrieved {Count} appointments for user {UserId} in {Duration}ms",
                appointments.Count, request.UserId, stopwatch.ElapsedMilliseconds);

            return Result<List<AppointmentDto>>.Success(appointments);
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _logger.LogError(ex,
                "Error retrieving appointment history for user {UserId} - Duration: {Duration}ms",
                request.UserId, stopwatch.ElapsedMilliseconds);

            return Result<List<AppointmentDto>>.Failure($"Failed to retrieve appointment history: {ex.Message}");
        }
    }
}
