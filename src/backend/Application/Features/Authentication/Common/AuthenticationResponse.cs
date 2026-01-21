namespace HospitalAppointmentSystem.Application.Features.Authentication.Common;

/// <summary>
/// Authentication response containing tokens and user information
/// </summary>
public class AuthenticationResponse
{
    public string Token { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public UserDto User { get; set; } = null!;
}

/// <summary>
/// User data transfer object
/// </summary>
public class UserDto
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public DateTime? DateOfBirth { get; set; }
    public string Role { get; set; } = string.Empty;
    public Guid? PatientId { get; set; }
    public Guid? DoctorId { get; set; }
}
