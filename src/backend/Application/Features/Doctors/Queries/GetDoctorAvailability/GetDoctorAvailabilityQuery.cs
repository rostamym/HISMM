using MediatR;
using HospitalAppointmentSystem.Application.Common.Models;
using HospitalAppointmentSystem.Application.Features.Doctors.Queries.GetDoctorAvailability.DTOs;

namespace HospitalAppointmentSystem.Application.Features.Doctors.Queries.GetDoctorAvailability;

/// <summary>
/// Query to get all availability schedules for a doctor
/// </summary>
public class GetDoctorAvailabilityQuery : IRequest<Result<List<DoctorAvailabilityDto>>>
{
    public Guid DoctorId { get; set; }
}
