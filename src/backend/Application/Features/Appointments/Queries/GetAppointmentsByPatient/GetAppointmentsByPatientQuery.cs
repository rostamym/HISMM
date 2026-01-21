using HospitalAppointmentSystem.Application.Common.Models;
using HospitalAppointmentSystem.Application.Features.Appointments.Queries.GetAppointmentById.DTOs;
using MediatR;

namespace HospitalAppointmentSystem.Application.Features.Appointments.Queries.GetAppointmentsByPatient;

/// <summary>
/// Query to get all appointments for a patient
/// </summary>
public record GetAppointmentsByPatientQuery : IRequest<Result<List<AppointmentDto>>>
{
    public Guid PatientId { get; init; }
    public string? StatusFilter { get; init; }
    public bool UpcomingOnly { get; init; }
}
