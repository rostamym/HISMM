using HospitalAppointmentSystem.Domain.Common;
using HospitalAppointmentSystem.Domain.Enums;

namespace HospitalAppointmentSystem.Domain.Entities;

/// <summary>
/// User entity - base for Patient, Doctor, and Administrator
/// </summary>
public class User : BaseEntity
{
    public string Email { get; private set; } = null!;
    public string FirstName { get; private set; } = null!;
    public string LastName { get; private set; } = null!;
    public string? PhoneNumber { get; private set; }
    public DateTime DateOfBirth { get; private set; }
    public UserRole Role { get; private set; }
    public bool IsActive { get; private set; }

    // For EF Core
    private User() { }

    private User(string email, string firstName, string lastName, DateTime dateOfBirth, UserRole role)
    {
        Email = email;
        FirstName = firstName;
        LastName = lastName;
        DateOfBirth = dateOfBirth;
        Role = role;
        IsActive = true;
    }

    public static User Create(string email, string firstName, string lastName, DateTime dateOfBirth, UserRole role)
    {
        // Validation
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email is required", nameof(email));

        if (string.IsNullOrWhiteSpace(firstName))
            throw new ArgumentException("First name is required", nameof(firstName));

        if (string.IsNullOrWhiteSpace(lastName))
            throw new ArgumentException("Last name is required", nameof(lastName));

        if (dateOfBirth >= DateTime.UtcNow)
            throw new ArgumentException("Date of birth must be in the past", nameof(dateOfBirth));

        return new User(email, firstName, lastName, dateOfBirth, role);
    }

    public void UpdateProfile(string firstName, string lastName, string? phoneNumber)
    {
        FirstName = firstName;
        LastName = lastName;
        PhoneNumber = phoneNumber;
        SetUpdated();
    }

    public void SetPhoneNumber(string? phoneNumber)
    {
        PhoneNumber = phoneNumber;
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

    public string GetFullName() => $"{FirstName} {LastName}";
}
