using HospitalAppointmentSystem.Domain.Entities;
using HospitalAppointmentSystem.Domain.Enums;
using HospitalAppointmentSystem.Infrastructure.Services.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HospitalAppointmentSystem.Infrastructure.Persistence;

/// <summary>
/// Seeds the database with initial data
/// </summary>
public class DatabaseSeeder
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<DatabaseSeeder> _logger;
    private readonly PasswordHasher _passwordHasher;

    public DatabaseSeeder(ApplicationDbContext context, ILogger<DatabaseSeeder> logger)
    {
        _context = context;
        _logger = logger;
        _passwordHasher = new PasswordHasher();
    }

    public async Task SeedAsync()
    {
        try
        {
            // Seed Admin User (First!)
            await SeedAdminAsync();

            // Seed Specialties
            await SeedSpecialtiesAsync();

            // Seed Doctors
            await SeedDoctorsAsync();

            // Seed Patients
            await SeedPatientsAsync();

            await _context.SaveChangesAsync();

            _logger.LogInformation("Database seeded successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while seeding the database");
            throw;
        }
    }

    private async Task SeedSpecialtiesAsync()
    {
        if (await _context.Specialties.AnyAsync())
        {
            _logger.LogInformation("Specialties already seeded");
            return;
        }

        var specialties = new[]
        {
            Specialty.Create("Cardiology", "Heart and cardiovascular system specialists"),
            Specialty.Create("Dermatology", "Skin, hair, and nail specialists"),
            Specialty.Create("Pediatrics", "Child healthcare specialists"),
            Specialty.Create("Orthopedics", "Bone, joint, and muscle specialists"),
            Specialty.Create("Neurology", "Nervous system specialists"),
            Specialty.Create("General Practice", "General medical practitioners"),
            Specialty.Create("Ophthalmology", "Eye care specialists"),
            Specialty.Create("Dentistry", "Dental care specialists")
        };

        _context.Specialties.AddRange(specialties);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Seeded {Count} specialties", specialties.Length);
    }

    private async Task SeedAdminAsync()
    {
        // Check if admin user already exists
        var adminExists = await _context.Users.AnyAsync(u => u.Email == "admin@hospital.com");
        if (adminExists)
        {
            _logger.LogInformation("Admin user already seeded");
            return;
        }

        var passwordHash = _passwordHasher.HashPassword("Admin@123");

        var adminUser = User.Create(
            "admin@hospital.com",
            passwordHash,
            "Admin",
            "User",
            "+989131234500",
            new DateTime(1980, 1, 1),
            UserRole.Administrator
        );

        _context.Users.Add(adminUser);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Seeded admin user: admin@hospital.com");
    }

    private async Task SeedDoctorsAsync()
    {
        // Check if doctor users already exist (more reliable than checking Doctors table)
        var doctorUsersExist = await _context.Users.AnyAsync(u => u.Email == "dr.smith@hospital.com");
        if (doctorUsersExist)
        {
            _logger.LogInformation("Doctors already seeded");
            return;
        }

        // If Doctors exist but Users don't, clean up orphaned doctors
        if (await _context.Doctors.AnyAsync())
        {
            _logger.LogWarning("Found orphaned doctor records without users, cleaning up...");
            // Also clean up orphaned availabilities
            var orphanedDoctorIds = await _context.Doctors.Select(d => d.Id).ToListAsync();
            var orphanedAvailabilities = await _context.Availabilities
                .Where(a => orphanedDoctorIds.Contains(a.DoctorId))
                .ToListAsync();
            _context.Availabilities.RemoveRange(orphanedAvailabilities);
            _context.Doctors.RemoveRange(_context.Doctors);
            await _context.SaveChangesAsync();
        }

        var passwordHash = _passwordHasher.HashPassword("Doctor@123");

        var cardiology = await _context.Specialties.FirstAsync(s => s.Name == "Cardiology");
        var dermatology = await _context.Specialties.FirstAsync(s => s.Name == "Dermatology");
        var pediatrics = await _context.Specialties.FirstAsync(s => s.Name == "Pediatrics");
        var orthopedics = await _context.Specialties.FirstAsync(s => s.Name == "Orthopedics");
        var neurology = await _context.Specialties.FirstAsync(s => s.Name == "Neurology");
        var generalPractice = await _context.Specialties.FirstAsync(s => s.Name == "General Practice");

        // Create doctor users and profiles
        var doctorsData = new[]
        {
            new
            {
                Email = "dr.smith@hospital.com",
                FirstName = "John",
                LastName = "Smith",
                Phone = "+989121234567",
                DoB = new DateTime(1975, 5, 15),
                LicenseNumber = "MD-12345",
                Specialty = cardiology,
                Biography = "Experienced cardiologist with 20+ years specializing in heart disease prevention and treatment. Board certified with extensive experience in cardiac imaging and interventional procedures.",
                YearsOfExperience = 20,
                ConsultationFee = 150000m,
                Rating = 4.8m
            },
            new
            {
                Email = "dr.johnson@hospital.com",
                FirstName = "Emily",
                LastName = "Johnson",
                Phone = "+989121234568",
                DoB = new DateTime(1980, 8, 20),
                LicenseNumber = "MD-12346",
                Specialty = dermatology,
                Biography = "Expert dermatologist focusing on skin conditions, cosmetic dermatology, and laser treatments. Passionate about helping patients achieve healthy, beautiful skin.",
                YearsOfExperience = 15,
                ConsultationFee = 120000m,
                Rating = 4.9m
            },
            new
            {
                Email = "dr.williams@hospital.com",
                FirstName = "Michael",
                LastName = "Williams",
                Phone = "+989121234569",
                DoB = new DateTime(1978, 3, 10),
                LicenseNumber = "MD-12347",
                Specialty = pediatrics,
                Biography = "Caring pediatrician dedicated to child health and development. Specializes in preventive care, vaccinations, and treating common childhood illnesses.",
                YearsOfExperience = 18,
                ConsultationFee = 100000m,
                Rating = 4.7m
            },
            new
            {
                Email = "dr.brown@hospital.com",
                FirstName = "Sarah",
                LastName = "Brown",
                Phone = "+989121234570",
                DoB = new DateTime(1982, 11, 25),
                LicenseNumber = "MD-12348",
                Specialty = orthopedics,
                Biography = "Orthopedic surgeon with expertise in joint replacement, sports injuries, and arthroscopic surgery. Committed to restoring mobility and quality of life.",
                YearsOfExperience = 14,
                ConsultationFee = 180000m,
                Rating = 4.6m
            },
            new
            {
                Email = "dr.davis@hospital.com",
                FirstName = "David",
                LastName = "Davis",
                Phone = "+989121234571",
                DoB = new DateTime(1985, 7, 30),
                LicenseNumber = "MD-12349",
                Specialty = neurology,
                Biography = "Neurologist specializing in diagnosis and treatment of brain and nervous system disorders. Expert in managing headaches, epilepsy, and movement disorders.",
                YearsOfExperience = 12,
                ConsultationFee = 160000m,
                Rating = 4.5m
            },
            new
            {
                Email = "dr.miller@hospital.com",
                FirstName = "Lisa",
                LastName = "Miller",
                Phone = "+989121234572",
                DoB = new DateTime(1979, 12, 5),
                LicenseNumber = "MD-12350",
                Specialty = generalPractice,
                Biography = "General practitioner providing comprehensive primary care for patients of all ages. Focus on preventive medicine and chronic disease management.",
                YearsOfExperience = 16,
                ConsultationFee = 80000m,
                Rating = 4.8m
            }
        };

        foreach (var doctorData in doctorsData)
        {
            // Create user
            var user = User.Create(
                doctorData.Email,
                passwordHash,
                doctorData.FirstName,
                doctorData.LastName,
                doctorData.Phone,
                doctorData.DoB,
                UserRole.Doctor
            );

            _context.Users.Add(user);
            await _context.SaveChangesAsync(); // Save to get user ID

            // Create doctor profile
            var doctor = Doctor.Create(
                user.Id,
                doctorData.LicenseNumber,
                doctorData.Specialty.Id,
                doctorData.YearsOfExperience
            );

            doctor.UpdateProfile(doctorData.Biography, doctorData.YearsOfExperience, doctorData.ConsultationFee);
            doctor.UpdateRating(doctorData.Rating);

            _context.Doctors.Add(doctor);
            await _context.SaveChangesAsync(); // Save to get doctor ID

            // Create availabilities (Monday to Friday, 9 AM to 5 PM for most doctors)
            var startHour = doctorData.Email.Contains("johnson") ? 10 : (doctorData.Email.Contains("williams") ? 8 : 9);
            var endHour = doctorData.Email.Contains("johnson") ? 18 : (doctorData.Email.Contains("williams") ? 16 : 17);

            var daysOfWeek = doctorData.Email.Contains("johnson")
                ? new[] { DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday, DayOfWeek.Saturday }
                : new[] { DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday };

            foreach (var day in daysOfWeek)
            {
                var availability = Availability.Create(
                    doctor.Id,
                    day,
                    new TimeSpan(startHour, 0, 0),
                    new TimeSpan(endHour, 0, 0),
                    30
                );

                _context.Availabilities.Add(availability);
            }
        }

        await _context.SaveChangesAsync();

        _logger.LogInformation("Seeded {Count} doctors with availabilities", doctorsData.Length);
    }

    private async Task SeedPatientsAsync()
    {
        // Check if patient users already exist (more reliable than checking Patients table)
        var patientUsersExist = await _context.Users.AnyAsync(u => u.Email == "alice.wilson@email.com");
        if (patientUsersExist)
        {
            _logger.LogInformation("Patients already seeded");
            return;
        }

        // If Patients exist but Users don't, clean up orphaned patients
        if (await _context.Patients.AnyAsync())
        {
            _logger.LogWarning("Found orphaned patient records without users, cleaning up...");
            _context.Patients.RemoveRange(_context.Patients);
            await _context.SaveChangesAsync();
        }

        var passwordHash = _passwordHasher.HashPassword("Patient@123");

        // Create patient users and profiles
        var patientsData = new[]
        {
            new
            {
                Email = "alice.wilson@email.com",
                FirstName = "Alice",
                LastName = "Wilson",
                Phone = "+989131234567",
                DoB = new DateTime(1990, 4, 12),
                MedicalRecordNumber = "MRN-001",
                BloodGroup = "A+",
                Allergies = "Penicillin, Peanuts"
            },
            new
            {
                Email = "bob.anderson@email.com",
                FirstName = "Bob",
                LastName = "Anderson",
                Phone = "+989131234568",
                DoB = new DateTime(1985, 7, 22),
                MedicalRecordNumber = "MRN-002",
                BloodGroup = "O+",
                Allergies = "None"
            },
            new
            {
                Email = "carol.martinez@email.com",
                FirstName = "Carol",
                LastName = "Martinez",
                Phone = "+989131234569",
                DoB = new DateTime(1995, 11, 8),
                MedicalRecordNumber = "MRN-003",
                BloodGroup = "B+",
                Allergies = "Latex, Shellfish"
            },
            new
            {
                Email = "david.thompson@email.com",
                FirstName = "David",
                LastName = "Thompson",
                Phone = "+989131234570",
                DoB = new DateTime(1988, 2, 14),
                MedicalRecordNumber = "MRN-004",
                BloodGroup = "AB+",
                Allergies = "None"
            },
            new
            {
                Email = "emma.garcia@email.com",
                FirstName = "Emma",
                LastName = "Garcia",
                Phone = "+989131234571",
                DoB = new DateTime(1992, 9, 25),
                MedicalRecordNumber = "MRN-005",
                BloodGroup = "A-",
                Allergies = "Aspirin"
            }
        };

        foreach (var patientData in patientsData)
        {
            // Create user
            var user = User.Create(
                patientData.Email,
                passwordHash,
                patientData.FirstName,
                patientData.LastName,
                patientData.Phone,
                patientData.DoB,
                UserRole.Patient
            );

            _context.Users.Add(user);
            await _context.SaveChangesAsync(); // Save to get user ID

            // Create patient profile
            var patient = Patient.Create(user.Id);
            patient.SetMedicalRecordNumber(patientData.MedicalRecordNumber);
            patient.UpdateMedicalInfo(patientData.BloodGroup, patientData.Allergies);

            _context.Patients.Add(patient);
        }

        await _context.SaveChangesAsync();

        _logger.LogInformation("Seeded {Count} patients", patientsData.Length);
    }
}
