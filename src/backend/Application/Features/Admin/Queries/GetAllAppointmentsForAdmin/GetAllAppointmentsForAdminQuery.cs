using HospitalAppointmentSystem.Application.Common.Models;
using HospitalAppointmentSystem.Application.Features.Appointments.Queries.GetAppointmentById.DTOs;
using MediatR;

namespace HospitalAppointmentSystem.Application.Features.Admin.Queries.GetAllAppointmentsForAdmin;

public record GetAllAppointmentsForAdminQuery : IRequest<Result<List<AppointmentDto>>>
{
    public DateTime? FromDate { get; init; }
    public DateTime? ToDate { get; init; }
    public string? StatusFilter { get; init; }
    public Guid? PatientId { get; init; }
    public Guid? DoctorId { get; init; }
    public string? SearchTerm { get; init; }  // Patient/Doctor name search
}
