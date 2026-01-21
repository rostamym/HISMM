using HospitalAppointmentSystem.Application.Common.Models;
using HospitalAppointmentSystem.Application.Features.Doctors.Common;
using MediatR;

namespace HospitalAppointmentSystem.Application.Features.Doctors.Queries.SearchDoctors;

/// <summary>
/// Query to search doctors with filters
/// </summary>
public class SearchDoctorsQuery : IRequest<Result<PaginatedDoctorsDto>>
{
    public string? SearchTerm { get; set; } // Search in name, specialty
    public Guid? SpecialtyId { get; set; }
    public decimal? MinRating { get; set; }
    public decimal? MaxConsultationFee { get; set; }
    public bool? IsAvailable { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SortBy { get; set; } = "rating"; // rating, name, experience, fee
    public string? SortOrder { get; set; } = "desc"; // asc, desc
}
