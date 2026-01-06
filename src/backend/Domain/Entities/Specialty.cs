using HospitalAppointmentSystem.Domain.Common;

namespace HospitalAppointmentSystem.Domain.Entities;

/// <summary>
/// Medical specialty entity
/// </summary>
public class Specialty : BaseEntity
{
    public string Name { get; private set; } = null!;
    public string? Description { get; private set; }
    public bool IsActive { get; private set; }

    // Navigation properties
    private readonly List<Doctor> _doctors = new();
    public IReadOnlyCollection<Doctor> Doctors => _doctors.AsReadOnly();

    // For EF Core
    private Specialty() { }

    private Specialty(string name, string? description)
    {
        Name = name;
        Description = description;
        IsActive = true;
    }

    public static Specialty Create(string name, string? description = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Specialty name is required", nameof(name));

        return new Specialty(name, description);
    }

    public void Update(string name, string? description)
    {
        Name = name;
        Description = description;
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
}
