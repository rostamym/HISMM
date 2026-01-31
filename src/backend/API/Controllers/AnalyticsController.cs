using HospitalAppointmentSystem.Application.Features.Analytics.Queries.GetAppointmentTrends;
using HospitalAppointmentSystem.Application.Features.Analytics.Queries.GetAppointmentsByStatus;
using HospitalAppointmentSystem.Application.Features.Analytics.Queries.GetAppointmentsBySpecialty;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HospitalAppointmentSystem.API.Controllers;

/// <summary>
/// Controller for analytics and reporting endpoints
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Administrator")]
public class AnalyticsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<AnalyticsController> _logger;

    public AnalyticsController(
        IMediator mediator,
        ILogger<AnalyticsController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Get appointment trends over time
    /// </summary>
    /// <param name="period">Period type: daily, weekly, or monthly</param>
    /// <param name="startDate">Optional start date</param>
    /// <param name="endDate">Optional end date</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of appointment trend data points</returns>
    [HttpGet("appointments/trends")]
    public async Task<IActionResult> GetAppointmentTrends(
        [FromQuery] string period = "daily",
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "GetAppointmentTrends called with period: {Period}, startDate: {StartDate}, endDate: {EndDate}",
            period,
            startDate,
            endDate);

        var query = new GetAppointmentTrendsQuery
        {
            Period = period,
            StartDate = startDate,
            EndDate = endDate
        };

        var result = await _mediator.Send(query, cancellationToken);

        if (!result.IsSuccess)
        {
            _logger.LogWarning("Failed to get appointment trends: {Error}", result.Error);
            return BadRequest(new { Error = result.Error });
        }

        return Ok(result.Value);
    }

    /// <summary>
    /// Get appointment distribution by status
    /// </summary>
    /// <param name="startDate">Optional start date for filtering</param>
    /// <param name="endDate">Optional end date for filtering</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of status distribution data</returns>
    [HttpGet("appointments/by-status")]
    public async Task<IActionResult> GetAppointmentsByStatus(
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "GetAppointmentsByStatus called with startDate: {StartDate}, endDate: {EndDate}",
            startDate,
            endDate);

        var query = new GetAppointmentsByStatusQuery
        {
            StartDate = startDate,
            EndDate = endDate
        };

        var result = await _mediator.Send(query, cancellationToken);

        if (!result.IsSuccess)
        {
            _logger.LogWarning("Failed to get appointments by status: {Error}", result.Error);
            return BadRequest(new { Error = result.Error });
        }

        return Ok(result.Value);
    }

    /// <summary>
    /// Get appointment distribution by specialty
    /// </summary>
    /// <param name="startDate">Optional start date for filtering</param>
    /// <param name="endDate">Optional end date for filtering</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of specialty distribution data</returns>
    [HttpGet("appointments/by-specialty")]
    public async Task<IActionResult> GetAppointmentsBySpecialty(
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "GetAppointmentsBySpecialty called with startDate: {StartDate}, endDate: {EndDate}",
            startDate,
            endDate);

        var query = new GetAppointmentsBySpecialtyQuery
        {
            StartDate = startDate,
            EndDate = endDate
        };

        var result = await _mediator.Send(query, cancellationToken);

        if (!result.IsSuccess)
        {
            _logger.LogWarning("Failed to get appointments by specialty: {Error}", result.Error);
            return BadRequest(new { Error = result.Error });
        }

        return Ok(result.Value);
    }
}
