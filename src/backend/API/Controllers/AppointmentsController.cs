using HospitalAppointmentSystem.Application.Features.Appointments.Commands;
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

        if (!result.Succeeded)
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
    /// <returns>Appointment details</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetAppointmentById(Guid id)
    {
        // TODO: Implement GetAppointmentByIdQuery
        _logger.LogInformation("Getting appointment {AppointmentId}", id);
        return Ok(new { Message = "Get appointment by ID - To be implemented" });
    }

    /// <summary>
    /// Get appointments for a patient
    /// </summary>
    /// <param name="patientId">Patient ID</param>
    /// <returns>List of appointments</returns>
    [HttpGet("patient/{patientId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetAppointmentsByPatient(Guid patientId)
    {
        // TODO: Implement GetAppointmentsByPatientQuery
        _logger.LogInformation("Getting appointments for patient {PatientId}", patientId);
        return Ok(new { Message = "Get appointments by patient - To be implemented" });
    }

    /// <summary>
    /// Get appointments for a doctor
    /// </summary>
    /// <param name="doctorId">Doctor ID</param>
    /// <returns>List of appointments</returns>
    [HttpGet("doctor/{doctorId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetAppointmentsByDoctor(Guid doctorId)
    {
        // TODO: Implement GetAppointmentsByDoctorQuery
        _logger.LogInformation("Getting appointments for doctor {DoctorId}", doctorId);
        return Ok(new { Message = "Get appointments by doctor - To be implemented" });
    }

    /// <summary>
    /// Cancel an appointment
    /// </summary>
    /// <param name="id">Appointment ID</param>
    /// <returns>Success result</returns>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> CancelAppointment(Guid id)
    {
        // TODO: Implement CancelAppointmentCommand
        _logger.LogInformation("Cancelling appointment {AppointmentId}", id);
        return NoContent();
    }

    /// <summary>
    /// Reschedule an appointment
    /// </summary>
    /// <param name="id">Appointment ID</param>
    /// <returns>Success result</returns>
    [HttpPut("{id}/reschedule")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> RescheduleAppointment(Guid id)
    {
        // TODO: Implement RescheduleAppointmentCommand
        _logger.LogInformation("Rescheduling appointment {AppointmentId}", id);
        return NoContent();
    }
}
