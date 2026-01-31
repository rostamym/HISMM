using Hangfire;
using Hangfire.SqlServer;
using HospitalAppointmentSystem.Application.Common.Interfaces;
using HospitalAppointmentSystem.Infrastructure.BackgroundJobs;
using HospitalAppointmentSystem.Infrastructure.Persistence;
using HospitalAppointmentSystem.Infrastructure.Services;
using HospitalAppointmentSystem.Infrastructure.Services.Authentication;
using HospitalAppointmentSystem.Infrastructure.Services.Email;
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

        // Configuration
        services.Configure<EmailSettings>(configuration.GetSection("EmailSettings"));

        // Services
        services.AddTransient<IDateTime, DateTimeService>();
        services.AddTransient<IEmailService, EmailService>();

        // Authentication Services
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<IJwtTokenService, JwtTokenService>();

        // Background Jobs
        services.AddScoped<AppointmentReminderJob>();
        services.AddScoped<NoShowMarkerJob>();
        services.AddScoped<DatabaseCleanupJob>();

        // Hangfire Configuration
        services.AddHangfire(config => config
            .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
            .UseSimpleAssemblyNameTypeSerializer()
            .UseRecommendedSerializerSettings()
            .UseSqlServerStorage(
                configuration.GetConnectionString("DefaultConnection"),
                new SqlServerStorageOptions
                {
                    CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                    SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                    QueuePollInterval = TimeSpan.Zero,
                    UseRecommendedIsolationLevel = true,
                    DisableGlobalLocks = true
                }));

        // Add Hangfire server
        services.AddHangfireServer();

        return services;
    }
}
