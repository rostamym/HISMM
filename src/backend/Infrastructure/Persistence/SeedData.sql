-- Seed data for Hospital Appointment System

-- First, let's check if we need to seed data
IF NOT EXISTS (SELECT 1 FROM Specialties)
BEGIN
    -- Insert Specialties
    INSERT INTO Specialties (Id, Name, Description, IsActive, CreatedAt, UpdatedAt)
    VALUES
        (NEWID(), 'Cardiology', 'Heart and cardiovascular system specialists', 1, GETUTCDATE(), GETUTCDATE()),
        (NEWID(), 'Dermatology', 'Skin, hair, and nail specialists', 1, GETUTCDATE(), GETUTCDATE()),
        (NEWID(), 'Pediatrics', 'Child healthcare specialists', 1, GETUTCDATE(), GETUTCDATE()),
        (NEWID(), 'Orthopedics', 'Bone, joint, and muscle specialists', 1, GETUTCDATE(), GETUTCDATE()),
        (NEWID(), 'Neurology', 'Nervous system specialists', 1, GETUTCDATE(), GETUTCDATE()),
        (NEWID(), 'General Practice', 'General medical practitioners', 1, GETUTCDATE(), GETUTCDATE()),
        (NEWID(), 'Ophthalmology', 'Eye care specialists', 1, GETUTCDATE(), GETUTCDATE()),
        (NEWID(), 'Dentistry', 'Dental care specialists', 1, GETUTCDATE(), GETUTCDATE());

    PRINT 'Specialties seeded successfully';
END

-- Get specialty IDs for doctors
DECLARE @CardiologyId UNIQUEIDENTIFIER = (SELECT TOP 1 Id FROM Specialties WHERE Name = 'Cardiology');
DECLARE @DermatologyId UNIQUEIDENTIFIER = (SELECT TOP 1 Id FROM Specialties WHERE Name = 'Dermatology');
DECLARE @PediatricsId UNIQUEIDENTIFIER = (SELECT TOP 1 Id FROM Specialties WHERE Name = 'Pediatrics');
DECLARE @OrthopedicsId UNIQUEIDENTIFIER = (SELECT TOP 1 Id FROM Specialties WHERE Name = 'Orthopedics');
DECLARE @NeurologyId UNIQUEIDENTIFIER = (SELECT TOP 1 Id FROM Specialties WHERE Name = 'Neurology');
DECLARE @GeneralPracticeId UNIQUEIDENTIFIER = (SELECT TOP 1 Id FROM Specialties WHERE Name = 'General Practice');

-- Check if we need to seed users and doctors
IF NOT EXISTS (SELECT 1 FROM Users WHERE Email = 'dr.smith@hospital.com')
BEGIN
    -- Insert Doctor Users (Password: Doctor@123 - BCrypt hashed)
    DECLARE @DrSmithUserId UNIQUEIDENTIFIER = NEWID();
    DECLARE @DrJohnsonUserId UNIQUEIDENTIFIER = NEWID();
    DECLARE @DrWilliamsUserId UNIQUEIDENTIFIER = NEWID();
    DECLARE @DrBrownUserId UNIQUEIDENTIFIER = NEWID();
    DECLARE @DrDavisUserId UNIQUEIDENTIFIER = NEWID();
    DECLARE @DrMillerUserId UNIQUEIDENTIFIER = NEWID();

    -- BCrypt hash for "Doctor@123"
    DECLARE @DoctorPasswordHash NVARCHAR(100) = '$2a$11$N0vFQl5YH5JlM8K3j8.YZuFjK8K3j8.YZuFjK8K3j8.YZuFjK8K3j8';

    INSERT INTO Users (Id, Email, PasswordHash, FirstName, LastName, PhoneNumber, DateOfBirth, Role, IsActive, CreatedAt, UpdatedAt)
    VALUES
        (@DrSmithUserId, 'dr.smith@hospital.com', @DoctorPasswordHash, 'John', 'Smith', '+989121234567', '1975-05-15', 1, 1, GETUTCDATE(), GETUTCDATE()),
        (@DrJohnsonUserId, 'dr.johnson@hospital.com', @DoctorPasswordHash, 'Emily', 'Johnson', '+989121234568', '1980-08-20', 1, 1, GETUTCDATE(), GETUTCDATE()),
        (@DrWilliamsUserId, 'dr.williams@hospital.com', @DoctorPasswordHash, 'Michael', 'Williams', '+989121234569', '1978-03-10', 1, 1, GETUTCDATE(), GETUTCDATE()),
        (@DrBrownUserId, 'dr.brown@hospital.com', @DoctorPasswordHash, 'Sarah', 'Brown', '+989121234570', '1982-11-25', 1, 1, GETUTCDATE(), GETUTCDATE()),
        (@DrDavisUserId, 'dr.davis@hospital.com', @DoctorPasswordHash, 'David', 'Davis', '+989121234571', '1985-07-30', 1, 1, GETUTCDATE(), GETUTCDATE()),
        (@DrMillerUserId, 'dr.miller@hospital.com', @DoctorPasswordHash, 'Lisa', 'Miller', '+989121234572', '1979-12-05', 1, 1, GETUTCDATE(), GETUTCDATE());

    -- Insert Doctors
    INSERT INTO Doctors (Id, UserId, LicenseNumber, SpecialtyId, Biography, YearsOfExperience, ConsultationFee, Rating, CreatedAt, UpdatedAt)
    VALUES
        (NEWID(), @DrSmithUserId, 'MD-12345', @CardiologyId,
         'Experienced cardiologist with 20+ years specializing in heart disease prevention and treatment. Board certified with extensive experience in cardiac imaging and interventional procedures.',
         20, 150000, 4.8, GETUTCDATE(), GETUTCDATE()),

        (NEWID(), @DrJohnsonUserId, 'MD-12346', @DermatologyId,
         'Expert dermatologist focusing on skin conditions, cosmetic dermatology, and laser treatments. Passionate about helping patients achieve healthy, beautiful skin.',
         15, 120000, 4.9, GETUTCDATE(), GETUTCDATE()),

        (NEWID(), @DrWilliamsUserId, 'MD-12347', @PediatricsId,
         'Caring pediatrician dedicated to child health and development. Specializes in preventive care, vaccinations, and treating common childhood illnesses.',
         18, 100000, 4.7, GETUTCDATE(), GETUTCDATE()),

        (NEWID(), @DrBrownUserId, 'MD-12348', @OrthopedicsId,
         'Orthopedic surgeon with expertise in joint replacement, sports injuries, and arthroscopic surgery. Committed to restoring mobility and quality of life.',
         14, 180000, 4.6, GETUTCDATE(), GETUTCDATE()),

        (NEWID(), @DrDavisUserId, 'MD-12349', @NeurologyId,
         'Neurologist specializing in diagnosis and treatment of brain and nervous system disorders. Expert in managing headaches, epilepsy, and movement disorders.',
         12, 160000, 4.5, GETUTCDATE(), GETUTCDATE()),

        (NEWID(), @DrMillerUserId, 'MD-12350', @GeneralPracticeId,
         'General practitioner providing comprehensive primary care for patients of all ages. Focus on preventive medicine and chronic disease management.',
         16, 80000, 4.8, GETUTCDATE(), GETUTCDATE());

    PRINT 'Doctor users and profiles seeded successfully';
END

-- Insert sample availabilities for doctors
DECLARE @DrSmithDoctorId UNIQUEIDENTIFIER = (SELECT TOP 1 d.Id FROM Doctors d INNER JOIN Users u ON d.UserId = u.Id WHERE u.Email = 'dr.smith@hospital.com');
DECLARE @DrJohnsonDoctorId UNIQUEIDENTIFIER = (SELECT TOP 1 d.Id FROM Doctors d INNER JOIN Users u ON d.UserId = u.Id WHERE u.Email = 'dr.johnson@hospital.com');
DECLARE @DrWilliamsDoctorId UNIQUEIDENTIFIER = (SELECT TOP 1 d.Id FROM Doctors d INNER JOIN Users u ON d.UserId = u.Id WHERE u.Email = 'dr.williams@hospital.com');

IF @DrSmithDoctorId IS NOT NULL AND NOT EXISTS (SELECT 1 FROM Availabilities WHERE DoctorId = @DrSmithDoctorId)
BEGIN
    -- Dr. Smith availability (Monday to Friday, 9 AM to 5 PM)
    INSERT INTO Availabilities (Id, DoctorId, DayOfWeek, StartTime, EndTime, SlotDurationMinutes, IsActive, CreatedAt, UpdatedAt)
    VALUES
        (NEWID(), @DrSmithDoctorId, 1, '09:00:00', '17:00:00', 30, 1, GETUTCDATE(), GETUTCDATE()),
        (NEWID(), @DrSmithDoctorId, 2, '09:00:00', '17:00:00', 30, 1, GETUTCDATE(), GETUTCDATE()),
        (NEWID(), @DrSmithDoctorId, 3, '09:00:00', '17:00:00', 30, 1, GETUTCDATE(), GETUTCDATE()),
        (NEWID(), @DrSmithDoctorId, 4, '09:00:00', '17:00:00', 30, 1, GETUTCDATE(), GETUTCDATE()),
        (NEWID(), @DrSmithDoctorId, 5, '09:00:00', '17:00:00', 30, 1, GETUTCDATE(), GETUTCDATE());

    -- Dr. Johnson availability (Tuesday to Saturday, 10 AM to 6 PM)
    INSERT INTO Availabilities (Id, DoctorId, DayOfWeek, StartTime, EndTime, SlotDurationMinutes, IsActive, CreatedAt, UpdatedAt)
    VALUES
        (NEWID(), @DrJohnsonDoctorId, 2, '10:00:00', '18:00:00', 30, 1, GETUTCDATE(), GETUTCDATE()),
        (NEWID(), @DrJohnsonDoctorId, 3, '10:00:00', '18:00:00', 30, 1, GETUTCDATE(), GETUTCDATE()),
        (NEWID(), @DrJohnsonDoctorId, 4, '10:00:00', '18:00:00', 30, 1, GETUTCDATE(), GETUTCDATE()),
        (NEWID(), @DrJohnsonDoctorId, 5, '10:00:00', '18:00:00', 30, 1, GETUTCDATE(), GETUTCDATE()),
        (NEWID(), @DrJohnsonDoctorId, 6, '10:00:00', '18:00:00', 30, 1, GETUTCDATE(), GETUTCDATE());

    -- Dr. Williams availability (Monday to Friday, 8 AM to 4 PM)
    INSERT INTO Availabilities (Id, DoctorId, DayOfWeek, StartTime, EndTime, SlotDurationMinutes, IsActive, CreatedAt, UpdatedAt)
    VALUES
        (NEWID(), @DrWilliamsDoctorId, 1, '08:00:00', '16:00:00', 30, 1, GETUTCDATE(), GETUTCDATE()),
        (NEWID(), @DrWilliamsDoctorId, 2, '08:00:00', '16:00:00', 30, 1, GETUTCDATE(), GETUTCDATE()),
        (NEWID(), @DrWilliamsDoctorId, 3, '08:00:00', '16:00:00', 30, 1, GETUTCDATE(), GETUTCDATE()),
        (NEWID(), @DrWilliamsDoctorId, 4, '08:00:00', '16:00:00', 30, 1, GETUTCDATE(), GETUTCDATE()),
        (NEWID(), @DrWilliamsDoctorId, 5, '08:00:00', '16:00:00', 30, 1, GETUTCDATE(), GETUTCDATE());

    PRINT 'Doctor availabilities seeded successfully';
END

PRINT 'Seed data process completed';
