# ADR-004: Database and ORM Selection (SQL Server + Entity Framework Core)

## Status
**Accepted** - 2026-01-06

## Context
The Hospital Appointment Management System requires a robust database solution to store and manage:
- User accounts (Patients, Doctors, Administrators)
- Doctor profiles and credentials
- Patient medical information
- Appointments and scheduling data
- Availability and time slots
- Notifications and audit logs

### Requirements

**Functional Requirements**:
- ACID compliance for data integrity
- Support for complex relationships (one-to-many, many-to-many)
- Efficient querying for appointment availability
- Full-text search for doctor names and specialties
- Support for concurrent bookings without conflicts
- Audit trail for compliance

**Non-Functional Requirements**:
- High availability (99.9% uptime)
- Performance (<100ms query response time for p95)
- Scalability (support 10,000+ concurrent users)
- Backup and disaster recovery
- HIPAA/GDPR compliance
- Data encryption at rest and in transit

---

## Decision
We will use **Microsoft SQL Server** as the primary database with **Entity Framework Core 8.0** as the Object-Relational Mapper (ORM).

### Technology Stack
- **Database**: SQL Server 2019 or 2022
- **ORM**: Entity Framework Core 8.0
- **Query Language**: LINQ (Language Integrated Query)
- **Migrations**: EF Core Migrations

---

## Consequences

### Positive Consequences

#### SQL Server Benefits

1. **Enterprise-Grade Reliability**
   - Proven track record in healthcare and enterprise
   - ACID compliance guarantees
   - High availability with Always On availability groups
   - Automatic failover support

2. **Security Features**
   - Transparent Data Encryption (TDE)
   - Row-level security
   - Dynamic data masking
   - Always Encrypted for sensitive data
   - HIPAA and GDPR compliant

3. **Performance**
   - Excellent query optimizer
   - Columnstore indexes for analytics
   - In-memory OLTP for hot tables
   - Query performance insights

4. **Management and Tooling**
   - SQL Server Management Studio (SSMS)
   - Azure Data Studio
   - Comprehensive monitoring tools
   - Excellent backup and restore

5. **Integration with .NET**
   - Native .NET support
   - Optimized for Entity Framework Core
   - SQL Server Profiler for debugging
   - Excellent error messages

6. **Scalability**
   - Vertical scaling (up to 24TB RAM, 640 cores)
   - Horizontal scaling with read replicas
   - Partitioning for large tables
   - Sharding support

7. **Cloud Support**
   - Azure SQL Database (managed service)
   - Automatic backups
   - Geo-replication
   - Easy migration from on-premises

#### Entity Framework Core Benefits

1. **Productivity**
   - LINQ queries (strongly typed)
   - Automatic SQL generation
   - Change tracking
   - Migrations for schema changes

2. **Type Safety**
   - Compile-time checking
   - IntelliSense support
   - Refactoring safety

3. **Flexibility**
   - Code-First or Database-First
   - Raw SQL support when needed
   - Stored procedure support
   - Global query filters

4. **Performance**
   - Query result caching
   - Compiled queries
   - No-tracking queries for reads
   - Batching support

5. **Clean Architecture Support**
   - DbContext as Unit of Work
   - Repository pattern support
   - Testable with InMemory provider

### Negative Consequences

1. **Cost**
   - SQL Server licenses (if on-premises)
   - Azure SQL costs (if cloud)
   - Higher than open-source alternatives

2. **Vendor Lock-In**
   - Some SQL Server-specific features used
   - Migration to other databases requires work
   - Mitigated by using standard SQL through EF Core

3. **Learning Curve**
   - SQL Server administration knowledge required
   - EF Core concepts (tracking, migrations, etc.)
   - Performance tuning expertise needed

4. **ORM Overhead**
   - EF Core adds slight performance overhead vs raw SQL
   - Complex queries may be slower
   - Generated SQL sometimes suboptimal
   - Mitigated by using raw SQL when needed

5. **Windows Bias**
   - Historically Windows-centric
   - SQL Server on Linux is newer
   - Tooling better on Windows

---

## Alternatives Considered

### Alternative 1: PostgreSQL + Entity Framework Core

**Pros**:
- Free and open-source
- Excellent performance
- Advanced features (JSON, full-text search, GIS)
- Works well with EF Core
- Strong community support
- Cross-platform

**Cons**:
- Less mature tooling than SQL Server
- Smaller enterprise support ecosystem
- Team less familiar with PostgreSQL
- Fewer DBAs with PostgreSQL experience
- Less integration with Azure (though available)

**Why Rejected**: While PostgreSQL is excellent and free, SQL Server's superior tooling, enterprise support, and team familiarity make it the better choice for a healthcare application. The licensing cost is justified by reduced operational complexity.

---

### Alternative 2: MongoDB (NoSQL)

**Pros**:
- Flexible schema
- Excellent horizontal scaling
- High write performance
- JSON-native storage
- Easy sharding

**Cons**:
- No ACID transactions (multi-document) until v4.0
- Weak relational support
- Complex queries more difficult
- No foreign key constraints
- Less mature for healthcare applications
- Team unfamiliar with NoSQL patterns

**Why Rejected**: Healthcare appointment data is inherently relational (patients have appointments with doctors). NoSQL's flexibility isn't needed, and relational databases provide better guarantees for data integrity.

---

### Alternative 3: MySQL/MariaDB + Entity Framework Core

**Pros**:
- Free and open-source
- Well-known and widely used
- Good performance
- Works with EF Core
- Large community

**Cons**:
- Less advanced features than SQL Server/PostgreSQL
- Weaker full-text search
- Less robust transactions
- Licensing confusion (MySQL vs MariaDB)
- Tooling not as mature

**Why Rejected**: MySQL doesn't offer significant advantages over SQL Server for our use case, and SQL Server's superior features, security, and enterprise support make it worth the cost.

---

### Alternative 4: Dapper (Micro-ORM) instead of EF Core

**Pros**:
- Faster than EF Core
- Direct SQL control
- Lightweight
- Simple and easy to learn

**Cons**:
- No change tracking
- No migrations
- No LINQ support
- Manual SQL writing
- More boilerplate code
- No automatic relationships

**Why Rejected**: While Dapper is faster, EF Core's productivity benefits (migrations, LINQ, change tracking) outweigh the performance difference for most operations. We can use raw SQL in EF Core for performance-critical queries.

---

### Alternative 5: Raw ADO.NET

**Pros**:
- Maximum control
- Best performance
- No abstraction overhead

**Cons**:
- Very verbose
- Manual parameter handling
- No type safety
- SQL injection risk
- Lots of boilerplate
- No migrations
- Manual mapping

**Why Rejected**: Too low-level and error-prone. EF Core provides type safety, productivity, and maintainability without significant performance penalty.

---

## Technology Comparison Matrix

| Criteria | SQL Server + EF Core | PostgreSQL + EF Core | MongoDB | MySQL + EF Core |
|----------|----------------------|----------------------|---------|-----------------|
| ACID Compliance | ⭐⭐⭐⭐⭐ | ⭐⭐⭐⭐⭐ | ⭐⭐⭐ | ⭐⭐⭐⭐ |
| Performance | ⭐⭐⭐⭐⭐ | ⭐⭐⭐⭐⭐ | ⭐⭐⭐⭐⭐ | ⭐⭐⭐⭐ |
| Tooling | ⭐⭐⭐⭐⭐ | ⭐⭐⭐⭐ | ⭐⭐⭐ | ⭐⭐⭐⭐ |
| .NET Integration | ⭐⭐⭐⭐⭐ | ⭐⭐⭐⭐ | ⭐⭐⭐ | ⭐⭐⭐⭐ |
| Security Features | ⭐⭐⭐⭐⭐ | ⭐⭐⭐⭐ | ⭐⭐⭐ | ⭐⭐⭐ |
| Healthcare Fit | ⭐⭐⭐⭐⭐ | ⭐⭐⭐⭐ | ⭐⭐ | ⭐⭐⭐ |
| Cost | ⭐⭐ | ⭐⭐⭐⭐⭐ | ⭐⭐⭐⭐⭐ | ⭐⭐⭐⭐⭐ |
| Enterprise Support | ⭐⭐⭐⭐⭐ | ⭐⭐⭐ | ⭐⭐⭐⭐ | ⭐⭐⭐ |
| Cloud Integration | ⭐⭐⭐⭐⭐ | ⭐⭐⭐⭐ | ⭐⭐⭐⭐⭐ | ⭐⭐⭐⭐ |

| ORM Criteria | EF Core | Dapper | ADO.NET |
|--------------|---------|--------|---------|
| Productivity | ⭐⭐⭐⭐⭐ | ⭐⭐⭐ | ⭐⭐ |
| Performance | ⭐⭐⭐⭐ | ⭐⭐⭐⭐⭐ | ⭐⭐⭐⭐⭐ |
| Type Safety | ⭐⭐⭐⭐⭐ | ⭐⭐⭐ | ⭐⭐ |
| Maintainability | ⭐⭐⭐⭐⭐ | ⭐⭐⭐ | ⭐⭐ |
| Learning Curve | ⭐⭐⭐ | ⭐⭐⭐⭐⭐ | ⭐⭐ |
| Testing Support | ⭐⭐⭐⭐⭐ | ⭐⭐⭐ | ⭐⭐ |

---

## Implementation Details

### Database Schema

#### Core Tables

```sql
-- Users
CREATE TABLE Users (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    Email NVARCHAR(256) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(MAX) NOT NULL,
    FirstName NVARCHAR(100) NOT NULL,
    LastName NVARCHAR(100) NOT NULL,
    PhoneNumber NVARCHAR(20),
    DateOfBirth DATE NOT NULL,
    Role INT NOT NULL, -- Patient=1, Doctor=2, Admin=3
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2
);

CREATE INDEX IX_Users_Email ON Users(Email);
CREATE INDEX IX_Users_Role ON Users(Role);

-- Doctors
CREATE TABLE Doctors (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    UserId UNIQUEIDENTIFIER NOT NULL UNIQUE,
    LicenseNumber NVARCHAR(50) NOT NULL UNIQUE,
    SpecialtyId UNIQUEIDENTIFIER NOT NULL,
    Biography NVARCHAR(MAX),
    YearsOfExperience INT NOT NULL,
    ConsultationFee DECIMAL(10,2),
    Rating DECIMAL(3,2),
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2,
    FOREIGN KEY (UserId) REFERENCES Users(Id),
    FOREIGN KEY (SpecialtyId) REFERENCES Specialties(Id)
);

CREATE INDEX IX_Doctors_SpecialtyId ON Doctors(SpecialtyId);

-- Appointments
CREATE TABLE Appointments (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    PatientId UNIQUEIDENTIFIER NOT NULL,
    DoctorId UNIQUEIDENTIFIER NOT NULL,
    ScheduledDate DATE NOT NULL,
    StartTime TIME NOT NULL,
    EndTime TIME NOT NULL,
    Status INT NOT NULL, -- Scheduled=1, Confirmed=2, etc.
    Reason NVARCHAR(500) NOT NULL,
    Notes NVARCHAR(MAX),
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2,
    FOREIGN KEY (PatientId) REFERENCES Patients(Id),
    FOREIGN KEY (DoctorId) REFERENCES Doctors(Id)
);

CREATE INDEX IX_Appointments_PatientId ON Appointments(PatientId);
CREATE INDEX IX_Appointments_DoctorId ON Appointments(DoctorId);
CREATE INDEX IX_Appointments_ScheduledDate ON Appointments(ScheduledDate);
CREATE INDEX IX_Appointments_Status ON Appointments(Status);

-- Composite index for availability checking
CREATE INDEX IX_Appointments_Doctor_Date
ON Appointments(DoctorId, ScheduledDate, Status)
INCLUDE (StartTime, EndTime);
```

### Entity Framework Core Configuration

#### DbContext

```csharp
public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Doctor> Doctors { get; set; }
    public DbSet<Patient> Patients { get; set; }
    public DbSet<Appointment> Appointments { get; set; }
    public DbSet<Availability> Availabilities { get; set; }
    public DbSet<Specialty> Specialties { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
```

#### Entity Configuration (Fluent API)

```csharp
public class AppointmentConfiguration : IEntityTypeConfiguration<Appointment>
{
    public void Configure(EntityTypeBuilder<Appointment> builder)
    {
        builder.ToTable("Appointments");

        builder.HasKey(a => a.Id);
        builder.Property(a => a.Id).ValueGeneratedNever();

        // Value Object mapping
        builder.OwnsOne(a => a.TimeSlot, ts =>
        {
            ts.Property(t => t.StartTime).HasColumnName("StartTime").IsRequired();
            ts.Property(t => t.EndTime).HasColumnName("EndTime").IsRequired();
        });

        // Relationships
        builder.HasOne<Patient>()
               .WithMany()
               .HasForeignKey(a => a.PatientId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Doctor>()
               .WithMany()
               .HasForeignKey(a => a.DoctorId)
               .OnDelete(DeleteBehavior.Restrict);

        // Indexes
        builder.HasIndex(a => a.PatientId);
        builder.HasIndex(a => a.DoctorId);
        builder.HasIndex(a => a.ScheduledDate);
        builder.HasIndex(a => new { a.DoctorId, a.ScheduledDate, a.Status });

        // Properties
        builder.Property(a => a.Reason).HasMaxLength(500).IsRequired();
        builder.Property(a => a.Status).HasConversion<int>();
    }
}
```

### Migrations

```bash
# Create migration
dotnet ef migrations add InitialCreate --project Infrastructure --startup-project API

# Update database
dotnet ef database update --project Infrastructure --startup-project API

# Generate SQL script
dotnet ef migrations script --project Infrastructure --startup-project API --output migration.sql
```

---

## Performance Optimization Strategies

### 1. Indexing Strategy

```sql
-- Covering indexes for common queries
CREATE INDEX IX_Doctors_Search
ON Doctors(SpecialtyId, IsActive)
INCLUDE (FirstName, LastName, Rating, ConsultationFee);

-- Filtered index for active appointments only
CREATE INDEX IX_Appointments_Active
ON Appointments(DoctorId, ScheduledDate)
WHERE Status IN (1, 2); -- Scheduled or Confirmed
```

### 2. Query Optimization

```csharp
// Use AsNoTracking for read-only queries
var doctors = await _context.Doctors
    .AsNoTracking()
    .Include(d => d.Specialty)
    .Where(d => d.IsActive)
    .ToListAsync();

// Project to DTOs to fetch only needed columns
var appointments = await _context.Appointments
    .Where(a => a.PatientId == patientId)
    .Select(a => new AppointmentDto
    {
        Id = a.Id,
        DoctorName = a.Doctor.User.FirstName + " " + a.Doctor.User.LastName,
        ScheduledDate = a.ScheduledDate,
        Status = a.Status
    })
    .ToListAsync();

// Use compiled queries for frequently executed queries
private static readonly Func<ApplicationDbContext, Guid, Task<Doctor>> GetDoctorById =
    EF.CompileAsyncQuery((ApplicationDbContext context, Guid id) =>
        context.Doctors
            .Include(d => d.Specialty)
            .FirstOrDefault(d => d.Id == id));
```

### 3. Connection Pooling

```json
// appsettings.json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.;Database=HospitalDB;Trusted_Connection=true;TrustServerCertificate=true;Max Pool Size=100;Min Pool Size=10;"
  }
}
```

### 4. Batching

```csharp
// EF Core batches multiple inserts automatically
var appointments = new List<Appointment>();
for (int i = 0; i < 100; i++)
{
    appointments.Add(new Appointment { /* ... */ });
}

_context.Appointments.AddRange(appointments);
await _context.SaveChangesAsync(); // Single round-trip
```

---

## Backup and Recovery Strategy

### Automated Backups
- **Full Backup**: Daily at 2 AM
- **Differential Backup**: Every 6 hours
- **Transaction Log Backup**: Every 15 minutes
- **Retention**: 30 days

### Disaster Recovery
- **RPO (Recovery Point Objective)**: 15 minutes
- **RTO (Recovery Time Objective)**: 1 hour
- **Geo-Replication**: Secondary region for disaster recovery

---

## Security Configuration

### Connection Security

```csharp
// Always use encrypted connections
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        connectionString,
        sqlOptions =>
        {
            sqlOptions.EnableRetryOnFailure(
                maxRetryCount: 5,
                maxRetryDelay: TimeSpan.FromSeconds(30),
                errorNumbersToAdd: null);
            sqlOptions.CommandTimeout(30);
        }));
```

### Data Encryption

```sql
-- Enable Transparent Data Encryption
ALTER DATABASE HospitalAppointmentDB
SET ENCRYPTION ON;

-- Column-level encryption for sensitive data
CREATE COLUMN MASTER KEY
WITH (
    KEY_STORE_PROVIDER_NAME = 'MSSQL_CERTIFICATE_STORE',
    KEY_PATH = 'CurrentUser/My/thumbprint'
);
```

---

## Monitoring and Maintenance

### Performance Monitoring
- SQL Server Profiler for slow queries
- Query Store for historical performance
- Azure SQL Insights for cloud deployments
- Application Performance Monitoring (APM) integration

### Maintenance Tasks
- **Index Rebuilding**: Weekly
- **Statistics Update**: Daily
- **Database Integrity Check**: Weekly
- **Log File Management**: Automatic

---

## Testing Strategy

### Unit Testing with InMemory Provider

```csharp
public class AppointmentRepositoryTests
{
    private ApplicationDbContext GetInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        return new ApplicationDbContext(options);
    }

    [Fact]
    public async Task CreateAppointment_SavesSuccessfully()
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        var appointment = new Appointment { /* ... */ };

        // Act
        context.Appointments.Add(appointment);
        await context.SaveChangesAsync();

        // Assert
        Assert.Equal(1, await context.Appointments.CountAsync());
    }
}
```

### Integration Testing with Test Containers

```csharp
public class AppointmentIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public AppointmentIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                // Use test database
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));

                services.Remove(descriptor);

                services.AddDbContext<ApplicationDbContext>(options =>
                {
                    options.UseSqlServer("Server=localhost;Database=TestDB;...");
                });
            });
        });
    }

    [Fact]
    public async Task CreateAppointment_ReturnsCreated()
    {
        // Arrange
        var client = _factory.CreateClient();
        var command = new CreateAppointmentCommand { /* ... */ };

        // Act
        var response = await client.PostAsJsonAsync("/api/appointments", command);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }
}
```

---

## Cost Estimation

### Development
- **SQL Server Developer Edition**: Free
- **Azure SQL Database (Dev/Test)**: ~$5-50/month

### Production
- **SQL Server Standard**: ~$900/core + Windows Server
- **SQL Server Enterprise**: ~$14,000/core + Windows Server
- **Azure SQL Database**: ~$200-2000/month (depending on tier)
- **Azure SQL Managed Instance**: ~$800-5000/month

**Recommendation**: Start with Azure SQL Database (General Purpose tier) for easier management and scaling.

---

## Migration Path

### Phase 1: Initial Setup
- Design database schema
- Create EF Core entities and configurations
- Generate initial migration
- Seed reference data (specialties)

### Phase 2: Development
- Use SQL Server LocalDB or Developer Edition
- Apply migrations automatically on startup (dev only)
- Use InMemory provider for unit tests

### Phase 3: Production
- Deploy to Azure SQL Database
- Setup automated backups
- Configure monitoring
- Implement performance tuning

---

## References

- [SQL Server Documentation](https://docs.microsoft.com/en-us/sql/)
- [Entity Framework Core Documentation](https://docs.microsoft.com/en-us/ef/core/)
- [Azure SQL Database Documentation](https://docs.microsoft.com/en-us/azure/azure-sql/)
- [EF Core Performance](https://docs.microsoft.com/en-us/ef/core/performance/)

---

## Reviewers and Stakeholders

- **Proposed by**: System Architect, Database Administrator
- **Reviewed by**: Development Team, DevOps Team
- **Approved by**: CTO
- **Date**: 2026-01-06

---

## Revision History

| Version | Date | Description | Author |
|---------|------|-------------|--------|
| 1.0 | 2026-01-06 | Initial ADR | System Architect |
