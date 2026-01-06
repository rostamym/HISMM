namespace HospitalAppointmentSystem.Domain.Enums;

/// <summary>
/// Status of a notification
/// </summary>
public enum NotificationStatus
{
    Pending = 1,
    Sent = 2,
    Failed = 3,
    Retrying = 4
}
