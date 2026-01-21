using HospitalAppointmentSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HospitalAppointmentSystem.Infrastructure.Persistence.Configurations;

public class DoctorConfiguration : IEntityTypeConfiguration<Doctor>
{
    public void Configure(EntityTypeBuilder<Doctor> builder)
    {
        // Configure decimal properties with explicit precision and scale
        builder.Property(d => d.ConsultationFee)
            .HasPrecision(18, 2); // 18 digits total, 2 after decimal point

        builder.Property(d => d.Rating)
            .HasPrecision(3, 2); // 3 digits total, 2 after decimal point (0.00 to 5.00)
    }
}
