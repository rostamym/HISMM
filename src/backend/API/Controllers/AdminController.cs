using HospitalAppointmentSystem.Application.Features.Admin.Commands.UpdateUserStatus;
using HospitalAppointmentSystem.Application.Features.Admin.Queries.GetAllAppointmentsForAdmin;
using HospitalAppointmentSystem.Application.Features.Admin.Queries.GetAllUsers;
using HospitalAppointmentSystem.Application.Features.Admin.Queries.GetSystemStatistics;
using HospitalAppointmentSystem.Application.Features.Admin.Queries.GetUserById;
using HospitalAppointmentSystem.Application.Features.Admin.Queries.GetUserAppointmentHistory;
using HospitalAppointmentSystem.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HospitalAppointmentSystem.API.Controllers;

/// <summary>
/// Admin management endpoints
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Administrator")]
public class AdminController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<AdminController> _logger;

    public AdminController(IMediator mediator, ILogger<AdminController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Get system statistics
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>System statistics</returns>
    [HttpGet("statistics")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetSystemStatistics(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting system statistics");

        var query = new GetSystemStatisticsQuery();
        var result = await _mediator.Send(query, cancellationToken);

        if (!result.IsSuccess)
        {
            return BadRequest(new { Error = result.Error });
        }

        return Ok(result.Value);
    }

    /// <summary>
    /// Get all users with optional filters
    /// </summary>
    /// <param name="role">Filter by user role</param>
    /// <param name="isActive">Filter by active status</param>
    /// <param name="searchTerm">Search term for email, first name, or last name</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of users</returns>
    [HttpGet("users")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetAllUsers(
        [FromQuery] UserRole? role,
        [FromQuery] bool? isActive,
        [FromQuery] string? searchTerm,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting all users - Role: {Role}, IsActive: {IsActive}, SearchTerm: {SearchTerm}",
            role, isActive, searchTerm);

        var query = new GetAllUsersQuery
        {
            Role = role,
            IsActive = isActive,
            SearchTerm = searchTerm
        };

        var result = await _mediator.Send(query, cancellationToken);

        if (!result.IsSuccess)
        {
            return BadRequest(new { Error = result.Error });
        }

        return Ok(result.Value);
    }

    /// <summary>
    /// Get user by ID
    /// </summary>
    /// <param name="id">User ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>User details</returns>
    [HttpGet("users/{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetUserById(Guid id, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting user by ID: {UserId}", id);

        var query = new GetUserByIdQuery(id);
        var result = await _mediator.Send(query, cancellationToken);

        if (!result.IsSuccess)
        {
            if (result.Error.Contains("not found"))
            {
                return NotFound(new { Error = result.Error });
            }
            return BadRequest(new { Error = result.Error });
        }

        return Ok(result.Value);
    }

    /// <summary>
    /// Update user active status
    /// </summary>
    /// <param name="id">User ID</param>
    /// <param name="command">Status update request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Success result</returns>
    [HttpPatch("users/{id}/status")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> UpdateUserStatus(
        Guid id,
        [FromBody] UpdateUserStatusCommand command,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating user status - UserId: {UserId}, IsActive: {IsActive}",
            id, command.IsActive);

        // Override the UserId from route parameter
        var updateCommand = command with { UserId = id };

        var result = await _mediator.Send(updateCommand, cancellationToken);

        if (!result.IsSuccess)
        {
            if (result.Error.Contains("not found"))
            {
                return NotFound(new { Error = result.Error });
            }
            return BadRequest(new { Error = result.Error });
        }

        return NoContent();
    }

    /// <summary>
    /// Get all appointments with optional filters
    /// </summary>
    /// <param name="fromDate">Filter appointments from this date</param>
    /// <param name="toDate">Filter appointments to this date</param>
    /// <param name="status">Filter by appointment status</param>
    /// <param name="patientId">Filter by patient ID</param>
    /// <param name="doctorId">Filter by doctor ID</param>
    /// <param name="searchTerm">Search term for patient or doctor name</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of appointments</returns>
    [HttpGet("appointments")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetAllAppointments(
        [FromQuery] DateTime? fromDate,
        [FromQuery] DateTime? toDate,
        [FromQuery] string? status,
        [FromQuery] Guid? patientId,
        [FromQuery] Guid? doctorId,
        [FromQuery] string? searchTerm,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Getting all appointments - FromDate: {FromDate}, ToDate: {ToDate}, Status: {Status}, PatientId: {PatientId}, DoctorId: {DoctorId}, SearchTerm: {SearchTerm}",
            fromDate, toDate, status, patientId, doctorId, searchTerm);

        var query = new GetAllAppointmentsForAdminQuery
        {
            FromDate = fromDate,
            ToDate = toDate,
            StatusFilter = status,
            PatientId = patientId,
            DoctorId = doctorId,
            SearchTerm = searchTerm
        };

        var result = await _mediator.Send(query, cancellationToken);

        if (!result.IsSuccess)
        {
            return BadRequest(new { Error = result.Error });
        }

        return Ok(result.Value);
    }

    /// <summary>
    /// Get appointment history for a specific user
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of user's appointments</returns>
    [HttpGet("users/{userId}/appointments")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetUserAppointmentHistory(
        Guid userId,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting appointment history for user: {UserId}", userId);

        var query = new GetUserAppointmentHistoryQuery(userId);
        var result = await _mediator.Send(query, cancellationToken);

        if (!result.IsSuccess)
        {
            if (result.Error.Contains("not found"))
            {
                return NotFound(new { Error = result.Error });
            }
            return BadRequest(new { Error = result.Error });
        }

        return Ok(result.Value);
    }
}
