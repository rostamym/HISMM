using HospitalAppointmentSystem.Domain.Common;
using HospitalAppointmentSystem.Domain.Enums;
using HospitalAppointmentSystem.Domain.Events;

namespace HospitalAppointmentSystem.Domain.Entities;

/// <summary>
/// Appointment entity
/// </summary>
public class Appointment : BaseEntity
{
    public Guid PatientId { get; private set; }
    public Guid DoctorId { get; private set; }
    public DateTime ScheduledDate { get; private set; }
    public TimeSpan StartTime { get; private set; }
    public TimeSpan EndTime { get; private set; }
    public AppointmentStatus Status { get; private set; }
    public string Reason { get; private set; } = null!;
    public string? Notes { get; private set; }

    // Navigation properties
    public Patient Patient { get; private set; } = null!;
    public Doctor Doctor { get; private set; } = null!;

    // For EF Core
    private Appointment() { }

    private Appointment(Guid patientId, Guid doctorId, DateTime scheduledDate, TimeSpan startTime, TimeSpan endTime, string reason)
    {
        PatientId = patientId;
        DoctorId = doctorId;
        ScheduledDate = scheduledDate;
        StartTime = startTime;
        EndTime = endTime;
        Status = AppointmentStatus.Scheduled;
        Reason = reason;
    }

    public static Appointment Create(Guid patientId, Guid doctorId, DateTime scheduledDate, TimeSpan startTime, TimeSpan endTime, string reason)
    {
        // Validation
        if (patientId == Guid.Empty)
            throw new ArgumentException("Patient ID is required", nameof(patientId));

        if (doctorId == Guid.Empty)
            throw new ArgumentException("Doctor ID is required", nameof(doctorId));

        if (scheduledDate.Date < DateTime.UtcNow.Date)
            throw new ArgumentException("Cannot schedule appointments in the past", nameof(scheduledDate));

        if (startTime >= endTime)
            throw new ArgumentException("Start time must be before end time", nameof(startTime));

        if (string.IsNullOrWhiteSpace(reason))
            throw new ArgumentException("Reason is required", nameof(reason));

        var appointment = new Appointment(patientId, doctorId, scheduledDate, startTime, endTime, reason);

        // Raise domain event
        appointment.AddDomainEvent(new AppointmentBookedEvent(appointment.Id, patientId, doctorId, scheduledDate, startTime, endTime));

        return appointment;
    }

    public void Confirm()
    {
        if (Status != AppointmentStatus.Scheduled)
            throw new InvalidOperationException("Only scheduled appointments can be confirmed");

        Status = AppointmentStatus.Confirmed;
        SetUpdated();
    }

    public void Cancel(string? cancellationReason = null)
    {
        if (Status == AppointmentStatus.Completed || Status == AppointmentStatus.Cancelled)
            throw new InvalidOperationException("Cannot cancel a completed or already cancelled appointment");

        Status = AppointmentStatus.Cancelled;
        if (!string.IsNullOrWhiteSpace(cancellationReason))
        {
            Notes = $"Cancellation Reason: {cancellationReason}";
        }
        SetUpdated();

        // Raise domain event
        AddDomainEvent(new AppointmentCancelledEvent(Id, PatientId, DoctorId));
    }

    public void Complete(string? notes = null)
    {
        if (Status == AppointmentStatus.Cancelled)
            throw new InvalidOperationException("Cannot complete a cancelled appointment");

        if (Status == AppointmentStatus.Completed)
            throw new InvalidOperationException("Appointment is already completed");

        if (Status != AppointmentStatus.Scheduled &&
            Status != AppointmentStatus.Confirmed &&
            Status != AppointmentStatus.InProgress)
            throw new InvalidOperationException("Only scheduled, confirmed, or in-progress appointments can be completed");

        Status = AppointmentStatus.Completed;

        if (!string.IsNullOrWhiteSpace(notes))
        {
            Notes = string.IsNullOrWhiteSpace(Notes)
                ? notes
                : $"{Notes}\n\nCompletion Notes: {notes}";
        }

        SetUpdated();

        // Raise domain event
        AddDomainEvent(new AppointmentCompletedEvent(Id, PatientId, DoctorId));
    }

    public void MarkAsInProgress()
    {
        if (Status != AppointmentStatus.Confirmed)
            throw new InvalidOperationException("Only confirmed appointments can be marked as in progress");

        Status = AppointmentStatus.InProgress;
        SetUpdated();
    }

    public void MarkAsNoShow()
    {
        if (Status != AppointmentStatus.Confirmed)
            throw new InvalidOperationException("Only confirmed appointments can be marked as no-show");

        Status = AppointmentStatus.NoShow;
        SetUpdated();
    }

    public void Reschedule(DateTime newScheduledDate, TimeSpan newStartTime, TimeSpan newEndTime)
    {
        if (Status == AppointmentStatus.Completed || Status == AppointmentStatus.Cancelled)
            throw new InvalidOperationException("Cannot reschedule a completed or cancelled appointment");

        if (newScheduledDate.Date < DateTime.UtcNow.Date)
            throw new ArgumentException("Cannot reschedule to a past date", nameof(newScheduledDate));

        if (newStartTime >= newEndTime)
            throw new ArgumentException("Start time must be before end time", nameof(newStartTime));

        ScheduledDate = newScheduledDate;
        StartTime = newStartTime;
        EndTime = newEndTime;
        Status = AppointmentStatus.Scheduled;
        SetUpdated();
    }

    public void AddNotes(string notes)
    {
        Notes = notes;
        SetUpdated();
    }
}
