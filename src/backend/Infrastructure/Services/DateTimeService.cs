using HospitalAppointmentSystem.Application.Common.Interfaces;

namespace HospitalAppointmentSystem.Infrastructure.Services;

/// <summary>
/// DateTime service implementation
/// </summary>
public class DateTimeService : IDateTime
{
    public DateTime UtcNow => DateTime.UtcNow;
    public DateTime Now => DateTime.Now;
}
