using System.Reflection;
using HospitalAppointmentSystem.Application.Common.Interfaces;
using HospitalAppointmentSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HospitalAppointmentSystem.Infrastructure.Persistence;

/// <summary>
/// Application database context - implements IApplicationDbContext
/// </summary>
public class ApplicationDbContext : DbContext, IApplicationDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Doctor> Doctors => Set<Doctor>();
    public DbSet<Patient> Patients => Set<Patient>();
    public DbSet<Appointment> Appointments => Set<Appointment>();
    public DbSet<Availability> Availabilities => Set<Availability>();
    public DbSet<Specialty> Specialties => Set<Specialty>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Apply all configurations from current assembly
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        base.OnModelCreating(modelBuilder);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // TODO: Dispatch domain events here before saving
        return await base.SaveChangesAsync(cancellationToken);
    }
}
