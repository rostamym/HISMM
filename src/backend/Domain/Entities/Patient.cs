using HospitalAppointmentSystem.Domain.Common;

namespace HospitalAppointmentSystem.Domain.Entities;

/// <summary>
/// Patient entity
/// </summary>
public class Patient : BaseEntity
{
    public Guid UserId { get; private set; }
    public string? MedicalRecordNumber { get; private set; }
    public string? BloodGroup { get; private set; }
    public string? Allergies { get; private set; }

    // Navigation properties
    public User User { get; private set; } = null!;

    private readonly List<Appointment> _appointments = new();
    public IReadOnlyCollection<Appointment> Appointments => _appointments.AsReadOnly();

    // For EF Core
    private Patient() { }

    private Patient(Guid userId)
    {
        UserId = userId;
    }

    public static Patient Create(Guid userId)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("User ID is required", nameof(userId));

        return new Patient(userId);
    }

    public void UpdateMedicalInfo(string? bloodGroup, string? allergies)
    {
        BloodGroup = bloodGroup;
        Allergies = allergies;
        SetUpdated();
    }

    public void SetMedicalRecordNumber(string medicalRecordNumber)
    {
        if (string.IsNullOrWhiteSpace(medicalRecordNumber))
            throw new ArgumentException("Medical record number is required", nameof(medicalRecordNumber));

        MedicalRecordNumber = medicalRecordNumber;
        SetUpdated();
    }
}
