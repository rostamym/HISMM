# C4 Model - Level 3: Component Diagram
## Hospital Appointment Management System

### Overview
The Component diagram shows the internal structure of each container, breaking them down into components. Components are groupings of related functionality encapsulated behind a well-defined interface. This level provides detailed insight into the architecture and design patterns used within each container.

---

## ASP.NET Core Web API Components

### Clean Architecture Layer Structure

```
┌───────────────────────────────────────────────────────────────────────┐
│                        PRESENTATION LAYER                             │
│  ┌────────────┐  ┌────────────┐  ┌────────────┐  ┌─────────────┐   │
│  │ Auth       │  │Appointments│  │  Doctors   │  │    Admin    │   │
│  │Controller  │  │ Controller │  │ Controller │  │  Controller │   │
│  └─────┬──────┘  └─────┬──────┘  └─────┬──────┘  └──────┬──────┘   │
│        │                │                │                │           │
│  ┌─────▼────────────────▼────────────────▼────────────────▼───────┐ │
│  │              API Middleware Pipeline                            │ │
│  │  [Exception Handler] [Auth] [CORS] [Logging] [Validation]      │ │
│  └─────────────────────────────────────────────────────────────────┘ │
└───────────────────────────────┬───────────────────────────────────────┘
                                │
                                │ Sends Commands/Queries via MediatR
                                ▼
┌───────────────────────────────────────────────────────────────────────┐
│                        APPLICATION LAYER (CQRS)                       │
│                                                                       │
│  ┌──────────────────── Commands (Write) ─────────────────────────┐  │
│  │  ┌─────────────────┐    ┌──────────────────┐                  │  │
│  │  │ RegisterCommand │    │ LoginCommand     │                  │  │
│  │  │    Handler      │    │    Handler       │                  │  │
│  │  └────────┬────────┘    └─────────┬────────┘                  │  │
│  │           │                       │                            │  │
│  │  ┌────────▼──────────┐  ┌────────▼─────────────┐             │  │
│  │  │CreateAppointment  │  │ CancelAppointment    │             │  │
│  │  │Command Handler    │  │ Command Handler      │             │  │
│  │  └───────────────────┘  └──────────────────────┘             │  │
│  └───────────────────────────────────────────────────────────────┘  │
│                                                                       │
│  ┌──────────────────── Queries (Read) ──────────────────────────┐   │
│  │  ┌──────────────────┐   ┌───────────────────┐                │   │
│  │  │GetDoctorsQuery   │   │GetAppointments    │                │   │
│  │  │   Handler        │   │Query Handler      │                │   │
│  │  └────────┬─────────┘   └─────────┬─────────┘                │   │
│  │           │                       │                            │   │
│  │  ┌────────▼─────────┐  ┌─────────▼──────────┐                │   │
│  │  │SearchDoctorsQuery│  │GetAvailabilityQuery│                │   │
│  │  │   Handler        │  │    Handler         │                │   │
│  │  └──────────────────┘  └────────────────────┘                │   │
│  └───────────────────────────────────────────────────────────────┘   │
│                                                                       │
│  ┌───────────────── Supporting Components ──────────────────────┐   │
│  │  ┌─────────────┐  ┌──────────────┐  ┌─────────────────────┐ │   │
│  │  │ FluentValid │  │  AutoMapper  │  │   MediatR Pipeline  │ │   │
│  │  │  ators      │  │  Profiles    │  │   Behaviors         │ │   │
│  │  └─────────────┘  └──────────────┘  └─────────────────────┘ │   │
│  └───────────────────────────────────────────────────────────────┘   │
└───────────────────────────────┬───────────────────────────────────────┘
                                │
                                │ Uses Interfaces
                                ▼
┌───────────────────────────────────────────────────────────────────────┐
│                           DOMAIN LAYER                                │
│                                                                       │
│  ┌──────────── Domain Entities ────────────────────────────────┐    │
│  │  ┌──────┐  ┌────────┐  ┌──────────┐  ┌────────────────┐    │    │
│  │  │ User │  │ Doctor │  │ Patient  │  │  Appointment   │    │    │
│  │  └──────┘  └────────┘  └──────────┘  └────────────────┘    │    │
│  │  ┌────────────┐  ┌──────────┐  ┌──────────────────────┐    │    │
│  │  │Availability│  │Specialty │  │   Notification       │    │    │
│  │  └────────────┘  └──────────┘  └──────────────────────┘    │    │
│  └───────────────────────────────────────────────────────────────┘    │
│                                                                       │
│  ┌──────────── Value Objects ──────────────────────────────────┐    │
│  │  ┌────────┐  ┌─────────────┐  ┌──────────┐                 │    │
│  │  │ Email  │  │ PhoneNumber │  │ TimeSlot │                 │    │
│  │  └────────┘  └─────────────┘  └──────────┘                 │    │
│  └───────────────────────────────────────────────────────────────┘    │
│                                                                       │
│  ┌──────────── Domain Events ──────────────────────────────────┐    │
│  │  ┌──────────────────────┐  ┌──────────────────────────────┐ │    │
│  │  │AppointmentBooked     │  │AppointmentCancelled          │ │    │
│  │  │     Event            │  │       Event                  │ │    │
│  │  └──────────────────────┘  └──────────────────────────────┘ │    │
│  └───────────────────────────────────────────────────────────────┘    │
└───────────────────────────────────────────────────────────────────────┘
                                ▲
                                │ Implements
                                │
┌───────────────────────────────────────────────────────────────────────┐
│                       INFRASTRUCTURE LAYER                            │
│                                                                       │
│  ┌──────────── Data Access ────────────────────────────────────┐    │
│  │  ┌──────────────────────┐   ┌─────────────────────────────┐ │    │
│  │  │ApplicationDbContext  │   │  Entity Configurations      │ │    │
│  │  │  (EF Core)           │   │  (Fluent API)               │ │    │
│  │  └──────────┬───────────┘   └─────────────────────────────┘ │    │
│  │             │                                                 │    │
│  │             │ DbSets: Users, Doctors, Appointments, etc.      │    │
│  │             ▼                                                 │    │
│  │  ┌──────────────────────────────────────────────────────┐    │    │
│  │  │          SQL Server Database                         │    │    │
│  │  └──────────────────────────────────────────────────────┘    │    │
│  └───────────────────────────────────────────────────────────────┘    │
│                                                                       │
│  ┌──────────── Identity & Auth ────────────────────────────────┐    │
│  │  ┌─────────────────┐   ┌────────────────────────────────┐   │    │
│  │  │ IdentityService │   │   JwtTokenGenerator            │   │    │
│  │  │                 │   │                                │   │    │
│  │  │- Register()     │   │- GenerateToken()               │   │    │
│  │  │- Login()        │   │- ValidateToken()               │   │    │
│  │  │- ValidateUser() │   │- GenerateRefreshToken()        │   │    │
│  │  └─────────────────┘   └────────────────────────────────┘   │    │
│  └───────────────────────────────────────────────────────────────┘    │
│                                                                       │
│  ┌──────────── External Services ──────────────────────────────┐    │
│  │  ┌──────────────┐  ┌──────────────┐  ┌─────────────────┐   │    │
│  │  │EmailService  │  │SMSService    │  │PaymentService   │   │    │
│  │  │              │  │              │  │                 │   │    │
│  │  │- SendAsync() │  │- SendSMS()   │  │- ProcessPay()   │   │    │
│  │  │- SendRemind()│  │- SendOTP()   │  │- Refund()       │   │    │
│  │  └──────────────┘  └──────────────┘  └─────────────────┘   │    │
│  └───────────────────────────────────────────────────────────────┘    │
│                                                                       │
│  ┌──────────── Background Jobs ─────────────────────────────────┐   │
│  │  ┌────────────────────────────────────────────────────────┐  │   │
│  │  │  AppointmentReminderJob                                │  │   │
│  │  │                                                        │  │   │
│  │  │  - Execute(): Queries upcoming appointments           │  │   │
│  │  │  - SendReminders(): Sends email/SMS                   │  │   │
│  │  │  - Schedule: Runs every hour                          │  │   │
│  │  └────────────────────────────────────────────────────────┘  │   │
│  └───────────────────────────────────────────────────────────────┘   │
└───────────────────────────────────────────────────────────────────────┘
```

---

## Angular Web Application Components

### Module Structure

```
┌───────────────────────────────────────────────────────────────────────┐
│                         APP MODULE (Root)                             │
│  ┌────────────────────┐  ┌──────────────────┐  ┌─────────────────┐  │
│  │ AppComponent       │  │  AppRouting      │  │  Core Module    │  │
│  │ (Shell)            │  │  Module          │  │  (Singleton)    │  │
│  └────────────────────┘  └──────────────────┘  └─────────────────┘  │
└───────────────────────────────────────────────────────────────────────┘
                                │
        ┌───────────────────────┼───────────────────────┐
        │                       │                       │
        ▼                       ▼                       ▼
┌──────────────────┐  ┌──────────────────┐  ┌──────────────────────┐
│  CORE MODULE     │  │  SHARED MODULE   │  │  FEATURE MODULES     │
│  (Singletons)    │  │  (Reusables)     │  │  (Lazy Loaded)       │
└──────────────────┘  └──────────────────┘  └──────────────────────┘
```

### Core Module Components

```
Core Module (Singleton Services)
│
├── Guards
│   ├── AuthGuard
│   │   └── canActivate(): Checks if user is authenticated
│   └── RoleGuard
│       └── canActivate(): Checks user role (Patient/Doctor/Admin)
│
├── Interceptors
│   ├── AuthInterceptor
│   │   └── intercept(): Adds JWT token to requests
│   ├── ErrorInterceptor
│   │   └── intercept(): Handles HTTP errors globally
│   └── LoadingInterceptor
│       └── intercept(): Shows/hides loading spinner
│
├── Services
│   ├── AuthService
│   │   ├── login(credentials): Authenticates user
│   │   ├── register(data): Registers new user
│   │   ├── logout(): Clears session
│   │   └── isAuthenticated(): Checks auth status
│   │
│   ├── TokenService
│   │   ├── getToken(): Retrieves JWT from storage
│   │   ├── setToken(token): Stores JWT
│   │   ├── removeToken(): Clears JWT
│   │   └── decodeToken(): Decodes JWT payload
│   │
│   └── NotificationService
│       ├── showSuccess(message): Shows success toast
│       ├── showError(message): Shows error toast
│       └── showInfo(message): Shows info toast
│
└── Models
    ├── User
    ├── ApiResponse
    └── PaginatedResult
```

### Feature Modules

#### Authentication Module

```
Authentication Module
│
├── Components
│   ├── LoginComponent
│   │   ├── Template: Login form with email/password
│   │   ├── Logic: Validates input, calls AuthService.login()
│   │   └── Navigation: Redirects to dashboard on success
│   │
│   ├── RegisterComponent
│   │   ├── Template: Registration form (multi-step wizard)
│   │   ├── Logic: Validates input, calls AuthService.register()
│   │   └── Navigation: Redirects to email verification
│   │
│   └── ForgotPasswordComponent
│       ├── Template: Email input form
│       ├── Logic: Sends password reset email
│       └── Navigation: Shows success message
│
└── Routes
    ├── /login
    ├── /register
    └── /forgot-password
```

#### Patient Module

```
Patient Module
│
├── Components
│   ├── PatientDashboardComponent
│   │   ├── Shows upcoming appointments
│   │   ├── Quick action buttons
│   │   └── Recent activity feed
│   │
│   ├── DoctorSearchComponent
│   │   ├── Search bar with autocomplete
│   │   ├── Filter panel (specialty, location, availability)
│   │   ├── Doctor cards grid
│   │   └── Pagination controls
│   │
│   ├── DoctorDetailComponent
│   │   ├── Doctor profile information
│   │   ├── Reviews and ratings
│   │   ├── Available time slots calendar
│   │   └── Book appointment button
│   │
│   ├── AppointmentBookingComponent
│   │   ├── Date picker calendar
│   │   ├── Time slot selection
│   │   ├── Reason for visit form
│   │   └── Confirmation dialog
│   │
│   └── AppointmentListComponent
│       ├── Tabs: Upcoming / Past / Cancelled
│       ├── Appointment cards with details
│       ├── Actions: Cancel, Reschedule, View details
│       └── Filter and sort controls
│
├── Services
│   ├── DoctorService
│   │   ├── getDoctors(filters): List doctors
│   │   ├── searchDoctors(query): Search doctors
│   │   ├── getDoctorById(id): Get doctor details
│   │   └── getAvailability(doctorId, date): Get time slots
│   │
│   └── AppointmentService
│       ├── createAppointment(data): Book appointment
│       ├── getAppointments(patientId): List appointments
│       ├── getAppointmentById(id): Get details
│       ├── rescheduleAppointment(id, data): Reschedule
│       └── cancelAppointment(id): Cancel appointment
│
└── Routes
    ├── /patient/dashboard
    ├── /patient/doctors
    ├── /patient/doctors/:id
    ├── /patient/appointments
    └── /patient/appointments/:id
```

#### Doctor Module

```
Doctor Module
│
├── Components
│   ├── DoctorDashboardComponent
│   │   ├── Today's appointments timeline
│   │   ├── Statistics (total patients, completion rate)
│   │   └── Quick actions panel
│   │
│   ├── ScheduleComponent
│   │   ├── Weekly calendar view
│   │   ├── Daily timeline view
│   │   ├── Appointment details on click
│   │   └── Drag-and-drop rescheduling
│   │
│   ├── AvailabilityManagementComponent
│   │   ├── Weekly schedule editor
│   │   ├── Time slot configuration
│   │   ├── Date blocker (holidays/time off)
│   │   └── Slot duration settings
│   │
│   ├── AppointmentDetailsComponent
│   │   ├── Patient information
│   │   ├── Appointment history
│   │   ├── Notes editor
│   │   └── Mark as completed button
│   │
│   └── DoctorProfileComponent
│       ├── Profile editor
│       ├── Credentials management
│       ├── Biography editor
│       └── Photo upload
│
├── Services
│   └── AvailabilityService
│       ├── getAvailability(doctorId): Get schedule
│       ├── setAvailability(data): Set schedule
│       ├── blockDate(date, reason): Block date
│       └── unblockDate(date): Unblock date
│
└── Routes
    ├── /doctor/dashboard
    ├── /doctor/schedule
    ├── /doctor/availability
    ├── /doctor/appointments/:id
    └── /doctor/profile
```

#### Admin Module

```
Admin Module
│
├── Components
│   ├── AdminDashboardComponent
│   │   ├── Key metrics cards (users, appointments, revenue)
│   │   ├── Charts (appointments trend, popular specialties)
│   │   └── Recent activity log
│   │
│   ├── UserManagementComponent
│   │   ├── User list with search/filter
│   │   ├── User details modal
│   │   ├── Actions: Activate, Deactivate, Delete
│   │   └── Role assignment
│   │
│   ├── DoctorManagementComponent
│   │   ├── Doctor list with verification status
│   │   ├── Approve/Reject doctor applications
│   │   ├── Edit doctor profiles
│   │   └── Assign specialties
│   │
│   ├── AppointmentManagementComponent
│   │   ├── All appointments view
│   │   ├── Filter by status, doctor, patient, date
│   │   ├── Override/cancel appointments
│   │   └── Resolve disputes
│   │
│   └── AnalyticsComponent
│       ├── Date range selector
│       ├── Revenue charts
│       ├── Appointment statistics
│       ├── Popular specialties graph
│       └── Export to CSV/PDF
│
├── Services
│   └── AdminService
│       ├── getUsers(filters): List users
│       ├── activateUser(id): Activate user
│       ├── deactivateUser(id): Deactivate user
│       ├── getAnalytics(dateRange): Get statistics
│       └── generateReport(params): Generate report
│
└── Routes
    ├── /admin/dashboard
    ├── /admin/users
    ├── /admin/doctors
    ├── /admin/appointments
    └── /admin/analytics
```

### Shared Module Components

```
Shared Module (Reusable Components)
│
├── Components
│   ├── HeaderComponent
│   │   ├── Logo and navigation menu
│   │   ├── User profile dropdown
│   │   └── Notifications bell
│   │
│   ├── SidebarComponent
│   │   ├── Navigation menu
│   │   └── Role-specific menu items
│   │
│   ├── FooterComponent
│   │   ├── Copyright information
│   │   └── Links (Privacy, Terms, Contact)
│   │
│   ├── LoadingSpinnerComponent
│   │   └── Animated loading indicator
│   │
│   ├── ErrorMessageComponent
│   │   └── Displays error messages
│   │
│   ├── ConfirmationDialogComponent
│   │   └── Reusable confirmation modal
│   │
│   ├── DatePickerComponent
│   │   └── Custom date picker with validation
│   │
│   └── PaginationComponent
│       └── Reusable pagination controls
│
├── Pipes
│   ├── DateFormatPipe: Formats dates
│   ├── TimePipe: Formats time
│   └── TruncatePipe: Truncates long text
│
└── Directives
    ├── HasRoleDirective: Show/hide based on role
    └── ClickOutsideDirective: Detect outside clicks
```

---

## Component Interaction Flows

### Example: Book Appointment Flow

```
1. User (Patient) → DoctorSearchComponent
   ↓ Searches for "Cardiologist"

2. DoctorSearchComponent → DoctorService.searchDoctors()
   ↓ HTTP GET /api/doctors/search?specialty=cardiology

3. DoctorService → AuthInterceptor → Adds JWT token
   ↓ Authorization: Bearer {token}

4. API → DoctorsController.Search()
   ↓ Receives request

5. DoctorsController → MediatR.Send(SearchDoctorsQuery)
   ↓ Dispatches query

6. SearchDoctorsQueryHandler → ApplicationDbContext
   ↓ Queries database

7. ApplicationDbContext → SQL Server
   ↓ SELECT * FROM Doctors WHERE SpecialtyId = ...

8. SQL Server → Returns doctor records
   ↓

9. SearchDoctorsQueryHandler → AutoMapper.Map()
   ↓ Maps entities to DTOs

10. API → Returns JSON response
    ↓ 200 OK [{doctor1}, {doctor2}, ...]

11. DoctorService → Returns Observable
    ↓

12. DoctorSearchComponent → Displays results
    ↓

13. User → Clicks "View Profile" on doctor
    ↓

14. DoctorDetailComponent → DoctorService.getDoctorById(id)
    ↓

15. [Similar flow to get doctor details]
    ↓

16. DoctorDetailComponent → Shows availability calendar
    ↓

17. User → Selects date and time slot
    ↓

18. User → Clicks "Book Appointment"
    ↓

19. AppointmentBookingComponent → AppointmentService.createAppointment()
    ↓ HTTP POST /api/appointments

20. API → AppointmentsController.Create()
    ↓

21. AppointmentsController → MediatR.Send(CreateAppointmentCommand)
    ↓

22. CreateAppointmentCommandHandler → Validates availability
    ↓ Checks for conflicts in database

23. CreateAppointmentCommandHandler → Creates appointment entity
    ↓ Saves to database

24. CreateAppointmentCommandHandler → Raises AppointmentBookedEvent
    ↓

25. Event Handler → Hangfire.Enqueue(SendConfirmationEmail)
    ↓ Queues background job

26. API → Returns success response
    ↓ 201 Created {appointmentId}

27. AppointmentBookingComponent → Shows confirmation
    ↓ "Appointment booked successfully!"

28. Hangfire Job → EmailService.SendConfirmationEmail()
    ↓

29. EmailService → SendGrid API
    ↓ Sends email to patient and doctor
```

---

## Key Design Patterns

### Backend (API)

1. **Clean Architecture**: Separation of concerns across layers
2. **CQRS**: Separate models for read and write operations
3. **Mediator Pattern**: MediatR for decoupling request handlers
4. **Repository Pattern**: Implicit via EF Core DbContext
5. **Unit of Work**: Implicit via EF Core transaction management
6. **Dependency Injection**: Built-in ASP.NET Core DI container
7. **Factory Pattern**: For creating domain objects
8. **Strategy Pattern**: For different notification delivery methods

### Frontend (Angular)

1. **Component-Based**: Modular UI components
2. **Module Pattern**: Feature modules and lazy loading
3. **Observer Pattern**: RxJS Observables for async operations
4. **Singleton Pattern**: Services in Core module
5. **Decorator Pattern**: Angular decorators (@Component, @Injectable)
6. **Facade Pattern**: Services as facades for complex operations
7. **Guard Pattern**: Route guards for authentication/authorization

---

## Component Dependencies

### API Dependencies
- Controllers depend on MediatR (no direct business logic)
- Command/Query handlers depend on domain entities
- Infrastructure services implement application interfaces
- Domain layer has no dependencies (pure C#)

### Angular Dependencies
- Feature modules depend on Shared module
- All modules depend on Core module (lazy-loaded don't import it)
- Components depend on services via DI
- Services depend on HttpClient for API calls

---

## Document Metadata

- **Document Version**: 1.0
- **Last Updated**: 2026-01-06
- **Author**: System Architect
- **Status**: Draft
- **C4 Level**: 3 (Component)
