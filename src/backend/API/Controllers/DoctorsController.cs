using HospitalAppointmentSystem.Application.Features.Doctors.Queries.GetDoctors;
using HospitalAppointmentSystem.Application.Features.Doctors.Queries.GetDoctorById;
using HospitalAppointmentSystem.Application.Features.Doctors.Queries.SearchDoctors;
using HospitalAppointmentSystem.Application.Features.Doctors.Queries.GetDoctorAvailability;
using HospitalAppointmentSystem.Application.Features.Doctors.Queries.GetAvailableTimeSlots;
using HospitalAppointmentSystem.Application.Features.Doctors.Commands.SetAvailability;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HospitalAppointmentSystem.API.Controllers;

/// <summary>
/// Doctors management endpoints
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class DoctorsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<DoctorsController> _logger;

    public DoctorsController(IMediator mediator, ILogger<DoctorsController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Get all doctors with pagination and sorting
    /// </summary>
    /// <param name="page">Page number (default: 1)</param>
    /// <param name="pageSize">Page size (default: 10)</param>
    /// <param name="sortBy">Sort by: rating, name, experience (default: rating)</param>
    /// <param name="sortOrder">Sort order: asc, desc (default: desc)</param>
    /// <returns>Paginated list of doctors</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetDoctors(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? sortBy = "rating",
        [FromQuery] string? sortOrder = "desc",
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting doctors - Page: {Page}, PageSize: {PageSize}", page, pageSize);

        var query = new GetDoctorsQuery
        {
            Page = page,
            PageSize = pageSize,
            SortBy = sortBy,
            SortOrder = sortOrder
        };

        var result = await _mediator.Send(query, cancellationToken);

        if (!result.IsSuccess)
        {
            return BadRequest(new { error = result.Error });
        }

        return Ok(result.Value);
    }

    /// <summary>
    /// Get doctor by ID with detailed information
    /// </summary>
    /// <param name="id">Doctor ID</param>
    /// <returns>Doctor details including availabilities and statistics</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetDoctorById(Guid id, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting doctor {DoctorId}", id);

        var query = new GetDoctorByIdQuery { DoctorId = id };
        var result = await _mediator.Send(query, cancellationToken);

        if (!result.IsSuccess)
        {
            return NotFound(new { error = result.Error });
        }

        return Ok(result.Value);
    }

    /// <summary>
    /// Search doctors by name, specialty, rating, consultation fee
    /// </summary>
    /// <param name="searchTerm">Search term (name or specialty)</param>
    /// <param name="specialtyId">Filter by specialty ID</param>
    /// <param name="minRating">Minimum rating (0-5)</param>
    /// <param name="maxConsultationFee">Maximum consultation fee</param>
    /// <param name="isAvailable">Filter only available doctors</param>
    /// <param name="page">Page number (default: 1)</param>
    /// <param name="pageSize">Page size (default: 10)</param>
    /// <param name="sortBy">Sort by: rating, name, experience, fee (default: rating)</param>
    /// <param name="sortOrder">Sort order: asc, desc (default: desc)</param>
    /// <returns>Paginated list of matching doctors</returns>
    [HttpGet("search")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> SearchDoctors(
        [FromQuery] string? searchTerm = null,
        [FromQuery] Guid? specialtyId = null,
        [FromQuery] decimal? minRating = null,
        [FromQuery] decimal? maxConsultationFee = null,
        [FromQuery] bool? isAvailable = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? sortBy = "rating",
        [FromQuery] string? sortOrder = "desc",
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Searching doctors with term: {SearchTerm}", searchTerm);

        var query = new SearchDoctorsQuery
        {
            SearchTerm = searchTerm,
            SpecialtyId = specialtyId,
            MinRating = minRating,
            MaxConsultationFee = maxConsultationFee,
            IsAvailable = isAvailable,
            Page = page,
            PageSize = pageSize,
            SortBy = sortBy,
            SortOrder = sortOrder
        };

        var result = await _mediator.Send(query, cancellationToken);

        if (!result.IsSuccess)
        {
            return BadRequest(new { error = result.Error });
        }

        return Ok(result.Value);
    }

    /// <summary>
    /// Get doctor's availability schedule (all days and times they work)
    /// </summary>
    /// <param name="id">Doctor ID</param>
    /// <returns>List of availability schedules</returns>
    [HttpGet("{id}/availability")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetDoctorAvailability(Guid id, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting availability schedule for doctor {DoctorId}", id);

        var query = new GetDoctorAvailabilityQuery { DoctorId = id };
        var result = await _mediator.Send(query, cancellationToken);

        if (!result.IsSuccess)
        {
            return NotFound(new { error = result.Error });
        }

        return Ok(result.Value);
    }

    /// <summary>
    /// Get available time slots for booking on a specific date
    /// </summary>
    /// <param name="id">Doctor ID</param>
    /// <param name="date">Date to check availability</param>
    /// <returns>List of available time slots for booking</returns>
    [HttpGet("{id}/available-slots")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetAvailableTimeSlots(
        Guid id,
        [FromQuery] DateTime date,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting available time slots for doctor {DoctorId} on {Date}", id, date);

        var query = new GetAvailableTimeSlotsQuery
        {
            DoctorId = id,
            Date = date
        };

        var result = await _mediator.Send(query, cancellationToken);

        if (!result.IsSuccess)
        {
            return BadRequest(new { error = result.Error });
        }

        return Ok(result.Value);
    }

    /// <summary>
    /// Set doctor availability (Doctor only)
    /// </summary>
    /// <param name="id">Doctor ID</param>
    /// <param name="command">Availability details</param>
    /// <returns>Created availability ID</returns>
    [HttpPost("{id}/availability")]
    [Authorize(Roles = "Doctor,Admin")]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> SetAvailability(
        Guid id,
        [FromBody] SetAvailabilityCommand command,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Setting availability for doctor {DoctorId}", id);

        // Ensure the doctor ID in the route matches the command
        command.DoctorId = id;

        var result = await _mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
        {
            return BadRequest(new { error = result.Error });
        }

        return CreatedAtAction(
            nameof(GetDoctorAvailability),
            new { id = id },
            new { availabilityId = result.Value });
    }

    /// <summary>
    /// Create a new doctor (Admin only)
    /// </summary>
    /// <returns>Created doctor ID</returns>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> CreateDoctor()
    {
        // TODO: Implement CreateDoctorCommand
        _logger.LogInformation("Creating new doctor");
        return CreatedAtAction(nameof(GetDoctorById), new { id = Guid.NewGuid() }, new { Message = "Create doctor - To be implemented" });
    }

    /// <summary>
    /// Update doctor information (Admin or Doctor themselves)
    /// </summary>
    /// <param name="id">Doctor ID</param>
    /// <returns>Success result</returns>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,Doctor")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> UpdateDoctor(Guid id)
    {
        // TODO: Implement UpdateDoctorCommand
        _logger.LogInformation("Updating doctor {DoctorId}", id);
        return NoContent();
    }
}
