namespace HospitalAppointmentSystem.Application.Features.Doctors.Common;

/// <summary>
/// Doctor data transfer object
/// </summary>
public class DoctorDto
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public string LicenseNumber { get; set; } = string.Empty;
    public Guid SpecialtyId { get; set; }
    public string SpecialtyName { get; set; } = string.Empty;
    public string? Biography { get; set; }
    public int YearsOfExperience { get; set; }
    public decimal? ConsultationFee { get; set; }
    public decimal Rating { get; set; }
    public bool IsAvailable { get; set; }
}

/// <summary>
/// Detailed doctor information including availability
/// </summary>
public class DoctorDetailDto : DoctorDto
{
    public List<AvailabilityDto> Availabilities { get; set; } = new();
    public int TotalAppointments { get; set; }
    public int CompletedAppointments { get; set; }
}

/// <summary>
/// Availability time slot
/// </summary>
public class AvailabilityDto
{
    public Guid Id { get; set; }
    public DayOfWeek DayOfWeek { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public bool IsAvailable { get; set; }
}

/// <summary>
/// Paginated list of doctors
/// </summary>
public class PaginatedDoctorsDto
{
    public List<DoctorDto> Items { get; set; } = new();
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    public bool HasNextPage => PageNumber < TotalPages;
    public bool HasPreviousPage => PageNumber > 1;
}
