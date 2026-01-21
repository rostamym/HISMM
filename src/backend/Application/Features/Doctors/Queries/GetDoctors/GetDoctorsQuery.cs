using HospitalAppointmentSystem.Application.Common.Models;
using HospitalAppointmentSystem.Application.Features.Doctors.Common;
using MediatR;

namespace HospitalAppointmentSystem.Application.Features.Doctors.Queries.GetDoctors;

/// <summary>
/// Query to get paginated list of doctors
/// </summary>
public class GetDoctorsQuery : IRequest<Result<PaginatedDoctorsDto>>
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SortBy { get; set; } = "rating"; // rating, name, experience
    public string? SortOrder { get; set; } = "desc"; // asc, desc
}
