using HospitalAppointmentSystem.Application.Features.Authentication.Commands.Register;
using HospitalAppointmentSystem.Application.Features.Authentication.Commands.Login;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace HospitalAppointmentSystem.API.Controllers;

/// <summary>
/// Authentication endpoints for user registration and login
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuthenticationController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<AuthenticationController> _logger;

    public AuthenticationController(IMediator mediator, ILogger<AuthenticationController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Register a new user
    /// </summary>
    /// <returns>Registration result with authentication tokens and user information</returns>
    [HttpPost("register")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register([FromBody] RegisterCommand command, CancellationToken cancellationToken)
    {
        _logger.LogInformation("User registration requested for email: {Email}", command.Email);

        var result = await _mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
        {
            return BadRequest(new { error = result.Error, errors = result.Errors });
        }

        return Ok(result.Value);
    }

    /// <summary>
    /// Login and get JWT token
    /// </summary>
    /// <returns>JWT token and user information</returns>
    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginCommand command, CancellationToken cancellationToken)
    {
        _logger.LogInformation("User login requested for email: {Email}", command.Email);

        var result = await _mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
        {
            return Unauthorized(new { error = result.Error, errors = result.Errors });
        }

        return Ok(result.Value);
    }

    /// <summary>
    /// Refresh JWT token
    /// </summary>
    /// <returns>New JWT token</returns>
    [HttpPost("refresh")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> RefreshToken()
    {
        // TODO: Implement RefreshTokenCommand
        _logger.LogInformation("Token refresh requested");
        return Ok(new { Message = "Refresh token - To be implemented" });
    }

    /// <summary>
    /// Request password reset
    /// </summary>
    /// <returns>Success message</returns>
    [HttpPost("forgot-password")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ForgotPassword()
    {
        // TODO: Implement ForgotPasswordCommand
        _logger.LogInformation("Password reset requested");
        return Ok(new { Message = "Password reset email sent (if user exists)" });
    }

    /// <summary>
    /// Reset password with token
    /// </summary>
    /// <returns>Success result</returns>
    [HttpPost("reset-password")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ResetPassword()
    {
        // TODO: Implement ResetPasswordCommand
        _logger.LogInformation("Password reset with token requested");
        return Ok(new { Message = "Password reset - To be implemented" });
    }
}
