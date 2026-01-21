using HospitalAppointmentSystem.Application.Common.Models;
using HospitalAppointmentSystem.Application.Features.Appointments.Queries.GetAppointmentById.DTOs;
using MediatR;

namespace HospitalAppointmentSystem.Application.Features.Appointments.Queries.GetAppointmentsByDoctor;

/// <summary>
/// Query to get all appointments for a doctor
/// </summary>
public record GetAppointmentsByDoctorQuery : IRequest<Result<List<AppointmentDto>>>
{
    public Guid DoctorId { get; init; }
    public DateTime? FromDate { get; init; }
    public DateTime? ToDate { get; init; }
    public string? StatusFilter { get; init; }
}
