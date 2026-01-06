# C4 Model Documentation
## Hospital Appointment Management System

## What is the C4 Model?

The C4 model is a lean graphical notation technique for modeling the architecture of software systems. It provides a way to describe and communicate software architecture at different levels of abstraction.

**C4 stands for**: Context, Containers, Components, and Code

### Why C4 Model?

- **Simple and accessible**: Easy for technical and non-technical stakeholders to understand
- **Multiple levels of detail**: Zoom in and out of the architecture
- **Supports agile**: Quick to create and update
- **Language agnostic**: Works with any technology stack
- **Standardized**: Consistent notation and terminology

---

## C4 Model Hierarchy

```
Level 1: System Context
    ↓ (zoom in)
Level 2: Containers
    ↓ (zoom in)
Level 3: Components
    ↓ (zoom in)
Level 4: Code (Classes)
```

---

## Our C4 Documentation

### [Level 1: System Context](c4-level1-system-context.md)
**Audience**: Everyone (technical and non-technical)

**Shows**: The big picture - how the system fits into the world

**Contains**:
- The system as a single box
- External actors (Patients, Doctors, Administrators)
- External systems (Email Service, SMS Gateway, Payment Gateway)
- Relationships and data flows
- System boundaries

**Key Questions Answered**:
- Who uses the system?
- What does the system do?
- What external systems does it integrate with?
- What are the system boundaries?

**Read this if**: You need to understand the overall system purpose and context

---

### [Level 2: Container](c4-level2-container.md)
**Audience**: Technical people (developers, architects, operations)

**Shows**: High-level technology decisions and major building blocks

**Contains**:
- **Angular Web Application**: Single Page Application (SPA) running in user's browser
- **ASP.NET Core Web API**: RESTful backend server
- **SQL Server Database**: Primary data store
- **Hangfire Job Processor**: Background job scheduler
- Communication protocols between containers
- Technology stack for each container

**Key Questions Answered**:
- What are the major technical building blocks?
- What technologies are used?
- How do containers communicate?
- Where does the application run?

**Read this if**: You need to understand the high-level technical architecture

---

### [Level 3: Component](c4-level3-component.md)
**Audience**: Developers and architects

**Shows**: Internal structure of each container

**Backend (Web API) Components**:

#### Presentation Layer
- **Controllers**: AuthenticationController, AppointmentsController, DoctorsController, AdminController
- **Middleware**: Exception handling, JWT authentication, CORS, logging
- **Filters**: Validation, authorization

#### Application Layer (CQRS)
- **Commands**: CreateAppointmentCommand, CancelAppointmentCommand, RegisterCommand
- **Queries**: GetDoctorsQuery, SearchDoctorsQuery, GetAppointmentsQuery
- **Handlers**: One handler per command/query
- **Validators**: FluentValidation validators
- **Behaviors**: MediatR pipeline behaviors

#### Domain Layer
- **Entities**: User, Doctor, Patient, Appointment, Availability
- **Value Objects**: Email, PhoneNumber, TimeSlot
- **Domain Events**: AppointmentBookedEvent, AppointmentCancelledEvent

#### Infrastructure Layer
- **Persistence**: ApplicationDbContext, Entity Configurations
- **Identity**: IdentityService, JwtTokenGenerator
- **External Services**: EmailService, SMSService, PaymentService
- **Background Jobs**: AppointmentReminderJob

**Frontend (Angular) Components**:

#### Core Module
- **Guards**: AuthGuard, RoleGuard
- **Interceptors**: AuthInterceptor, ErrorInterceptor, LoadingInterceptor
- **Services**: AuthService, TokenService, NotificationService

#### Feature Modules
- **Authentication Module**: Login, Register, Forgot Password components
- **Patient Module**: Doctor Search, Appointment Booking, Appointment List
- **Doctor Module**: Dashboard, Schedule, Availability Management
- **Admin Module**: User Management, Analytics, System Configuration

#### Shared Module
- Reusable components, pipes, directives

**Key Questions Answered**:
- What are the main components in each container?
- How do components interact?
- What design patterns are used?
- Where is specific functionality implemented?

**Read this if**: You're implementing features or need to understand the internal structure

---

### [Level 4: Code](c4-level4-code.md)
**Audience**: Developers

**Shows**: Class-level details and relationships

**Contains**:

#### Domain Entities (Class Diagrams)
```
BaseEntity (abstract)
├── User
├── Doctor
├── Patient
├── Appointment
├── Availability
├── Specialty
└── Notification
```

#### Value Objects
- **Email**: Encapsulates email validation
- **PhoneNumber**: Encapsulates phone number validation
- **TimeSlot**: Represents appointment time slot

#### Enumerations
- **UserRole**: Patient, Doctor, Administrator
- **AppointmentStatus**: Scheduled, Confirmed, Completed, Cancelled, NoShow
- **NotificationType**: Email, SMS, Push
- **NotificationStatus**: Pending, Sent, Failed, Retrying

#### CQRS Implementation
- **Command Classes**: CreateAppointmentCommand, CancelAppointmentCommand
- **Command Handlers**: Business logic implementation
- **Query Classes**: GetDoctorsQuery, SearchDoctorsQuery
- **Query Handlers**: Data retrieval logic
- **Validators**: FluentValidation validators for commands

#### EF Core Configuration
- **DbContext**: ApplicationDbContext
- **Entity Configurations**: Fluent API configurations
- **Migrations**: Database schema versioning

#### Angular Models
- **Interfaces**: User, Doctor, Patient, Appointment
- **Enums**: UserRole, AppointmentStatus
- **Services**: AuthService, DoctorService, AppointmentService

**Key Questions Answered**:
- What are the key classes and their relationships?
- What properties and methods do classes have?
- How are domain entities structured?
- How is CQRS implemented?

**Read this if**: You're writing code or need to understand specific implementations

---

## How to Navigate the C4 Documentation

### Top-Down Approach (Recommended for new team members)
1. Start with **Level 1: System Context** to understand the big picture
2. Move to **Level 2: Container** to see major technical components
3. Dive into **Level 3: Component** for internal structure
4. Reference **Level 4: Code** when implementing specific features

### Bottom-Up Approach (For detailed code understanding)
1. Start with **Level 4: Code** to see class structures
2. Move to **Level 3: Component** to understand how classes work together
3. Review **Level 2: Container** to see the bigger technical picture
4. Check **Level 1: System Context** for the overall system purpose

### Feature-Based Approach (For implementing new features)
1. Check **Level 1** to understand which external systems are involved
2. Review **Level 2** to identify which containers need changes
3. Study **Level 3** to find which components to modify
4. Reference **Level 4** for specific class implementations

---

## C4 Model Notation

### Level 1 & 2 Notation
- **Rectangle**: System, Container, or Person
- **Arrow**: Relationship/interaction
- **Label on Arrow**: Description of relationship
- **Technology Label**: Shows technology choice (e.g., "Angular 17+", ".NET 8")

### Level 3 Notation
- **Rectangle**: Component
- **Arrow**: Dependency or data flow
- **Grouping**: Components grouped by layer or module

### Level 4 Notation
- **UML Class Diagram**: Standard UML notation
- **+ Public**: Public members
- **- Private**: Private members
- **# Protected**: Protected members

---

## Diagram Update Guidelines

### When to Update

1. **Major Architecture Changes**
   - Adding or removing containers
   - Changing technology stack
   - New external system integrations

2. **Significant Feature Additions**
   - New modules or components
   - New domain entities
   - New external services

3. **Technology Upgrades**
   - Framework version changes
   - Database migrations
   - Infrastructure changes

### Update Process

1. **Identify affected levels**: Determine which C4 levels need updates
2. **Update diagrams**: Modify the appropriate markdown files
3. **Review changes**: Get team review
4. **Update related documentation**: Keep ADRs and architecture docs in sync
5. **Communicate**: Inform team of architecture changes

---

## Relationship with Other Documentation

### C4 Model complements:

**Architecture Decision Records (ADRs)**
- C4 shows WHAT the architecture is
- ADRs explain WHY decisions were made
- Reference: [../adr/](../adr/)

**Architecture Documentation**
- C4 provides visual overview
- Architecture docs provide detailed implementation
- Reference: [../architecture/](../architecture/)

**User Stories and Tasks**
- C4 shows technical structure
- User stories show functional requirements
- Reference: [../user-story.md](../user-story.md), [../task.md](../task.md)

---

## Tools for C4 Diagrams

### Recommended Tools

1. **PlantUML with C4-PlantUML**
   - Text-based diagrams
   - Version control friendly
   - Can be automated

2. **Structurizr**
   - Official C4 tool
   - Code-based diagrams
   - Multiple export formats

3. **Draw.io (diagrams.net)**
   - Visual editor
   - Free and open-source
   - Has C4 templates

4. **Mermaid**
   - Markdown-based
   - GitHub integration
   - Simple syntax

### Current Format
Our C4 documentation is currently in **text-based ASCII diagrams** in markdown format for:
- Easy version control
- No tool dependencies
- Universal readability

Consider migrating to PlantUML or Structurizr for:
- More complex diagrams
- Automated generation from code
- Better visual presentation

---

## Best Practices

### Do's
✅ Keep diagrams simple and focused
✅ Use consistent terminology
✅ Add enough detail but not too much
✅ Update diagrams with code changes
✅ Use appropriate level for audience
✅ Include technology choices
✅ Show clear relationships

### Don'ts
❌ Don't show implementation details in Level 1-2
❌ Don't make diagrams too cluttered
❌ Don't use inconsistent notation
❌ Don't let diagrams become outdated
❌ Don't skip levels (maintain all 4)
❌ Don't mix levels in one diagram

---

## Common Questions

### Q: Which level should I start with?
**A**: Level 1 (System Context) for understanding the big picture, or Level 3 (Component) if you're implementing features.

### Q: How detailed should Level 4 be?
**A**: Show key classes and relationships, not every property and method. Focus on architectural significance.

### Q: How often should we update C4 diagrams?
**A**: Update when making significant architectural changes, ideally as part of the development process.

### Q: Can we automate C4 diagram generation?
**A**: Yes, tools like Structurizr allow code-based diagram generation. Consider for future enhancement.

### Q: Do we need all 4 levels?
**A**: Yes, each level serves different audiences and purposes. Maintain all levels for complete documentation.

---

## C4 Model Resources

### Official Resources
- [C4 Model Website](https://c4model.com/)
- [C4 Model on GitHub](https://github.com/structurizr/c4model)
- [Structurizr](https://structurizr.com/)

### Tutorials and Guides
- [C4 Model Introduction](https://www.infoq.com/articles/C4-architecture-model/)
- [PlantUML C4 Extension](https://github.com/plantuml-stdlib/C4-PlantUML)
- [Simon Brown's Talks](https://www.youtube.com/results?search_query=simon+brown+c4+model)

### Related Concepts
- **Arc42**: Documentation template (C4 can be used within Arc42)
- **UML**: C4 is simpler and more focused than UML
- **4+1 Views**: Similar multi-view approach to architecture

---

## Feedback and Improvements

### How to Provide Feedback
1. **Documentation Issues**: Report inaccuracies or outdated information
2. **Clarity Issues**: Suggest improvements for better understanding
3. **Missing Information**: Request additional details or examples
4. **Diagram Quality**: Suggest better visual representations

### Continuous Improvement
- Regular reviews during architecture meetings
- Update after major releases
- Incorporate team feedback
- Keep aligned with actual implementation

---

## Summary

### Quick Reference

| Level | Focus | Audience | Detail |
|-------|-------|----------|--------|
| 1. System Context | Big picture | Everyone | Low |
| 2. Container | Technology choices | Technical | Medium |
| 3. Component | Internal structure | Developers | High |
| 4. Code | Class design | Developers | Very High |

### Navigation

- **System Context** → [c4-level1-system-context.md](c4-level1-system-context.md)
- **Container** → [c4-level2-container.md](c4-level2-container.md)
- **Component** → [c4-level3-component.md](c4-level3-component.md)
- **Code** → [c4-level4-code.md](c4-level4-code.md)

### Related Documentation

- **ADRs** → [../adr/](../adr/)
- **Architecture** → [../architecture/](../architecture/)
- **Product Docs** → [../](../)

---

## Document Metadata

- **Document Version**: 1.0
- **Last Updated**: 2026-01-06
- **Author**: System Architect
- **Status**: Current

---

*The C4 model provides a clear and consistent way to communicate software architecture at multiple levels of detail.*
