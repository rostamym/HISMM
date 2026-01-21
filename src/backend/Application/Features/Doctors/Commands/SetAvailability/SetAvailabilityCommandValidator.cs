using FluentValidation;

namespace HospitalAppointmentSystem.Application.Features.Doctors.Commands.SetAvailability;

/// <summary>
/// Validator for SetAvailabilityCommand
/// </summary>
public class SetAvailabilityCommandValidator : AbstractValidator<SetAvailabilityCommand>
{
    public SetAvailabilityCommandValidator()
    {
        RuleFor(x => x.DoctorId)
            .NotEmpty()
            .WithMessage("Doctor ID is required");

        RuleFor(x => x.DayOfWeek)
            .IsInEnum()
            .WithMessage("Invalid day of week");

        RuleFor(x => x.StartTime)
            .LessThan(x => x.EndTime)
            .WithMessage("Start time must be before end time");

        RuleFor(x => x.EndTime)
            .GreaterThan(x => x.StartTime)
            .WithMessage("End time must be after start time");

        RuleFor(x => x.SlotDurationMinutes)
            .GreaterThan(0)
            .WithMessage("Slot duration must be greater than 0")
            .LessThanOrEqualTo(240)
            .WithMessage("Slot duration cannot exceed 240 minutes (4 hours)");

        // Ensure reasonable working hours (6 AM to 11 PM)
        RuleFor(x => x.StartTime)
            .Must(time => time.Hours >= 6 && time.Hours < 23)
            .WithMessage("Start time must be between 6:00 AM and 11:00 PM");

        RuleFor(x => x.EndTime)
            .Must(time => time.Hours >= 7 && time.Hours <= 23)
            .WithMessage("End time must be between 7:00 AM and 11:00 PM");

        // Ensure minimum duration of 1 hour
        RuleFor(x => x)
            .Must(x => (x.EndTime - x.StartTime).TotalHours >= 1)
            .WithMessage("Availability duration must be at least 1 hour");
    }
}
