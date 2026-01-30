using HospitalAppointmentSystem.Application.Common.Interfaces;
using HospitalAppointmentSystem.Application.Common.Models;
using HospitalAppointmentSystem.Application.Features.Admin.Queries.GetAllUsers.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HospitalAppointmentSystem.Application.Features.Admin.Queries.GetAllUsers;

/// <summary>
/// Handler for GetAllUsersQuery
/// </summary>
public class GetAllUsersQueryHandler : IRequestHandler<GetAllUsersQuery, Result<List<UserListDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<GetAllUsersQueryHandler> _logger;

    public GetAllUsersQueryHandler(
        IApplicationDbContext context,
        ILogger<GetAllUsersQueryHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Result<List<UserListDto>>> Handle(
        GetAllUsersQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Retrieving all users with filters - Role: {Role}, IsActive: {IsActive}, SearchTerm: {SearchTerm}",
                request.Role, request.IsActive, request.SearchTerm);

            var query = _context.Users.AsQueryable();

            // Apply role filter
            if (request.Role.HasValue)
            {
                query = query.Where(u => u.Role == request.Role.Value);
            }

            // Apply active status filter
            if (request.IsActive.HasValue)
            {
                query = query.Where(u => u.IsActive == request.IsActive.Value);
            }

            // Apply search term filter
            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                var searchTerm = request.SearchTerm.ToLower();
                query = query.Where(u =>
                    u.Email.ToLower().Contains(searchTerm) ||
                    u.FirstName.ToLower().Contains(searchTerm) ||
                    u.LastName.ToLower().Contains(searchTerm));
            }

            var users = await query
                .OrderBy(u => u.CreatedAt)
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
                    // Get patient/doctor IDs if applicable
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
                .ToListAsync(cancellationToken);

            _logger.LogInformation("Retrieved {Count} users", users.Count);
            return Result<List<UserListDto>>.Success(users);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving users");
            return Result<List<UserListDto>>.Failure($"Failed to retrieve users: {ex.Message}");
        }
    }
}
