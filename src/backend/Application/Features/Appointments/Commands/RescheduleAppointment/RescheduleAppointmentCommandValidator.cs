using FluentValidation;

namespace HospitalAppointmentSystem.Application.Features.Appointments.Commands.RescheduleAppointment;

/// <summary>
/// Validator for RescheduleAppointmentCommand
/// </summary>
public class RescheduleAppointmentCommandValidator : AbstractValidator<RescheduleAppointmentCommand>
{
    public RescheduleAppointmentCommandValidator()
    {
        RuleFor(x => x.AppointmentId)
            .NotEmpty()
            .WithMessage("Appointment ID is required");

        RuleFor(x => x.NewScheduledDate)
            .NotEmpty()
            .WithMessage("New scheduled date is required")
            .Must(date => date.Date >= DateTime.Today)
            .WithMessage("New appointment date must be today or in the future");

        RuleFor(x => x.NewStartTime)
            .NotEmpty()
            .WithMessage("New start time is required")
            .Must(BeValidTimeFormat)
            .WithMessage("New start time must be in valid format (HH:mm, e.g., 09:00)");

        RuleFor(x => x.NewEndTime)
            .NotEmpty()
            .WithMessage("New end time is required")
            .Must(BeValidTimeFormat)
            .WithMessage("New end time must be in valid format (HH:mm, e.g., 17:00)");

        RuleFor(x => x)
            .Must(x => {
                if (TimeSpan.TryParse(x.NewStartTime, out var start) && TimeSpan.TryParse(x.NewEndTime, out var end))
                {
                    return end > start;
                }
                return true;
            })
            .WithMessage("New end time must be after new start time");

        RuleFor(x => x)
            .Must(x => {
                if (TimeSpan.TryParse(x.NewStartTime, out var start) && TimeSpan.TryParse(x.NewEndTime, out var end))
                {
                    return (end - start).TotalMinutes >= 15;
                }
                return true;
            })
            .WithMessage("New appointment duration must be at least 15 minutes");

        RuleFor(x => x)
            .Must(x => {
                if (TimeSpan.TryParse(x.NewStartTime, out var start) && TimeSpan.TryParse(x.NewEndTime, out var end))
                {
                    return (end - start).TotalHours <= 4;
                }
                return true;
            })
            .WithMessage("New appointment duration cannot exceed 4 hours");
    }

    private static bool BeValidTimeFormat(string time)
    {
        if (string.IsNullOrWhiteSpace(time))
            return false;

        if (!TimeSpan.TryParse(time, out var timeSpan))
            return false;

        return timeSpan >= TimeSpan.Zero && timeSpan < TimeSpan.FromHours(24);
    }
}
