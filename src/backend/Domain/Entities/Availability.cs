using HospitalAppointmentSystem.Domain.Common;

namespace HospitalAppointmentSystem.Domain.Entities;

/// <summary>
/// Doctor availability entity
/// </summary>
public class Availability : BaseEntity
{
    public Guid DoctorId { get; private set; }
    public DayOfWeek DayOfWeek { get; private set; }
    public TimeSpan StartTime { get; private set; }
    public TimeSpan EndTime { get; private set; }
    public int SlotDurationMinutes { get; private set; }
    public bool IsActive { get; private set; }

    // Navigation properties
    public Doctor Doctor { get; private set; } = null!;

    // For EF Core
    private Availability() { }

    private Availability(Guid doctorId, DayOfWeek dayOfWeek, TimeSpan startTime, TimeSpan endTime, int slotDurationMinutes)
    {
        DoctorId = doctorId;
        DayOfWeek = dayOfWeek;
        StartTime = startTime;
        EndTime = endTime;
        SlotDurationMinutes = slotDurationMinutes;
        IsActive = true;
    }

    public static Availability Create(Guid doctorId, DayOfWeek dayOfWeek, TimeSpan startTime, TimeSpan endTime, int slotDurationMinutes = 30)
    {
        if (doctorId == Guid.Empty)
            throw new ArgumentException("Doctor ID is required", nameof(doctorId));

        if (startTime >= endTime)
            throw new ArgumentException("Start time must be before end time", nameof(startTime));

        if (slotDurationMinutes <= 0 || slotDurationMinutes > 240)
            throw new ArgumentException("Slot duration must be between 1 and 240 minutes", nameof(slotDurationMinutes));

        return new Availability(doctorId, dayOfWeek, startTime, endTime, slotDurationMinutes);
    }

    public void Update(TimeSpan startTime, TimeSpan endTime, int slotDurationMinutes)
    {
        if (startTime >= endTime)
            throw new ArgumentException("Start time must be before end time", nameof(startTime));

        StartTime = startTime;
        EndTime = endTime;
        SlotDurationMinutes = slotDurationMinutes;
        SetUpdated();
    }

    public void Activate()
    {
        IsActive = true;
        SetUpdated();
    }

    public void Deactivate()
    {
        IsActive = false;
        SetUpdated();
    }

    public List<(TimeSpan Start, TimeSpan End)> GetTimeSlots()
    {
        var slots = new List<(TimeSpan, TimeSpan)>();
        var currentTime = StartTime;

        while (currentTime.Add(TimeSpan.FromMinutes(SlotDurationMinutes)) <= EndTime)
        {
            var slotEnd = currentTime.Add(TimeSpan.FromMinutes(SlotDurationMinutes));
            slots.Add((currentTime, slotEnd));
            currentTime = slotEnd;
        }

        return slots;
    }
}
