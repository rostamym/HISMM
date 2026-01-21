using HospitalAppointmentSystem.Application.Common.Interfaces;
using HospitalAppointmentSystem.Application.Common.Models;
using HospitalAppointmentSystem.Application.Features.Appointments.Queries.GetAppointmentById.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HospitalAppointmentSystem.Application.Features.Appointments.Queries.GetAppointmentById;

/// <summary>
/// Handler for GetAppointmentByIdQuery
/// </summary>
public class GetAppointmentByIdQueryHandler : IRequestHandler<GetAppointmentByIdQuery, Result<AppointmentDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<GetAppointmentByIdQueryHandler> _logger;

    public GetAppointmentByIdQueryHandler(
        IApplicationDbContext context,
        ILogger<GetAppointmentByIdQueryHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Result<AppointmentDto>> Handle(
        GetAppointmentByIdQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            var appointment = await _context.Appointments
                .Include(a => a.Patient)
                    .ThenInclude(p => p.User)
                .Include(a => a.Doctor)
                    .ThenInclude(d => d.User)
                .Include(a => a.Doctor)
                    .ThenInclude(d => d.Specialty)
                .Where(a => a.Id == request.AppointmentId)
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
                .FirstOrDefaultAsync(cancellationToken);

            if (appointment == null)
            {
                _logger.LogWarning("Appointment not found with ID: {AppointmentId}", request.AppointmentId);
                return Result<AppointmentDto>.Failure("Appointment not found");
            }

            _logger.LogInformation("Retrieved appointment: {AppointmentId}", request.AppointmentId);
            return Result<AppointmentDto>.Success(appointment);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving appointment: {AppointmentId}", request.AppointmentId);
            return Result<AppointmentDto>.Failure($"Failed to retrieve appointment: {ex.Message}");
        }
    }
}
