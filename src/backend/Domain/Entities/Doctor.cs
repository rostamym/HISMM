using HospitalAppointmentSystem.Domain.Common;

namespace HospitalAppointmentSystem.Domain.Entities;

/// <summary>
/// Doctor entity
/// </summary>
public class Doctor : BaseEntity
{
    public Guid UserId { get; private set; }
    public string LicenseNumber { get; private set; } = null!;
    public Guid SpecialtyId { get; private set; }
    public string? Biography { get; private set; }
    public int YearsOfExperience { get; private set; }
    public decimal? ConsultationFee { get; private set; }
    public decimal Rating { get; private set; }

    // Navigation properties
    public User User { get; private set; } = null!;
    public Specialty Specialty { get; private set; } = null!;

    private readonly List<Availability> _availabilities = new();
    public IReadOnlyCollection<Availability> Availabilities => _availabilities.AsReadOnly();

    private readonly List<Appointment> _appointments = new();
    public IReadOnlyCollection<Appointment> Appointments => _appointments.AsReadOnly();

    // For EF Core
    private Doctor() { }

    private Doctor(Guid userId, string licenseNumber, Guid specialtyId, int yearsOfExperience)
    {
        UserId = userId;
        LicenseNumber = licenseNumber;
        SpecialtyId = specialtyId;
        YearsOfExperience = yearsOfExperience;
        Rating = 0;
    }

    public static Doctor Create(Guid userId, string licenseNumber, Guid specialtyId, int yearsOfExperience)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("User ID is required", nameof(userId));

        if (string.IsNullOrWhiteSpace(licenseNumber))
            throw new ArgumentException("License number is required", nameof(licenseNumber));

        if (specialtyId == Guid.Empty)
            throw new ArgumentException("Specialty ID is required", nameof(specialtyId));

        if (yearsOfExperience < 0)
            throw new ArgumentException("Years of experience cannot be negative", nameof(yearsOfExperience));

        return new Doctor(userId, licenseNumber, specialtyId, yearsOfExperience);
    }

    public void UpdateProfile(string? biography, int yearsOfExperience, decimal? consultationFee)
    {
        Biography = biography;
        YearsOfExperience = yearsOfExperience;
        ConsultationFee = consultationFee;
        SetUpdated();
    }

    public void SetConsultationFee(decimal fee)
    {
        if (fee < 0)
            throw new ArgumentException("Consultation fee cannot be negative", nameof(fee));

        ConsultationFee = fee;
        SetUpdated();
    }

    public void UpdateRating(decimal newRating)
    {
        if (newRating < 0 || newRating > 5)
            throw new ArgumentException("Rating must be between 0 and 5", nameof(newRating));

        Rating = newRating;
        SetUpdated();
    }

    public void AddAvailability(Availability availability)
    {
        _availabilities.Add(availability);
        SetUpdated();
    }
}
