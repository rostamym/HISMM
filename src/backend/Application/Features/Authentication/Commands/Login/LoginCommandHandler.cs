using HospitalAppointmentSystem.Application.Common.Interfaces;
using HospitalAppointmentSystem.Application.Common.Models;
using HospitalAppointmentSystem.Application.Features.Authentication.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HospitalAppointmentSystem.Application.Features.Authentication.Commands.Login;

/// <summary>
/// Handler for user login
/// </summary>
public class LoginCommandHandler : IRequestHandler<LoginCommand, Result<AuthenticationResponse>>
{
    private readonly IApplicationDbContext _context;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly ILogger<LoginCommandHandler> _logger;

    public LoginCommandHandler(
        IApplicationDbContext context,
        IPasswordHasher passwordHasher,
        IJwtTokenService jwtTokenService,
        ILogger<LoginCommandHandler> logger)
    {
        _context = context;
        _passwordHasher = passwordHasher;
        _jwtTokenService = jwtTokenService;
        _logger = logger;
    }

    public async Task<Result<AuthenticationResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Processing login for email: {Email}", request.Email);

        // Find user by email
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == request.Email, cancellationToken);

        if (user == null)
        {
            _logger.LogWarning("User not found for email: {Email}", request.Email);
            return Result<AuthenticationResponse>.Failure("Invalid email or password");
        }

        _logger.LogInformation("User found: Id={UserId}, Email={Email}, Role={Role}", user.Id, user.Email, user.Role);
        _logger.LogInformation("Password hash from DB: {Hash}", user.PasswordHash.Substring(0, 20) + "...");

        // Verify password
        var passwordValid = _passwordHasher.VerifyPassword(request.Password, user.PasswordHash);
        _logger.LogInformation("Password verification result: {IsValid}", passwordValid);

        if (!passwordValid)
        {
            _logger.LogWarning("Invalid password for user: {Email}", request.Email);
            return Result<AuthenticationResponse>.Failure("Invalid email or password");
        }

        // Check if user is active
        if (!user.IsActive)
        {
            _logger.LogWarning("User account is disabled: {Email}", request.Email);
            return Result<AuthenticationResponse>.Failure("User account is disabled");
        }

        // Get role-specific IDs
        Guid? patientId = null;
        Guid? doctorId = null;

        if (user.Role == Domain.Enums.UserRole.Patient)
        {
            var patient = await _context.Patients
                .FirstOrDefaultAsync(p => p.UserId == user.Id, cancellationToken);
            patientId = patient?.Id;
        }
        else if (user.Role == Domain.Enums.UserRole.Doctor)
        {
            var doctor = await _context.Doctors
                .FirstOrDefaultAsync(d => d.UserId == user.Id, cancellationToken);
            doctorId = doctor?.Id;
        }

        // Generate tokens
        var accessToken = _jwtTokenService.GenerateAccessToken(user);
        var refreshToken = _jwtTokenService.GenerateRefreshToken();

        // Create response
        var response = new AuthenticationResponse
        {
            Token = accessToken,
            RefreshToken = refreshToken,
            User = new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                PhoneNumber = user.PhoneNumber,
                DateOfBirth = user.DateOfBirth,
                Role = user.Role.ToString(),
                PatientId = patientId,
                DoctorId = doctorId
            }
        };

        return Result<AuthenticationResponse>.Success(response);
    }
}
