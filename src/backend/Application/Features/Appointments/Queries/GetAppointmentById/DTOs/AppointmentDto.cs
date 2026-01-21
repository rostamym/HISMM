namespace HospitalAppointmentSystem.Application.Features.Appointments.Queries.GetAppointmentById.DTOs;

/// <summary>
/// DTO for appointment details
/// </summary>
public class AppointmentDto
{
    public Guid Id { get; set; }
    public DateTime ScheduledDate { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public string Status { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    // Patient information
    public PatientInfoDto Patient { get; set; } = null!;

    // Doctor information
    public DoctorInfoDto Doctor { get; set; } = null!;

    // Formatted display properties
    public string FormattedDate => ScheduledDate.ToString("dddd, MMMM dd, yyyy");
    public string FormattedTime => $"{StartTime:hh\\:mm} - {EndTime:hh\\:mm}";
    public int DurationMinutes => (int)(EndTime - StartTime).TotalMinutes;
}

/// <summary>
/// Patient information in appointment
/// </summary>
public class PatientInfoDto
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string FullName => $"{FirstName} {LastName}";
}

/// <summary>
/// Doctor information in appointment
/// </summary>
public class DoctorInfoDto
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string SpecialtyName { get; set; } = string.Empty;
    public string LicenseNumber { get; set; } = string.Empty;
    public decimal ConsultationFee { get; set; }
    public string FullName => $"Dr. {FirstName} {LastName}";
}
