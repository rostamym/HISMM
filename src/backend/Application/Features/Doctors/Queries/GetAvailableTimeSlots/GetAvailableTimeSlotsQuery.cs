using MediatR;
using HospitalAppointmentSystem.Application.Common.Models;
using HospitalAppointmentSystem.Application.Features.Doctors.Queries.GetAvailableTimeSlots.DTOs;

namespace HospitalAppointmentSystem.Application.Features.Doctors.Queries.GetAvailableTimeSlots;

/// <summary>
/// Query to get available time slots for a doctor on a specific date
/// </summary>
public class GetAvailableTimeSlotsQuery : IRequest<Result<List<TimeSlotDto>>>
{
    public Guid DoctorId { get; set; }
    public DateTime Date { get; set; }
}
