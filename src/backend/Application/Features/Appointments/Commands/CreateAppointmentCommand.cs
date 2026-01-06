using HospitalAppointmentSystem.Application.Common.Interfaces;
using HospitalAppointmentSystem.Application.Common.Models;
using HospitalAppointmentSystem.Domain.Entities;
using MediatR;

namespace HospitalAppointmentSystem.Application.Features.Appointments.Commands;

/// <summary>
/// Command to create a new appointment
/// </summary>
public record CreateAppointmentCommand : IRequest<Result<Guid>>
{
    public Guid PatientId { get; init; }
    public Guid DoctorId { get; init; }
    public DateTime ScheduledDate { get; init; }
    public TimeSpan StartTime { get; init; }
    public TimeSpan EndTime { get; init; }
    public string Reason { get; init; } = string.Empty;
}

/// <summary>
/// Handler for CreateAppointmentCommand
/// </summary>
public class CreateAppointmentCommandHandler : IRequestHandler<CreateAppointmentCommand, Result<Guid>>
{
    private readonly IApplicationDbContext _context;
    private readonly IEmailService _emailService;

    public CreateAppointmentCommandHandler(IApplicationDbContext context, IEmailService emailService)
    {
        _context = context;
        _emailService = emailService;
    }

    public async Task<Result<Guid>> Handle(CreateAppointmentCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // TODO: Check if doctor is available at this time slot
            // TODO: Check for conflicting appointments

            // Create appointment entity
            var appointment = Appointment.Create(
                request.PatientId,
                request.DoctorId,
                request.ScheduledDate,
                request.StartTime,
                request.EndTime,
                request.Reason
            );

            // Add to database
            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync(cancellationToken);

            // Send confirmation email (async, don't await)
            _ = _emailService.SendAppointmentConfirmationAsync(appointment.Id, cancellationToken);

            return Result<Guid>.Success(appointment.Id);
        }
        catch (ArgumentException ex)
        {
            return Result<Guid>.Failure(ex.Message);
        }
        catch (Exception ex)
        {
            return Result<Guid>.Failure($"Failed to create appointment: {ex.Message}");
        }
    }
}
