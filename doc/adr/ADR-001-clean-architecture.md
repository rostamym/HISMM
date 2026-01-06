# ADR-001: Adopt Clean Architecture Pattern

## Status
**Accepted** - 2026-01-06

## Context
The Hospital Appointment Management System requires a maintainable, testable, and scalable architecture that can evolve with changing business requirements while remaining independent of external frameworks and infrastructure.

### Problem Statement
We need to decide on an architectural pattern for building the backend system that will:
- Support long-term maintainability and evolution
- Enable independent testing of business logic
- Allow flexibility in changing infrastructure components
- Enforce separation of concerns
- Support multiple user roles (Patient, Doctor, Administrator)
- Handle complex business rules around appointment scheduling

### Requirements
1. **Maintainability**: Code should be easy to understand and modify
2. **Testability**: Business logic must be testable without external dependencies
3. **Flexibility**: Ability to change databases, UI frameworks, or external services without affecting core logic
4. **Scalability**: Architecture should support growth in complexity and team size
5. **Domain-Centric**: Business rules should be the center of the application

---

## Decision
We will adopt **Clean Architecture** (also known as Onion Architecture or Hexagonal Architecture) for the Hospital Appointment Management System backend.

### Architecture Layers

```
┌─────────────────────────────────────────────────────────┐
│                   Presentation Layer                    │
│              (API Controllers, Middleware)              │
├─────────────────────────────────────────────────────────┤
│                   Application Layer                     │
│           (Use Cases, DTOs, Interfaces, CQRS)          │
├─────────────────────────────────────────────────────────┤
│                     Domain Layer                        │
│      (Entities, Value Objects, Domain Services,         │
│              Domain Events, Business Rules)             │
├─────────────────────────────────────────────────────────┤
│                 Infrastructure Layer                    │
│     (Database, Email, External Services, Identity)      │
└─────────────────────────────────────────────────────────┘
```

### Dependency Rule
Dependencies point **inward**:
- Presentation → Application → Domain
- Infrastructure → Application

The Domain layer has **NO** dependencies on any other layer.

---

## Consequences

### Positive Consequences

1. **Independence of Business Logic**
   - Domain entities contain pure business logic
   - No coupling to databases, UI, or external services
   - Business rules can be tested in isolation

2. **Testability**
   - Unit testing domain logic without any infrastructure
   - Mocking infrastructure through interfaces
   - High test coverage achievable with minimal setup

3. **Flexibility and Maintainability**
   - Can swap out SQL Server for PostgreSQL without touching business logic
   - Can change from Angular to React without affecting backend
   - Can replace SendGrid with AWS SES by implementing IEmailService

4. **Clear Separation of Concerns**
   - Each layer has a specific responsibility
   - Easier to reason about code location
   - Reduces cognitive load for developers

5. **Scalability**
   - Multiple teams can work on different layers independently
   - Can evolve to microservices if needed
   - Easy to add new features without disrupting existing code

6. **Framework Independence**
   - Not tied to any specific framework
   - Core business logic remains even if frameworks change
   - Reduces risk of framework obsolescence

### Negative Consequences

1. **Initial Complexity**
   - More files and folders than a simple layered architecture
   - Steeper learning curve for developers unfamiliar with Clean Architecture
   - Requires discipline to maintain layer boundaries

2. **More Boilerplate Code**
   - DTOs for data transfer between layers
   - Interfaces for infrastructure services
   - More mapping between entities and DTOs

3. **Slower Initial Development**
   - Setting up the architecture takes more time upfront
   - More ceremony for simple CRUD operations
   - Team needs training on Clean Architecture principles

4. **Potential Over-Engineering**
   - May be overkill for very simple applications
   - Risk of creating unnecessary abstractions
   - Can lead to analysis paralysis

---

## Alternatives Considered

### Alternative 1: Traditional Layered (N-Tier) Architecture

**Structure**:
```
Presentation Layer → Business Logic Layer → Data Access Layer
```

**Pros**:
- Simple and well-understood
- Less code and fewer files
- Faster initial development
- Lower learning curve

**Cons**:
- Business logic often becomes coupled to data access
- Difficult to test without database
- Hard to change infrastructure components
- Database-centric rather than domain-centric
- Tight coupling between layers

**Why Rejected**: Healthcare systems require long-term maintainability and testability. The tight coupling in traditional layered architecture makes it difficult to evolve the system over time.

---

### Alternative 2: Microservices Architecture

**Structure**:
```
Multiple independent services:
- User Service
- Doctor Service
- Appointment Service
- Notification Service
```

**Pros**:
- Independent scalability
- Technology diversity
- Independent deployments
- Fault isolation

**Cons**:
- Significant operational complexity
- Distributed system challenges (network latency, data consistency)
- More complex deployment and monitoring
- Overkill for initial system scope
- Higher infrastructure costs

**Why Rejected**: The system doesn't have the scale requirements to justify microservices complexity. We can evolve to microservices later if needed, starting with a Clean Architecture monolith provides a good foundation.

---

### Alternative 3: Feature-Sliced Architecture

**Structure**:
```
Organized by features rather than layers:
- /features/appointments
- /features/doctors
- /features/patients
```

**Pros**:
- Organized by business capabilities
- Easy to find related code
- Supports incremental development
- Good for feature teams

**Cons**:
- Can lead to code duplication
- Shared logic can be unclear where to place
- Less emphasis on domain modeling
- May not scale well for complex domains

**Why Rejected**: While feature-sliced architecture has merits, our system has complex domain logic (appointment scheduling, availability management) that benefits from a strong domain layer. Clean Architecture provides better support for complex business rules.

---

### Alternative 4: Simple MVC Pattern

**Structure**:
```
Models → Controllers → Views
```

**Pros**:
- Extremely simple
- Minimal ceremony
- Fast development
- Built into ASP.NET Core

**Cons**:
- Business logic often ends up in controllers (fat controllers)
- Poor testability
- No clear separation of concerns
- Difficult to maintain as complexity grows

**Why Rejected**: MVC is too simplistic for a healthcare appointment system with complex business rules. The lack of clear separation between business logic and infrastructure makes it unsuitable for long-term maintenance.

---

## Implementation Details

### Project Structure

```
HospitalAppointmentSystem/
├── Domain/
│   └── HospitalAppointmentSystem.Domain.csproj
│       ├── Entities/
│       ├── ValueObjects/
│       ├── Enums/
│       ├── Events/
│       └── Exceptions/
│
├── Application/
│   └── HospitalAppointmentSystem.Application.csproj
│       ├── Common/
│       │   └── Interfaces/
│       ├── Features/ (CQRS Commands & Queries)
│       ├── DTOs/
│       ├── Mappings/
│       └── Validators/
│
├── Infrastructure/
│   └── HospitalAppointmentSystem.Infrastructure.csproj
│       ├── Persistence/
│       ├── Identity/
│       ├── Services/
│       └── BackgroundJobs/
│
└── Presentation/
    └── HospitalAppointmentSystem.API.csproj
        ├── Controllers/
        ├── Middleware/
        └── Filters/
```

### Key Principles

1. **Dependency Inversion**
   - Application layer defines interfaces
   - Infrastructure layer implements interfaces
   - Presentation layer depends on Application

2. **Single Responsibility**
   - Each class has one reason to change
   - Domain entities focus on business rules
   - Controllers focus on HTTP concerns

3. **Open/Closed Principle**
   - Open for extension (new features)
   - Closed for modification (existing code)

4. **Interface Segregation**
   - Small, focused interfaces
   - Clients don't depend on methods they don't use

---

## Metrics for Success

1. **Test Coverage**: Aim for 80%+ code coverage in Domain and Application layers
2. **Maintainability**: New features should not require changes to existing layer boundaries
3. **Build Time**: Clean separation should keep build times reasonable
4. **Team Velocity**: After initial setup, development speed should improve
5. **Bug Rate**: Lower bug rate due to better testability and separation

---

## Migration Path

### Phase 1: Initial Setup
- Set up project structure with four layers
- Define domain entities and value objects
- Establish interfaces in Application layer

### Phase 2: Core Implementation
- Implement use cases in Application layer
- Implement infrastructure services
- Wire up dependency injection

### Phase 3: Refinement
- Add comprehensive tests
- Refine domain model based on learnings
- Optimize performance bottlenecks

### Future Evolution
If system grows significantly, Clean Architecture provides a good foundation for:
- Extracting bounded contexts into microservices
- Implementing event sourcing
- Adding CQRS read models (separate databases for queries)

---

## References

- **Clean Architecture** by Robert C. Martin
- **Implementing Domain-Driven Design** by Vaughn Vernon
- **.NET Microservices: Architecture for Containerized .NET Applications** (Microsoft)
- [Clean Architecture Template](https://github.com/jasontaylordev/CleanArchitecture) by Jason Taylor

---

## Reviewers and Stakeholders

- **Proposed by**: System Architect
- **Reviewed by**: Development Team, Technical Lead
- **Approved by**: CTO
- **Date**: 2026-01-06

---

## Revision History

| Version | Date | Description | Author |
|---------|------|-------------|--------|
| 1.0 | 2026-01-06 | Initial ADR | System Architect |

---

## Notes

- Team training on Clean Architecture principles is required
- Code reviews should enforce layer boundaries
- Consider using ArchUnit or similar tools to validate architectural constraints
- Document any exceptions to the architecture rules
