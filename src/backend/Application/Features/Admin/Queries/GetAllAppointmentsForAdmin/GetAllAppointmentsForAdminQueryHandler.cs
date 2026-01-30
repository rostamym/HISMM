using HospitalAppointmentSystem.Application.Common.Interfaces;
using HospitalAppointmentSystem.Application.Common.Models;
using HospitalAppointmentSystem.Application.Features.Appointments.Queries.GetAppointmentById.DTOs;
using HospitalAppointmentSystem.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace HospitalAppointmentSystem.Application.Features.Admin.Queries.GetAllAppointmentsForAdmin;

public class GetAllAppointmentsForAdminQueryHandler : IRequestHandler<GetAllAppointmentsForAdminQuery, Result<List<AppointmentDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<GetAllAppointmentsForAdminQueryHandler> _logger;

    public GetAllAppointmentsForAdminQueryHandler(
        IApplicationDbContext context,
        ILogger<GetAllAppointmentsForAdminQueryHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Result<List<AppointmentDto>>> Handle(GetAllAppointmentsForAdminQuery request, CancellationToken cancellationToken)
    {
        var stopwatch = Stopwatch.StartNew();
        _logger.LogInformation(
            "Admin GetAllAppointments Query Started - Filters: FromDate={FromDate}, ToDate={ToDate}, Status={Status}, PatientId={PatientId}, DoctorId={DoctorId}, SearchTerm={SearchTerm}",
            request.FromDate, request.ToDate, request.StatusFilter, request.PatientId, request.DoctorId, request.SearchTerm);

        try
        {
            // Start with base query
            var query = _context.Appointments
                .Include(a => a.Patient)
                    .ThenInclude(p => p.User)
                .Include(a => a.Doctor)
                    .ThenInclude(d => d.User)
                .Include(a => a.Doctor)
                    .ThenInclude(d => d.Specialty)
                .AsQueryable();

            // Apply date range filter
            if (request.FromDate.HasValue)
            {
                query = query.Where(a => a.ScheduledDate >= request.FromDate.Value.Date);
            }

            if (request.ToDate.HasValue)
            {
                query = query.Where(a => a.ScheduledDate <= request.ToDate.Value.Date);
            }

            // Apply status filter
            if (!string.IsNullOrWhiteSpace(request.StatusFilter))
            {
                query = query.Where(a => a.Status.ToString() == request.StatusFilter);
            }

            // Apply patient filter
            if (request.PatientId.HasValue)
            {
                query = query.Where(a => a.PatientId == request.PatientId.Value);
            }

            // Apply doctor filter
            if (request.DoctorId.HasValue)
            {
                query = query.Where(a => a.DoctorId == request.DoctorId.Value);
            }

            // Apply search term filter (searches patient and doctor names)
            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                var searchTerm = request.SearchTerm.ToLower().Trim();
                query = query.Where(a =>
                    a.Patient.User.FirstName.ToLower().Contains(searchTerm) ||
                    a.Patient.User.LastName.ToLower().Contains(searchTerm) ||
                    a.Doctor.User.FirstName.ToLower().Contains(searchTerm) ||
                    a.Doctor.User.LastName.ToLower().Contains(searchTerm));
            }

            // Order by most recent first
            query = query.OrderByDescending(a => a.ScheduledDate)
                         .ThenByDescending(a => a.StartTime);

            // Execute query and map to DTOs
            var appointmentDtos = await query
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

            stopwatch.Stop();
            _logger.LogInformation(
                "Admin GetAllAppointments Query Completed - Duration: {Duration}ms, Count: {Count}",
                stopwatch.ElapsedMilliseconds, appointmentDtos.Count);

            return Result<List<AppointmentDto>>.Success(appointmentDtos);
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _logger.LogError(ex,
                "Admin GetAllAppointments Query Failed - Duration: {Duration}ms, Error: {ErrorMessage}",
                stopwatch.ElapsedMilliseconds, ex.Message);

            return Result<List<AppointmentDto>>.Failure($"Failed to retrieve appointments: {ex.Message}");
        }
    }
}
