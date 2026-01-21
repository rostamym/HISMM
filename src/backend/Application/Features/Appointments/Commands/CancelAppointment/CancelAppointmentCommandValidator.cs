using FluentValidation;

namespace HospitalAppointmentSystem.Application.Features.Appointments.Commands.CancelAppointment;

/// <summary>
/// Validator for CancelAppointmentCommand
/// </summary>
public class CancelAppointmentCommandValidator : AbstractValidator<CancelAppointmentCommand>
{
    public CancelAppointmentCommandValidator()
    {
        RuleFor(x => x.AppointmentId)
            .NotEmpty()
            .WithMessage("Appointment ID is required");

        RuleFor(x => x.CancellationReason)
            .NotEmpty()
            .WithMessage("Cancellation reason is required")
            .MinimumLength(5)
            .WithMessage("Cancellation reason must be at least 5 characters")
            .MaximumLength(500)
            .WithMessage("Cancellation reason cannot exceed 500 characters");
    }
}
