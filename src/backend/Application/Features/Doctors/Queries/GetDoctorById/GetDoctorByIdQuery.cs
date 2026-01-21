using HospitalAppointmentSystem.Application.Common.Models;
using HospitalAppointmentSystem.Application.Features.Doctors.Common;
using MediatR;

namespace HospitalAppointmentSystem.Application.Features.Doctors.Queries.GetDoctorById;

/// <summary>
/// Query to get detailed doctor information by ID
/// </summary>
public class GetDoctorByIdQuery : IRequest<Result<DoctorDetailDto>>
{
    public Guid DoctorId { get; set; }
}
