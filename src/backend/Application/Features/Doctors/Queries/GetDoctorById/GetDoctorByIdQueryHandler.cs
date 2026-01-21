using HospitalAppointmentSystem.Application.Common.Interfaces;
using HospitalAppointmentSystem.Application.Common.Models;
using HospitalAppointmentSystem.Application.Features.Doctors.Common;
using HospitalAppointmentSystem.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HospitalAppointmentSystem.Application.Features.Doctors.Queries.GetDoctorById;

/// <summary>
/// Handler for getting detailed doctor information
/// </summary>
public class GetDoctorByIdQueryHandler : IRequestHandler<GetDoctorByIdQuery, Result<DoctorDetailDto>>
{
    private readonly IApplicationDbContext _context;

    public GetDoctorByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<DoctorDetailDto>> Handle(GetDoctorByIdQuery request, CancellationToken cancellationToken)
    {
        var doctor = await _context.Doctors
            .Include(d => d.User)
            .Include(d => d.Specialty)
            .Include(d => d.Availabilities)
            .Include(d => d.Appointments)
            .FirstOrDefaultAsync(d => d.Id == request.DoctorId, cancellationToken);

        if (doctor == null)
        {
            return Result<DoctorDetailDto>.Failure("Doctor not found");
        }

        // Calculate statistics
        var totalAppointments = doctor.Appointments.Count;
        var completedAppointments = doctor.Appointments.Count(a => a.Status == AppointmentStatus.Completed);

        // Map to DTO
        var doctorDto = new DoctorDetailDto
        {
            Id = doctor.Id,
            FirstName = doctor.User.FirstName,
            LastName = doctor.User.LastName,
            Email = doctor.User.Email,
            PhoneNumber = doctor.User.PhoneNumber,
            LicenseNumber = doctor.LicenseNumber,
            SpecialtyId = doctor.SpecialtyId,
            SpecialtyName = doctor.Specialty.Name,
            Biography = doctor.Biography,
            YearsOfExperience = doctor.YearsOfExperience,
            ConsultationFee = doctor.ConsultationFee,
            Rating = doctor.Rating,
            IsAvailable = doctor.Availabilities.Any(a => a.IsActive),
            TotalAppointments = totalAppointments,
            CompletedAppointments = completedAppointments,
            Availabilities = doctor.Availabilities.Select(a => new AvailabilityDto
            {
                Id = a.Id,
                DayOfWeek = a.DayOfWeek,
                StartTime = a.StartTime,
                EndTime = a.EndTime,
                IsAvailable = a.IsActive
            }).ToList()
        };

        return Result<DoctorDetailDto>.Success(doctorDto);
    }
}
