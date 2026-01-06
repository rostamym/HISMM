using HospitalAppointmentSystem.Application.Common.Interfaces;
using HospitalAppointmentSystem.Infrastructure.Persistence;
using HospitalAppointmentSystem.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HospitalAppointmentSystem.Infrastructure;

/// <summary>
/// Dependency injection configuration for Infrastructure layer
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Database
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));

        services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());

        // Services
        services.AddTransient<IDateTime, DateTimeService>();
        services.AddTransient<IEmailService, EmailService>();

        // TODO: Add Identity, JWT, Hangfire, etc.

        return services;
    }
}
