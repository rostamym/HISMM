using HospitalAppointmentSystem.Domain.Common;

namespace HospitalAppointmentSystem.Domain.Events;

/// <summary>
/// Domain event raised when an appointment is completed
/// </summary>
public class AppointmentCompletedEvent : BaseDomainEvent
{
    public Guid AppointmentId { get; }
    public Guid PatientId { get; }
    public Guid DoctorId { get; }

    public AppointmentCompletedEvent(Guid appointmentId, Guid patientId, Guid doctorId)
    {
        AppointmentId = appointmentId;
        PatientId = patientId;
        DoctorId = doctorId;
    }
}
