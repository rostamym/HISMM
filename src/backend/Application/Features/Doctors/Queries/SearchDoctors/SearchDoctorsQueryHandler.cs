using HospitalAppointmentSystem.Application.Common.Interfaces;
using HospitalAppointmentSystem.Application.Common.Models;
using HospitalAppointmentSystem.Application.Features.Doctors.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HospitalAppointmentSystem.Application.Features.Doctors.Queries.SearchDoctors;

/// <summary>
/// Handler for searching doctors with filters
/// </summary>
public class SearchDoctorsQueryHandler : IRequestHandler<SearchDoctorsQuery, Result<PaginatedDoctorsDto>>
{
    private readonly IApplicationDbContext _context;

    public SearchDoctorsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<PaginatedDoctorsDto>> Handle(SearchDoctorsQuery request, CancellationToken cancellationToken)
    {
        // Validate pagination parameters
        if (request.Page < 1)
            request.Page = 1;

        if (request.PageSize < 1 || request.PageSize > 100)
            request.PageSize = 10;

        // Build query
        var query = _context.Doctors
            .Include(d => d.User)
            .Include(d => d.Specialty)
            .Include(d => d.Availabilities)
            .Where(d => d.User.IsActive)
            .AsQueryable();

        // Apply filters
        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var searchTerm = request.SearchTerm.ToLower();
            query = query.Where(d =>
                d.User.FirstName.ToLower().Contains(searchTerm) ||
                d.User.LastName.ToLower().Contains(searchTerm) ||
                d.Specialty.Name.ToLower().Contains(searchTerm));
        }

        if (request.SpecialtyId.HasValue)
        {
            query = query.Where(d => d.SpecialtyId == request.SpecialtyId.Value);
        }

        if (request.MinRating.HasValue)
        {
            query = query.Where(d => d.Rating >= request.MinRating.Value);
        }

        if (request.MaxConsultationFee.HasValue)
        {
            query = query.Where(d => d.ConsultationFee == null || d.ConsultationFee <= request.MaxConsultationFee.Value);
        }

        if (request.IsAvailable.HasValue && request.IsAvailable.Value)
        {
            query = query.Where(d => d.Availabilities.Any(a => a.IsActive));
        }

        // Apply sorting
        query = request.SortBy?.ToLower() switch
        {
            "name" => request.SortOrder?.ToLower() == "desc"
                ? query.OrderByDescending(d => d.User.LastName).ThenByDescending(d => d.User.FirstName)
                : query.OrderBy(d => d.User.LastName).ThenBy(d => d.User.FirstName),
            "experience" => request.SortOrder?.ToLower() == "desc"
                ? query.OrderByDescending(d => d.YearsOfExperience)
                : query.OrderBy(d => d.YearsOfExperience),
            "fee" => request.SortOrder?.ToLower() == "desc"
                ? query.OrderByDescending(d => d.ConsultationFee ?? 0)
                : query.OrderBy(d => d.ConsultationFee ?? 0),
            "rating" or _ => request.SortOrder?.ToLower() == "desc"
                ? query.OrderByDescending(d => d.Rating)
                : query.OrderBy(d => d.Rating)
        };

        // Get total count
        var totalCount = await query.CountAsync(cancellationToken);

        // Apply pagination
        var doctors = await query
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        // Map to DTOs
        var doctorDtos = doctors.Select(d => new DoctorDto
        {
            Id = d.Id,
            FirstName = d.User.FirstName,
            LastName = d.User.LastName,
            Email = d.User.Email,
            PhoneNumber = d.User.PhoneNumber,
            LicenseNumber = d.LicenseNumber,
            SpecialtyId = d.SpecialtyId,
            SpecialtyName = d.Specialty.Name,
            Biography = d.Biography,
            YearsOfExperience = d.YearsOfExperience,
            ConsultationFee = d.ConsultationFee,
            Rating = d.Rating,
            IsAvailable = d.Availabilities.Any(a => a.IsActive)
        }).ToList();

        var result = new PaginatedDoctorsDto
        {
            Items = doctorDtos,
            TotalCount = totalCount,
            PageNumber = request.Page,
            PageSize = request.PageSize
        };

        return Result<PaginatedDoctorsDto>.Success(result);
    }
}
