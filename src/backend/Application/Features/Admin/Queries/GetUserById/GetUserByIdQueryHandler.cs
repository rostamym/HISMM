using HospitalAppointmentSystem.Application.Common.Interfaces;
using HospitalAppointmentSystem.Application.Common.Models;
using HospitalAppointmentSystem.Application.Features.Admin.Queries.GetAllUsers.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HospitalAppointmentSystem.Application.Features.Admin.Queries.GetUserById;

/// <summary>
/// Handler for GetUserByIdQuery
/// </summary>
public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, Result<UserListDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<GetUserByIdQueryHandler> _logger;

    public GetUserByIdQueryHandler(
        IApplicationDbContext context,
        ILogger<GetUserByIdQueryHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Result<UserListDto>> Handle(
        GetUserByIdQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Retrieving user with ID: {UserId}", request.UserId);

            var user = await _context.Users
                .Where(u => u.Id == request.UserId)
                .Select(u => new UserListDto
                {
                    Id = u.Id,
                    Email = u.Email,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    FullName = u.FirstName + " " + u.LastName,
                    PhoneNumber = u.PhoneNumber,
                    Role = u.Role.ToString(),
                    IsActive = u.IsActive,
                    CreatedAt = u.CreatedAt,
                    UpdatedAt = u.UpdatedAt,
                    PatientId = _context.Patients
                        .Where(p => p.UserId == u.Id)
                        .Select(p => (Guid?)p.Id)
                        .FirstOrDefault(),
                    DoctorId = _context.Doctors
                        .Where(d => d.UserId == u.Id)
                        .Select(d => (Guid?)d.Id)
                        .FirstOrDefault(),
                    DoctorSpecialty = _context.Doctors
                        .Where(d => d.UserId == u.Id)
                        .Select(d => d.Specialty != null ? d.Specialty.Name : null)
                        .FirstOrDefault(),
                    DoctorLicenseNumber = _context.Doctors
                        .Where(d => d.UserId == u.Id)
                        .Select(d => d.LicenseNumber)
                        .FirstOrDefault()
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (user == null)
            {
                _logger.LogWarning("User not found with ID: {UserId}", request.UserId);
                return Result<UserListDto>.Failure("User not found");
            }

            _logger.LogInformation("Retrieved user: {UserId}", request.UserId);
            return Result<UserListDto>.Success(user);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving user: {UserId}", request.UserId);
            return Result<UserListDto>.Failure($"Failed to retrieve user: {ex.Message}");
        }
    }
}
