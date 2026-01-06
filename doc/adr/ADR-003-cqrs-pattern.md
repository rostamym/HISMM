# ADR-003: Implement CQRS Pattern with MediatR

## Status
**Accepted** - 2026-01-06

## Context
The Hospital Appointment Management System needs a clear separation between read (query) and write (command) operations. The application has different requirements for these operations:

### Read Operations (Queries)
- Searching doctors by specialty, location, availability
- Viewing appointment lists with complex filtering
- Generating reports and analytics
- Dashboard data aggregation
- High read frequency (80% of operations)

### Write Operations (Commands)
- Creating appointments
- Updating doctor availability
- Cancelling appointments
- User registration

### Problem Statement
We need to decide how to structure business logic and data access to:
- Optimize read and write operations independently
- Maintain clear separation of concerns
- Enable testability of business logic
- Support eventual consistency if needed in the future
- Decouple request handling from business logic

---

## Decision
We will implement the **CQRS (Command Query Responsibility Segregation)** pattern using **MediatR** library for mediator pattern implementation.

### Pattern Structure

```
Controller → MediatR → Command/Query Handler → Domain/Database

Commands (Write):
- CreateAppointmentCommand
- CancelAppointmentCommand
- RegisterUserCommand

Queries (Read):
- GetDoctorsQuery
- SearchDoctorsQuery
- GetAppointmentsQuery
```

### Key Principles

1. **Separation**: Commands and Queries are separate classes
2. **Handlers**: Each Command/Query has a dedicated handler
3. **Mediator**: MediatR decouples controllers from handlers
4. **Validation**: FluentValidation validators for each command
5. **Single Responsibility**: Each handler does one thing

---

## Consequences

### Positive Consequences

1. **Clear Separation of Concerns**
   - Read models can be optimized differently from write models
   - Query projections can skip unnecessary data
   - Commands focus purely on business logic

2. **Improved Testability**
   - Each handler can be tested in isolation
   - Mock dependencies easily via interfaces
   - Clear input/output contracts

3. **Performance Optimization**
   - Queries can use read-only database connections
   - Different caching strategies for reads vs writes
   - Can use separate read/write databases if needed (future)

4. **Scalability**
   - Read replicas for queries
   - Write master for commands
   - Easy to implement eventual consistency

5. **Maintainability**
   - Easy to find where logic lives (specific handler)
   - Adding new features doesn't affect existing handlers
   - Refactoring is safer due to isolation

6. **Flexibility**
   - Can add cross-cutting concerns via MediatR pipeline behaviors
   - Validation, logging, caching as behaviors
   - Easy to add new commands/queries

7. **Business Logic Encapsulation**
   - Controllers become thin (just route requests)
   - Business logic concentrated in handlers
   - Domain knowledge in one place

### Negative Consequences

1. **Increased Code Volume**
   - More files (command, handler, validator, DTO)
   - Boilerplate for simple CRUD operations
   - More classes to maintain

2. **Learning Curve**
   - Team needs to understand CQRS pattern
   - MediatR library concepts (handlers, pipeline behaviors)
   - Different from traditional service layer

3. **Potential Over-Engineering**
   - May be overkill for very simple operations
   - Risk of creating unnecessary abstractions

4. **Debugging Complexity**
   - Request flow through MediatR can be harder to trace
   - Need good logging to track request pipeline

---

## Alternatives Considered

### Alternative 1: Traditional Service Layer Pattern

**Structure**:
```
Controller → Service → Repository → Database

Example:
AppointmentController → AppointmentService → AppointmentRepository
```

**Pros**:
- Simpler and more familiar to most developers
- Less code and fewer files
- Easier to debug (direct method calls)
- Faster initial development

**Cons**:
- Services can become bloated (God objects)
- No clear separation between reads and writes
- Harder to test (services often have many dependencies)
- Difficult to add cross-cutting concerns
- Read and write logic mixed in same class

**Why Rejected**: Services tend to grow large over time, mixing read and write concerns. CQRS provides better long-term maintainability and clearer structure for complex business logic.

---

### Alternative 2: Full CQRS with Event Sourcing

**Structure**:
```
Write Side: Commands → Event Store → Events
Read Side: Events → Projections → Read Database

Separate databases for reads and writes
```

**Pros**:
- Complete audit trail (event history)
- Time-travel debugging
- Perfect eventual consistency support
- Optimal read/write performance

**Cons**:
- Significantly more complex
- Requires event store infrastructure
- Eventual consistency complexity
- Steeper learning curve
- Higher operational costs
- Overkill for current requirements

**Why Rejected**: While event sourcing has benefits for audit-heavy systems, it adds significant complexity. Our appointment system doesn't require full event history, and eventual consistency would complicate the user experience. We can evolve to this if needed later.

---

### Alternative 3: Simple Repository Pattern Only

**Structure**:
```
Controller → Repository → Database

Example:
AppointmentController → IAppointmentRepository
```

**Pros**:
- Very simple
- Minimal abstraction
- Fast development
- Easy to understand

**Cons**:
- Business logic ends up in controllers
- No clear place for complex operations
- Difficult to add validation
- Hard to add cross-cutting concerns
- Poor testability
- No separation of reads/writes

**Why Rejected**: Too simplistic for a healthcare appointment system with complex business rules. Controllers would become fat, making maintenance difficult.

---

### Alternative 4: Vertical Slice Architecture

**Structure**:
```
Organized by feature, each feature has its own:
/Features/CreateAppointment
  - Controller
  - Command
  - Handler
  - Validator
```

**Pros**:
- Clear feature organization
- Easy to find related code
- Supports incremental development
- Similar to CQRS but feature-focused

**Cons**:
- Can lead to code duplication
- Shared logic placement unclear
- May not enforce separation as strongly

**Why Rejected**: While vertical slices are good, CQRS with MediatR provides similar benefits while also enforcing command/query separation and providing a standard mediator pattern implementation.

---

## Implementation Details

### Example: Create Appointment Flow

#### 1. Command Definition

```csharp
public class CreateAppointmentCommand : IRequest<Result<Guid>>
{
    public Guid PatientId { get; set; }
    public Guid DoctorId { get; set; }
    public DateTime ScheduledDate { get; set; }
    public TimeSlot TimeSlot { get; set; }
    public string Reason { get; set; }
}
```

#### 2. Command Validator

```csharp
public class CreateAppointmentCommandValidator : AbstractValidator<CreateAppointmentCommand>
{
    public CreateAppointmentCommandValidator()
    {
        RuleFor(x => x.PatientId).NotEmpty();
        RuleFor(x => x.DoctorId).NotEmpty();
        RuleFor(x => x.ScheduledDate)
            .GreaterThan(DateTime.Now)
            .WithMessage("Cannot schedule appointments in the past");
        RuleFor(x => x.Reason)
            .NotEmpty()
            .MaximumLength(500);
    }
}
```

#### 3. Command Handler

```csharp
public class CreateAppointmentCommandHandler
    : IRequestHandler<CreateAppointmentCommand, Result<Guid>>
{
    private readonly IApplicationDbContext _context;
    private readonly IEmailService _emailService;

    public async Task<Result<Guid>> Handle(
        CreateAppointmentCommand request,
        CancellationToken cancellationToken)
    {
        // 1. Check doctor availability
        var isAvailable = await CheckAvailability(request);
        if (!isAvailable)
            return Result<Guid>.Failure("Time slot not available");

        // 2. Create appointment entity
        var appointment = Appointment.Create(
            request.PatientId,
            request.DoctorId,
            request.ScheduledDate,
            request.TimeSlot,
            request.Reason
        );

        // 3. Save to database
        _context.Appointments.Add(appointment);
        await _context.SaveChangesAsync(cancellationToken);

        // 4. Raise domain event
        appointment.AddDomainEvent(new AppointmentBookedEvent(appointment));

        // 5. Send confirmation email (via background job)
        await _emailService.SendConfirmationAsync(appointment.Id);

        return Result<Guid>.Success(appointment.Id);
    }
}
```

#### 4. Controller

```csharp
[ApiController]
[Route("api/[controller]")]
public class AppointmentsController : ControllerBase
{
    private readonly IMediator _mediator;

    public AppointmentsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateAppointmentCommand command)
    {
        var result = await _mediator.Send(command);

        if (result.IsSuccess)
            return CreatedAtAction(nameof(GetById), new { id = result.Value }, result.Value);

        return BadRequest(result.Error);
    }
}
```

---

### Example: Query Definition

#### 1. Query Definition

```csharp
public class GetDoctorsQuery : IRequest<Result<PaginatedList<DoctorDto>>>
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public Guid? SpecialtyId { get; set; }
    public string Location { get; set; }
}
```

#### 2. Query Handler

```csharp
public class GetDoctorsQueryHandler
    : IRequestHandler<GetDoctorsQuery, Result<PaginatedList<DoctorDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public async Task<Result<PaginatedList<DoctorDto>>> Handle(
        GetDoctorsQuery request,
        CancellationToken cancellationToken)
    {
        var query = _context.Doctors
            .Include(d => d.Specialty)
            .AsNoTracking(); // Read-only optimization

        // Apply filters
        if (request.SpecialtyId.HasValue)
            query = query.Where(d => d.SpecialtyId == request.SpecialtyId);

        if (!string.IsNullOrEmpty(request.Location))
            query = query.Where(d => d.Location.Contains(request.Location));

        // Paginate
        var doctors = await query
            .OrderBy(d => d.LastName)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        var doctorDtos = _mapper.Map<List<DoctorDto>>(doctors);

        var totalCount = await query.CountAsync(cancellationToken);

        return Result<PaginatedList<DoctorDto>>.Success(
            new PaginatedList<DoctorDto>(doctorDtos, totalCount, request.PageNumber, request.PageSize)
        );
    }
}
```

---

### MediatR Pipeline Behaviors

#### Validation Behavior

```csharp
public class ValidationBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (_validators.Any())
        {
            var context = new ValidationContext<TRequest>(request);
            var validationResults = await Task.WhenAll(
                _validators.Select(v => v.ValidateAsync(context, cancellationToken))
            );

            var failures = validationResults
                .SelectMany(r => r.Errors)
                .Where(f => f != null)
                .ToList();

            if (failures.Any())
                throw new ValidationException(failures);
        }

        return await next();
    }
}
```

#### Logging Behavior

```csharp
public class LoggingBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling {RequestName}", typeof(TRequest).Name);

        var response = await next();

        _logger.LogInformation("Handled {RequestName}", typeof(TRequest).Name);

        return response;
    }
}
```

---

## Configuration

### Program.cs

```csharp
// Register MediatR
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(CreateAppointmentCommand).Assembly));

// Register validators
builder.Services.AddValidatorsFromAssembly(typeof(CreateAppointmentCommandValidator).Assembly);

// Register pipeline behaviors
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
```

---

## Naming Conventions

### Commands
- Verb-based: `CreateAppointmentCommand`, `CancelAppointmentCommand`
- Suffix: `Command`
- Handler: `[CommandName]Handler`

### Queries
- Get-based: `GetDoctorsQuery`, `SearchDoctorsQuery`
- Suffix: `Query`
- Handler: `[QueryName]Handler`

### DTOs
- Suffix: `Dto`
- Example: `DoctorDto`, `AppointmentDto`

---

## Testing Strategy

### Command Handler Tests

```csharp
public class CreateAppointmentCommandHandlerTests
{
    [Fact]
    public async Task Handle_ValidCommand_CreatesAppointment()
    {
        // Arrange
        var context = GetMockDbContext();
        var emailService = new Mock<IEmailService>();
        var handler = new CreateAppointmentCommandHandler(context, emailService.Object);

        var command = new CreateAppointmentCommand
        {
            PatientId = Guid.NewGuid(),
            DoctorId = Guid.NewGuid(),
            ScheduledDate = DateTime.Now.AddDays(1),
            TimeSlot = new TimeSlot(new TimeSpan(10, 0, 0), new TimeSpan(11, 0, 0)),
            Reason = "Check-up"
        };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotEqual(Guid.Empty, result.Value);
    }
}
```

---

## Performance Considerations

### Query Optimization
- Use `AsNoTracking()` for read-only queries
- Project to DTOs to fetch only needed columns
- Use pagination for large result sets
- Consider caching frequently accessed queries

### Command Optimization
- Use transactions appropriately
- Batch related operations
- Use background jobs for non-critical operations
- Optimize database indexes

---

## Migration Path

### Phase 1: Core Commands/Queries
- Authentication commands
- Appointment CRUD commands
- Doctor queries
- Basic appointment queries

### Phase 2: Advanced Features
- Complex search queries
- Analytics queries
- Batch operations
- Scheduled commands

### Phase 3: Optimization
- Add caching behaviors
- Implement query result caching
- Add performance monitoring
- Optimize slow queries

---

## Metrics for Success

1. **Code Organization**: Related logic grouped in handlers
2. **Test Coverage**: 80%+ coverage of handlers
3. **Performance**: Query response times <100ms (p95)
4. **Maintainability**: New features added without modifying existing handlers
5. **Developer Satisfaction**: Team finds pattern helpful

---

## References

- [CQRS Pattern by Martin Fowler](https://martinfowler.com/bliki/CQRS.html)
- [MediatR GitHub](https://github.com/jbogard/MediatR)
- [Jimmy Bogard - CQRS and MediatR in Practice](https://www.youtube.com/watch?v=yozD5Tnd8nw)
- [Clean Architecture with MediatR](https://github.com/jasontaylordev/CleanArchitecture)

---

## Reviewers and Stakeholders

- **Proposed by**: System Architect
- **Reviewed by**: Technical Lead, Senior Developers
- **Approved by**: CTO
- **Date**: 2026-01-06

---

## Revision History

| Version | Date | Description | Author |
|---------|------|-------------|--------|
| 1.0 | 2026-01-06 | Initial ADR | System Architect |

---

## Notes

- Keep commands and queries focused (Single Responsibility)
- Use pipeline behaviors for cross-cutting concerns
- Document complex business rules in handler comments
- Consider eventual consistency for future scaling
- Monitor handler performance metrics
