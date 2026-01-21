using HospitalAppointmentSystem.Application.Features.Appointments.Commands;
using HospitalAppointmentSystem.Application.Features.Appointments.Commands.CancelAppointment;
using HospitalAppointmentSystem.Application.Features.Appointments.Commands.RescheduleAppointment;
using HospitalAppointmentSystem.Application.Features.Appointments.Queries.GetAppointmentById;
using HospitalAppointmentSystem.Application.Features.Appointments.Queries.GetAppointmentsByDoctor;
using HospitalAppointmentSystem.Application.Features.Appointments.Queries.GetAppointmentsByPatient;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HospitalAppointmentSystem.API.Controllers;

/// <summary>
/// Appointments management endpoints
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AppointmentsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<AppointmentsController> _logger;

    public AppointmentsController(IMediator mediator, ILogger<AppointmentsController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Create a new appointment
    /// </summary>
    /// <param name="command">Appointment details</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Created appointment ID</returns>
    [HttpPost]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> CreateAppointment(
        [FromBody] CreateAppointmentCommand command,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating appointment for patient {PatientId} with doctor {DoctorId}",
            command.PatientId, command.DoctorId);

        var result = await _mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
        {
            return BadRequest(new { Error = result.Error });
        }

        return CreatedAtAction(
            nameof(GetAppointmentById),
            new { id = result.Value },
            new { Id = result.Value });
    }

    /// <summary>
    /// Get appointment by ID
    /// </summary>
    /// <param name="id">Appointment ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Appointment details</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetAppointmentById(Guid id, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting appointment {AppointmentId}", id);

        var result = await _mediator.Send(new GetAppointmentByIdQuery(id), cancellationToken);

        if (!result.IsSuccess)
        {
            return NotFound(new { Error = result.Error });
        }

        return Ok(result.Value);
    }

    /// <summary>
    /// Get appointments for a patient
    /// </summary>
    /// <param name="patientId">Patient ID</param>
    /// <param name="statusFilter">Optional status filter (e.g., Scheduled, Completed, Cancelled)</param>
    /// <param name="upcomingOnly">Filter for upcoming appointments only</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of appointments</returns>
    [HttpGet("patient/{patientId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetAppointmentsByPatient(
        Guid patientId,
        [FromQuery] string? statusFilter = null,
        [FromQuery] bool upcomingOnly = false,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting appointments for patient {PatientId}", patientId);

        var query = new GetAppointmentsByPatientQuery
        {
            PatientId = patientId,
            StatusFilter = statusFilter,
            UpcomingOnly = upcomingOnly
        };

        var result = await _mediator.Send(query, cancellationToken);

        if (!result.IsSuccess)
        {
            return BadRequest(new { Error = result.Error });
        }

        return Ok(result.Value);
    }

    /// <summary>
    /// Get appointments for a doctor
    /// </summary>
    /// <param name="doctorId">Doctor ID</param>
    /// <param name="fromDate">Optional start date filter</param>
    /// <param name="toDate">Optional end date filter</param>
    /// <param name="statusFilter">Optional status filter (e.g., Scheduled, Completed, Cancelled)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of appointments</returns>
    [HttpGet("doctor/{doctorId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetAppointmentsByDoctor(
        Guid doctorId,
        [FromQuery] DateTime? fromDate = null,
        [FromQuery] DateTime? toDate = null,
        [FromQuery] string? statusFilter = null,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting appointments for doctor {DoctorId}", doctorId);

        var query = new GetAppointmentsByDoctorQuery
        {
            DoctorId = doctorId,
            FromDate = fromDate,
            ToDate = toDate,
            StatusFilter = statusFilter
        };

        var result = await _mediator.Send(query, cancellationToken);

        if (!result.IsSuccess)
        {
            return BadRequest(new { Error = result.Error });
        }

        return Ok(result.Value);
    }

    /// <summary>
    /// Cancel an appointment
    /// </summary>
    /// <param name="id">Appointment ID</param>
    /// <param name="command">Cancellation details (reason)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Success result</returns>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> CancelAppointment(
        Guid id,
        [FromBody] CancelAppointmentCommand command,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Cancelling appointment {AppointmentId}", id);

        // Override the AppointmentId from route
        var cancelCommand = command with { AppointmentId = id };

        var result = await _mediator.Send(cancelCommand, cancellationToken);

        if (!result.IsSuccess)
        {
            if (result.Error.Contains("not found"))
                return NotFound(new { Error = result.Error });

            return BadRequest(new { Error = result.Error });
        }

        return NoContent();
    }

    /// <summary>
    /// Reschedule an appointment
    /// </summary>
    /// <param name="id">Appointment ID</param>
    /// <param name="command">New appointment details</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Success result</returns>
    [HttpPut("{id}/reschedule")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> RescheduleAppointment(
        Guid id,
        [FromBody] RescheduleAppointmentCommand command,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Rescheduling appointment {AppointmentId}", id);

        // Override the AppointmentId from route
        var rescheduleCommand = command with { AppointmentId = id };

        var result = await _mediator.Send(rescheduleCommand, cancellationToken);

        if (!result.IsSuccess)
        {
            if (result.Error.Contains("not found"))
                return NotFound(new { Error = result.Error });

            return BadRequest(new { Error = result.Error });
        }

        return NoContent();
    }
}
