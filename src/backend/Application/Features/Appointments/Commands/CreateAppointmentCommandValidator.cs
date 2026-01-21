using FluentValidation;

namespace HospitalAppointmentSystem.Application.Features.Appointments.Commands;

/// <summary>
/// Validator for CreateAppointmentCommand
/// </summary>
public class CreateAppointmentCommandValidator : AbstractValidator<CreateAppointmentCommand>
{
    public CreateAppointmentCommandValidator()
    {
        RuleFor(x => x.PatientId)
            .NotEmpty()
            .WithMessage("Patient ID is required");

        RuleFor(x => x.DoctorId)
            .NotEmpty()
            .WithMessage("Doctor ID is required");

        RuleFor(x => x.ScheduledDate)
            .NotEmpty()
            .WithMessage("Scheduled date is required")
            .Must(date => date.Date >= DateTime.Today)
            .WithMessage("Appointment date must be today or in the future");

        RuleFor(x => x.StartTime)
            .NotEmpty()
            .WithMessage("Start time is required")
            .Must(BeValidTimeFormat)
            .WithMessage("Start time must be in valid format (HH:mm, e.g., 09:00)");

        RuleFor(x => x.EndTime)
            .NotEmpty()
            .WithMessage("End time is required")
            .Must(BeValidTimeFormat)
            .WithMessage("End time must be in valid format (HH:mm, e.g., 17:00)");

        RuleFor(x => x)
            .Must(x => {
                if (TimeSpan.TryParse(x.StartTime, out var start) && TimeSpan.TryParse(x.EndTime, out var end))
                {
                    return end > start;
                }
                return true; // Skip this validation if format is invalid (will be caught by format validator)
            })
            .WithMessage("End time must be after start time");

        RuleFor(x => x)
            .Must(x => {
                if (TimeSpan.TryParse(x.StartTime, out var start) && TimeSpan.TryParse(x.EndTime, out var end))
                {
                    return (end - start).TotalMinutes >= 15;
                }
                return true;
            })
            .WithMessage("Appointment duration must be at least 15 minutes");

        RuleFor(x => x)
            .Must(x => {
                if (TimeSpan.TryParse(x.StartTime, out var start) && TimeSpan.TryParse(x.EndTime, out var end))
                {
                    return (end - start).TotalHours <= 4;
                }
                return true;
            })
            .WithMessage("Appointment duration cannot exceed 4 hours");

        RuleFor(x => x.Reason)
            .NotEmpty()
            .WithMessage("Appointment reason is required")
            .MinimumLength(5)
            .WithMessage("Reason must be at least 5 characters")
            .MaximumLength(500)
            .WithMessage("Reason cannot exceed 500 characters");
    }

    private static bool BeValidTimeFormat(string time)
    {
        if (string.IsNullOrWhiteSpace(time))
            return false;

        if (!TimeSpan.TryParse(time, out var timeSpan))
            return false;

        // Ensure time is within valid day range (00:00 to 23:59)
        return timeSpan >= TimeSpan.Zero && timeSpan < TimeSpan.FromHours(24);
    }
}
