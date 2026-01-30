namespace HospitalAppointmentSystem.Application.Features.Admin.Queries.GetAllUsers.DTOs;

/// <summary>
/// DTO for user list item
/// </summary>
public class UserListDto
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public string Role { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    // Additional info based on role
    public Guid? PatientId { get; set; }
    public Guid? DoctorId { get; set; }
    public string? DoctorSpecialty { get; set; }
    public string? DoctorLicenseNumber { get; set; }
}
