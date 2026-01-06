using HospitalAppointmentSystem.Domain.Common;

namespace HospitalAppointmentSystem.Domain.Events;

/// <summary>
/// Domain event raised when an appointment is booked
/// </summary>
public class AppointmentBookedEvent : BaseDomainEvent
{
    public Guid AppointmentId { get; }
    public Guid PatientId { get; }
    public Guid DoctorId { get; }
    public DateTime ScheduledDate { get; }
    public TimeSpan StartTime { get; }
    public TimeSpan EndTime { get; }

    public AppointmentBookedEvent(Guid appointmentId, Guid patientId, Guid doctorId, DateTime scheduledDate, TimeSpan startTime, TimeSpan endTime)
    {
        AppointmentId = appointmentId;
        PatientId = patientId;
        DoctorId = doctorId;
        ScheduledDate = scheduledDate;
        StartTime = startTime;
        EndTime = endTime;
    }
}
