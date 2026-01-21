using HospitalAppointmentSystem.Application.Common.Interfaces;
using HospitalAppointmentSystem.Application.Common.Models;
using HospitalAppointmentSystem.Application.Features.Authentication.Common;
using HospitalAppointmentSystem.Domain.Entities;
using HospitalAppointmentSystem.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HospitalAppointmentSystem.Application.Features.Authentication.Commands.Register;

/// <summary>
/// Handler for user registration
/// </summary>
public class RegisterCommandHandler : IRequestHandler<RegisterCommand, Result<AuthenticationResponse>>
{
    private readonly IApplicationDbContext _context;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IEmailService _emailService;

    public RegisterCommandHandler(
        IApplicationDbContext context,
        IPasswordHasher passwordHasher,
        IJwtTokenService jwtTokenService,
        IEmailService emailService)
    {
        _context = context;
        _passwordHasher = passwordHasher;
        _jwtTokenService = jwtTokenService;
        _emailService = emailService;
    }

    public async Task<Result<AuthenticationResponse>> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        // Check if user already exists
        var existingUser = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == request.Email, cancellationToken);

        if (existingUser != null)
        {
            return Result<AuthenticationResponse>.Failure("A user with this email already exists");
        }

        // Parse role
        if (!Enum.TryParse<UserRole>(request.Role, out var userRole))
        {
            return Result<AuthenticationResponse>.Failure("Invalid role specified");
        }

        // Hash password
        var passwordHash = _passwordHasher.HashPassword(request.Password);

        // Create new user using factory method
        var user = User.Create(
            request.Email,
            passwordHash,
            request.FirstName,
            request.LastName,
            request.PhoneNumber,
            request.DateOfBirth,
            userRole
        );

        _context.Users.Add(user);
        await _context.SaveChangesAsync(cancellationToken);

        // If role is Doctor or Patient, create corresponding entity
        Guid? patientId = null;
        Guid? doctorId = null;

        if (userRole == UserRole.Doctor)
        {
            // For registration, we'll create a minimal Doctor profile
            // They will need to complete their profile later with specialty, license, etc.
            // For now, we'll skip creating Doctor entity during registration
            // and handle it separately in a complete profile workflow
        }
        else if (userRole == UserRole.Patient)
        {
            var patient = Patient.Create(user.Id);
            _context.Patients.Add(patient);
            await _context.SaveChangesAsync(cancellationToken);
            patientId = patient.Id;
        }

        // Generate tokens
        var accessToken = _jwtTokenService.GenerateAccessToken(user);
        var refreshToken = _jwtTokenService.GenerateRefreshToken();

        // Send registration confirmation email (fire and forget - don't block response)
        _ = Task.Run(async () =>
        {
            try
            {
                await _emailService.SendRegistrationConfirmationAsync(
                    user.Email,
                    user.FirstName,
                    user.LastName,
                    cancellationToken);
            }
            catch (Exception)
            {
                // Log error but don't fail registration if email fails
                // Email failures shouldn't prevent user registration
            }
        }, cancellationToken);

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
