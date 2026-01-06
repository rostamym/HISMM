# C4 Model - Level 4: Code Diagram
## Hospital Appointment Management System

### Overview
The Code (Class) diagram shows the detailed implementation structure at the code level. This level provides class diagrams showing the relationships between key classes, their properties, methods, and inheritance hierarchies. This document focuses on the Domain layer as it contains the core business logic.

---

## Domain Model - Class Diagram

### Core Domain Entities

```
┌────────────────────────────────────────────────────────────────┐
│                        BaseEntity                              │
│                       (Abstract)                               │
├────────────────────────────────────────────────────────────────┤
│ + Id: Guid                                                     │
│ + CreatedAt: DateTime                                          │
│ + UpdatedAt: DateTime                                          │
│ + CreatedBy: string                                            │
│ + UpdatedBy: string                                            │
├────────────────────────────────────────────────────────────────┤
│                                                                │
└────────────────────────────────────────────────────────────────┘
                               ▲
                               │ Inherits
        ┌──────────────────────┼──────────────────────┐
        │                      │                      │
        │                      │                      │
┌───────▼────────┐    ┌────────▼──────┐    ┌─────────▼────────┐
│     User       │    │    Doctor     │    │    Patient       │
├────────────────┤    ├───────────────┤    ├──────────────────┤
│- _email: Email │    │- _userId      │    │- _userId         │
│- _firstName    │    │- _licenseNum  │    │- _medicalRecord  │
│- _lastName     │    │- _specialtyId │    │- _bloodGroup     │
│- _phone:       │    │- _biography   │    │- _allergies      │
│  PhoneNumber   │    │- _yearsOfExp  │    │                  │
│- _dateOfBirth  │    │- _fee         │    │                  │
│- _role:        │    │- _rating      │    │                  │
│  UserRole      │    │- _availabili  │    │                  │
│- _isActive     │    │  ties: List   │    │                  │
├────────────────┤    ├───────────────┤    ├──────────────────┤
│+ Create()      │    │+ Create()     │    │+ Create()        │
│+ Update()      │    │+ SetAvail()   │    │+ UpdateInfo()    │
│+ Deactivate()  │    │+ AddAvail()   │    │                  │
│+ Activate()    │    │+ BlockDate()  │    │                  │
└────────────────┘    └───────────────┘    └──────────────────┘
        │                     │                      │
        │                     │                      │
        │                     │                      │
        │                     └──────────┬───────────┘
        │                                │
        │                                │ References
        │                                │
        │                     ┌──────────▼───────────┐
        │                     │    Appointment       │
        │                     ├──────────────────────┤
        │                     │- _patientId: Guid    │
        │                     │- _doctorId: Guid     │
        │                     │- _scheduledDate      │
        │                     │- _timeSlot: TimeSlot │
        │                     │- _status: Status     │
        │                     │- _reason: string     │
        │                     │- _notes: string      │
        │                     ├──────────────────────┤
        │                     │+ Create()            │
        │                     │+ Cancel()            │
        │                     │+ Reschedule()        │
        │                     │+ Complete()          │
        │                     │+ AddNotes()          │
        │                     └──────────────────────┘
        │
        │
┌───────▼──────────────────────────────────────────────────────┐
│                      Specialty                               │
├──────────────────────────────────────────────────────────────┤
│+ Id: Guid                                                    │
│- _name: string                                               │
│- _description: string                                        │
│- _isActive: bool                                             │
├──────────────────────────────────────────────────────────────┤
│+ Create()                                                    │
└──────────────────────────────────────────────────────────────┘


┌──────────────────────────────────────────────────────────────┐
│                      Availability                            │
├──────────────────────────────────────────────────────────────┤
│+ Id: Guid                                                    │
│- _doctorId: Guid                                             │
│- _dayOfWeek: DayOfWeek                                       │
│- _startTime: TimeSpan                                        │
│- _endTime: TimeSpan                                          │
│- _slotDurationMinutes: int                                   │
│- _isActive: bool                                             │
├──────────────────────────────────────────────────────────────┤
│+ Create()                                                    │
│+ Deactivate()                                                │
│+ GetTimeSlots(): List<TimeSlot>                              │
│+ IsAvailable(time): bool                                     │
└──────────────────────────────────────────────────────────────┘


┌──────────────────────────────────────────────────────────────┐
│                      DateBlock                               │
├──────────────────────────────────────────────────────────────┤
│+ Id: Guid                                                    │
│- _doctorId: Guid                                             │
│- _blockedDate: DateTime                                      │
│- _reason: string                                             │
├──────────────────────────────────────────────────────────────┤
│+ Create()                                                    │
│+ Remove()                                                    │
└──────────────────────────────────────────────────────────────┘


┌──────────────────────────────────────────────────────────────┐
│                      Notification                            │
├──────────────────────────────────────────────────────────────┤
│+ Id: Guid                                                    │
│- _userId: Guid                                               │
│- _type: NotificationType                                     │
│- _subject: string                                            │
│- _body: string                                               │
│- _sentAt: DateTime?                                          │
│- _status: NotificationStatus                                 │
├──────────────────────────────────────────────────────────────┤
│+ Create()                                                    │
│+ MarkAsSent()                                                │
│+ MarkAsFailed()                                              │
└──────────────────────────────────────────────────────────────┘
```

---

## Value Objects

```
┌──────────────────────────────────────────────────────────────┐
│                         Email                                │
│                      (Value Object)                          │
├──────────────────────────────────────────────────────────────┤
│- _value: string                                              │
├──────────────────────────────────────────────────────────────┤
│+ Email(string value)                                         │
│+ Value: string { get; }                                      │
│- Validate(): void                                            │
│+ Equals(Email other): bool                                   │
│+ GetHashCode(): int                                          │
│+ ToString(): string                                          │
└──────────────────────────────────────────────────────────────┘


┌──────────────────────────────────────────────────────────────┐
│                      PhoneNumber                             │
│                     (Value Object)                           │
├──────────────────────────────────────────────────────────────┤
│- _value: string                                              │
│- _countryCode: string                                        │
├──────────────────────────────────────────────────────────────┤
│+ PhoneNumber(string value, string countryCode)              │
│+ Value: string { get; }                                      │
│+ CountryCode: string { get; }                                │
│+ FullNumber: string { get; }                                 │
│- Validate(): void                                            │
│+ Equals(PhoneNumber other): bool                            │
│+ GetHashCode(): int                                          │
└──────────────────────────────────────────────────────────────┘


┌──────────────────────────────────────────────────────────────┐
│                        TimeSlot                              │
│                     (Value Object)                           │
├──────────────────────────────────────────────────────────────┤
│- _startTime: TimeSpan                                        │
│- _endTime: TimeSpan                                          │
├──────────────────────────────────────────────────────────────┤
│+ TimeSlot(TimeSpan start, TimeSpan end)                      │
│+ StartTime: TimeSpan { get; }                                │
│+ EndTime: TimeSpan { get; }                                  │
│+ Duration: TimeSpan { get; }                                 │
│+ OverlapsWith(TimeSlot other): bool                          │
│- Validate(): void                                            │
│+ Equals(TimeSlot other): bool                                │
│+ ToString(): string                                          │
└──────────────────────────────────────────────────────────────┘
```

---

## Enumerations

```csharp
public enum UserRole
{
    Patient = 1,
    Doctor = 2,
    Administrator = 3
}

public enum AppointmentStatus
{
    Scheduled = 1,
    Confirmed = 2,
    InProgress = 3,
    Completed = 4,
    Cancelled = 5,
    NoShow = 6
}

public enum NotificationType
{
    Email = 1,
    SMS = 2,
    Push = 3
}

public enum NotificationStatus
{
    Pending = 1,
    Sent = 2,
    Failed = 3,
    Retrying = 4
}

public enum DayOfWeek
{
    Sunday = 0,
    Monday = 1,
    Tuesday = 2,
    Wednesday = 3,
    Thursday = 4,
    Friday = 5,
    Saturday = 6
}
```

---

## Domain Events

```
┌──────────────────────────────────────────────────────────────┐
│                      BaseDomainEvent                         │
│                        (Abstract)                            │
├──────────────────────────────────────────────────────────────┤
│+ DateOccurred: DateTime                                      │
└──────────────────────────────────────────────────────────────┘
                               ▲
                               │ Inherits
        ┌──────────────────────┼──────────────────────┐
        │                      │                      │
┌───────▼───────────┐  ┌───────▼──────────┐  ┌───────▼───────────┐
│AppointmentBooked  │  │AppointmentCancell│  │AppointmentCompleted│
│     Event         │  │     edEvent      │  │      Event         │
├───────────────────┤  ├──────────────────┤  ├────────────────────┤
│+ AppointmentId    │  │+ AppointmentId   │  │+ AppointmentId     │
│+ PatientId        │  │+ PatientId       │  │+ PatientId         │
│+ DoctorId         │  │+ DoctorId        │  │+ DoctorId          │
│+ ScheduledDate    │  │+ CancelledBy     │  │+ CompletedAt       │
│+ TimeSlot         │  │+ Reason          │  │                    │
└───────────────────┘  └──────────────────┘  └────────────────────┘
```

---

## Application Layer - CQRS Implementation

### Commands

```
┌──────────────────────────────────────────────────────────────┐
│                   IRequest<TResponse>                        │
│                   (MediatR Interface)                        │
└──────────────────────────────────────────────────────────────┘
                               ▲
                               │ Implements
        ┌──────────────────────┼──────────────────────┐
        │                      │                      │
┌───────▼────────────┐  ┌──────▼──────────┐  ┌───────▼─────────┐
│CreateAppointment   │  │CancelAppointment│  │RescheduleAppoint│
│    Command         │  │    Command      │  │   mentCommand   │
├────────────────────┤  ├─────────────────┤  ├─────────────────┤
│+ PatientId: Guid   │  │+ AppointmentId  │  │+ AppointmentId  │
│+ DoctorId: Guid    │  │+ Reason: string │  │+ NewDate        │
│+ ScheduledDate     │  │+ CancelledBy    │  │+ NewTimeSlot    │
│+ TimeSlot          │  │                 │  │                 │
│+ Reason: string    │  │                 │  │                 │
└────────────────────┘  └─────────────────┘  └─────────────────┘
```

### Command Handlers

```
┌──────────────────────────────────────────────────────────────┐
│          IRequestHandler<TRequest, TResponse>                │
│                   (MediatR Interface)                        │
└──────────────────────────────────────────────────────────────┘
                               ▲
                               │ Implements
        ┌──────────────────────┼──────────────────────┐
        │                      │                      │
┌───────▼───────────────────┐  ┌────▼─────────────────────┐
│CreateAppointmentCommand   │  │CancelAppointmentCommand  │
│        Handler            │  │        Handler           │
├───────────────────────────┤  ├──────────────────────────┤
│- _context: IDbContext     │  │- _context: IDbContext    │
│- _emailService            │  │- _emailService           │
├───────────────────────────┤  ├──────────────────────────┤
│+ Handle(command, token)   │  │+ Handle(command, token)  │
│  Returns: Result<Guid>    │  │  Returns: Result<Unit>   │
│                           │  │                          │
│Steps:                     │  │Steps:                    │
│1. Validate availability   │  │1. Load appointment       │
│2. Check conflicts         │  │2. Validate cancellation  │
│3. Create appointment      │  │3. Update status          │
│4. Save to database        │  │4. Raise event            │
│5. Raise event             │  │5. Save changes           │
│6. Return appointmentId    │  │6. Return success         │
└───────────────────────────┘  └──────────────────────────┘
```

### Queries

```
┌──────────────────────────────────────────────────────────────┐
│                   IRequest<TResponse>                        │
│                   (MediatR Interface)                        │
└──────────────────────────────────────────────────────────────┘
                               ▲
                               │ Implements
        ┌──────────────────────┼──────────────────────┐
        │                      │                      │
┌───────▼────────────┐  ┌──────▼─────────┐  ┌────────▼────────┐
│  GetDoctorsQuery   │  │SearchDoctors   │  │GetAppointments  │
│                    │  │    Query       │  │    Query        │
├────────────────────┤  ├────────────────┤  ├─────────────────┤
│+ PageNumber: int   │  │+ SearchTerm    │  │+ PatientId      │
│+ PageSize: int     │  │+ SpecialtyId   │  │+ StartDate      │
│+ SpecialtyId: Guid?│  │+ Location      │  │+ EndDate        │
│                    │  │+ MinRating     │  │+ Status         │
└────────────────────┘  └────────────────┘  └─────────────────┘
```

### Query Handlers

```
┌──────────────────────────────────────────────────────────────┐
│             IRequestHandler<TRequest, TResponse>             │
│                   (MediatR Interface)                        │
└──────────────────────────────────────────────────────────────┘
                               ▲
                               │ Implements
┌──────────────────────────────▼───────────────────────────────┐
│              GetDoctorsQueryHandler                          │
├──────────────────────────────────────────────────────────────┤
│- _context: IDbContext                                        │
│- _mapper: IMapper                                            │
├──────────────────────────────────────────────────────────────┤
│+ Handle(query, token)                                        │
│  Returns: Result<PaginatedList<DoctorDto>>                   │
│                                                              │
│Steps:                                                        │
│1. Build query with filters                                  │
│2. Apply pagination                                          │
│3. Execute query                                             │
│4. Map to DTOs                                               │
│5. Return paginated result                                   │
└──────────────────────────────────────────────────────────────┘
```

---

## Validators (FluentValidation)

```
┌──────────────────────────────────────────────────────────────┐
│              AbstractValidator<T>                            │
│              (FluentValidation)                              │
└──────────────────────────────────────────────────────────────┘
                               ▲
                               │ Inherits
┌──────────────────────────────▼───────────────────────────────┐
│        CreateAppointmentCommandValidator                     │
├──────────────────────────────────────────────────────────────┤
│+ CreateAppointmentCommandValidator()                         │
│  {                                                           │
│    RuleFor(x => x.PatientId).NotEmpty();                     │
│    RuleFor(x => x.DoctorId).NotEmpty();                      │
│    RuleFor(x => x.ScheduledDate)                             │
│      .GreaterThan(DateTime.Now);                             │
│    RuleFor(x => x.Reason)                                    │
│      .NotEmpty()                                             │
│      .MaximumLength(500);                                    │
│  }                                                           │
└──────────────────────────────────────────────────────────────┘
```

---

## Infrastructure Layer - Data Access

```
┌──────────────────────────────────────────────────────────────┐
│                       DbContext                              │
│                   (Entity Framework Core)                    │
└──────────────────────────────────────────────────────────────┘
                               ▲
                               │ Inherits
┌──────────────────────────────▼───────────────────────────────┐
│                 ApplicationDbContext                         │
├──────────────────────────────────────────────────────────────┤
│+ Users: DbSet<User>                                          │
│+ Doctors: DbSet<Doctor>                                      │
│+ Patients: DbSet<Patient>                                    │
│+ Appointments: DbSet<Appointment>                            │
│+ Availabilities: DbSet<Availability>                         │
│+ Specialties: DbSet<Specialty>                               │
│+ Notifications: DbSet<Notification>                          │
├──────────────────────────────────────────────────────────────┤
│# OnModelCreating(ModelBuilder builder)                       │
│  {                                                           │
│    builder.ApplyConfigurationsFromAssembly();                │
│  }                                                           │
│                                                              │
│+ SaveChangesAsync(CancellationToken): Task<int>             │
└──────────────────────────────────────────────────────────────┘
```

### Entity Configurations

```
┌──────────────────────────────────────────────────────────────┐
│            IEntityTypeConfiguration<TEntity>                 │
│                 (Entity Framework Core)                      │
└──────────────────────────────────────────────────────────────┘
                               ▲
                               │ Implements
┌──────────────────────────────▼───────────────────────────────┐
│                 AppointmentConfiguration                     │
├──────────────────────────────────────────────────────────────┤
│+ Configure(EntityTypeBuilder<Appointment> builder)          │
│  {                                                           │
│    builder.ToTable("Appointments");                          │
│    builder.HasKey(x => x.Id);                                │
│    builder.Property(x => x.Id).ValueGeneratedNever();        │
│                                                              │
│    // Value Object mapping                                  │
│    builder.OwnsOne(x => x.TimeSlot, ts => {                 │
│      ts.Property(t => t.StartTime).HasColumnName("StartTime");│
│      ts.Property(t => t.EndTime).HasColumnName("EndTime");   │
│    });                                                       │
│                                                              │
│    // Relationships                                          │
│    builder.HasOne<Patient>()                                 │
│           .WithMany()                                        │
│           .HasForeignKey(x => x.PatientId);                  │
│                                                              │
│    builder.HasOne<Doctor>()                                  │
│           .WithMany()                                        │
│           .HasForeignKey(x => x.DoctorId);                   │
│                                                              │
│    // Indexes                                                │
│    builder.HasIndex(x => x.PatientId);                       │
│    builder.HasIndex(x => x.DoctorId);                        │
│    builder.HasIndex(x => x.ScheduledDate);                   │
│  }                                                           │
└──────────────────────────────────────────────────────────────┘
```

---

## Angular Frontend - Key Classes

### Services

```typescript
@Injectable({ providedIn: 'root' })
export class AuthService {
  private currentUserSubject: BehaviorSubject<User | null>;
  public currentUser$: Observable<User | null>;

  constructor(
    private http: HttpClient,
    private tokenService: TokenService
  ) {
    this.currentUserSubject = new BehaviorSubject<User | null>(
      this.getUserFromToken()
    );
    this.currentUser$ = this.currentUserSubject.asObservable();
  }

  public get currentUserValue(): User | null {
    return this.currentUserSubject.value;
  }

  login(credentials: LoginDto): Observable<AuthResponse> {
    return this.http.post<AuthResponse>('/api/auth/login', credentials)
      .pipe(
        map(response => {
          this.tokenService.setToken(response.token);
          this.currentUserSubject.next(response.user);
          return response;
        })
      );
  }

  logout(): void {
    this.tokenService.removeToken();
    this.currentUserSubject.next(null);
  }

  isAuthenticated(): boolean {
    return !!this.tokenService.getToken();
  }

  private getUserFromToken(): User | null {
    const token = this.tokenService.getToken();
    if (!token) return null;
    return this.tokenService.decodeToken(token);
  }
}
```

### Models

```typescript
export interface User {
  id: string;
  email: string;
  firstName: string;
  lastName: string;
  role: UserRole;
}

export interface Doctor extends User {
  licenseNumber: string;
  specialtyId: string;
  specialtyName: string;
  biography: string;
  yearsOfExperience: number;
  rating: number;
}

export interface Appointment {
  id: string;
  patientId: string;
  doctorId: string;
  doctorName: string;
  scheduledDate: Date;
  startTime: string;
  endTime: string;
  status: AppointmentStatus;
  reason: string;
  notes?: string;
}

export enum UserRole {
  Patient = 1,
  Doctor = 2,
  Administrator = 3
}

export enum AppointmentStatus {
  Scheduled = 1,
  Confirmed = 2,
  InProgress = 3,
  Completed = 4,
  Cancelled = 5,
  NoShow = 6
}
```

---

## Dependency Injection Configuration

### Backend (Program.cs)

```csharp
// Domain Layer - No dependencies

// Application Layer
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(CreateAppointmentCommand).Assembly));
builder.Services.AddValidatorsFromAssembly(typeof(CreateAppointmentCommandValidator).Assembly);
builder.Services.AddAutoMapper(typeof(MappingProfile).Assembly);

// Infrastructure Layer
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<IIdentityService, IdentityService>();

// Presentation Layer
builder.Services.AddControllers();
builder.Services.AddSwaggerGen();
```

---

## Document Metadata

- **Document Version**: 1.0
- **Last Updated**: 2026-01-06
- **Author**: System Architect
- **Status**: Draft
- **C4 Level**: 4 (Code)
