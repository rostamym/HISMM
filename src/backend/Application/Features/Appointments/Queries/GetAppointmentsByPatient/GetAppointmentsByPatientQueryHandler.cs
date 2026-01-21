using HospitalAppointmentSystem.Application.Common.Interfaces;
using HospitalAppointmentSystem.Application.Common.Models;
using HospitalAppointmentSystem.Application.Features.Appointments.Queries.GetAppointmentById.DTOs;
using HospitalAppointmentSystem.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HospitalAppointmentSystem.Application.Features.Appointments.Queries.GetAppointmentsByPatient;

/// <summary>
/// Handler for GetAppointmentsByPatientQuery
/// </summary>
public class GetAppointmentsByPatientQueryHandler
    : IRequestHandler<GetAppointmentsByPatientQuery, Result<List<AppointmentDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<GetAppointmentsByPatientQueryHandler> _logger;

    public GetAppointmentsByPatientQueryHandler(
        IApplicationDbContext context,
        ILogger<GetAppointmentsByPatientQueryHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Result<List<AppointmentDto>>> Handle(
        GetAppointmentsByPatientQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            // Check if patient exists
            var patientExists = await _context.Patients
                .AnyAsync(p => p.Id == request.PatientId, cancellationToken);

            if (!patientExists)
            {
                _logger.LogWarning("Patient not found with ID: {PatientId}", request.PatientId);
                return Result<List<AppointmentDto>>.Failure("Patient not found");
            }

            var query = _context.Appointments
                .Include(a => a.Patient)
                    .ThenInclude(p => p.User)
                .Include(a => a.Doctor)
                    .ThenInclude(d => d.User)
                .Include(a => a.Doctor)
                    .ThenInclude(d => d.Specialty)
                .Where(a => a.PatientId == request.PatientId);

            // Apply status filter if provided
            if (!string.IsNullOrEmpty(request.StatusFilter)
                && Enum.TryParse<AppointmentStatus>(request.StatusFilter, true, out var status))
            {
                query = query.Where(a => a.Status == status);
            }

            // Apply upcoming filter if requested
            if (request.UpcomingOnly)
            {
                var today = DateTime.Today;
                var now = DateTime.Now.TimeOfDay;

                query = query.Where(a =>
                    a.ScheduledDate.Date > today ||
                    (a.ScheduledDate.Date == today && a.StartTime > now));
            }

            var appointments = await query
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

            _logger.LogInformation(
                "Retrieved {Count} appointments for Patient {PatientId}",
                appointments.Count,
                request.PatientId);

            return Result<List<AppointmentDto>>.Success(appointments);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error retrieving appointments for Patient {PatientId}",
                request.PatientId);

            return Result<List<AppointmentDto>>.Failure($"Failed to retrieve appointments: {ex.Message}");
        }
    }
}
