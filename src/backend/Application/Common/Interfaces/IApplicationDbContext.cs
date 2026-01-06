using HospitalAppointmentSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HospitalAppointmentSystem.Application.Common.Interfaces;

/// <summary>
/// Application database context interface
/// Defines the contract for data access without coupling to EF Core implementation
/// </summary>
public interface IApplicationDbContext
{
    DbSet<User> Users { get; }
    DbSet<Doctor> Doctors { get; }
    DbSet<Patient> Patients { get; }
    DbSet<Appointment> Appointments { get; }
    DbSet<Availability> Availabilities { get; }
    DbSet<Specialty> Specialties { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
