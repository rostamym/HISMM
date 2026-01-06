# C4 Model - Level 2: Container Diagram
## Hospital Appointment Management System

### Overview
The Container diagram shows the high-level technical building blocks (containers) of the Hospital Appointment Management System. Containers represent applications or data stores that execute code or store data. This diagram zooms into the system context and shows the major containers that make up the system and how they interact.

---

## Container Diagram

```
                          ┌─────────────────────────────────────┐
                          │         External Actors             │
                          │                                     │
                          │  Patient   Doctor   Administrator   │
                          └─────────────┬───────────────────────┘
                                        │
                                        │ HTTPS
                                        ▼
            ┌──────────────────────────────────────────────────────────┐
            │                                                          │
            │              Angular Web Application                     │
            │                                                          │
            │  Single Page Application (SPA)                           │
            │  Technology: Angular 17+, TypeScript                     │
            │  UI Framework: Angular Material / PrimeNG                │
            │  State Management: NgRx / Signals                        │
            │                                                          │
            │  Responsibilities:                                       │
            │  - User interface rendering                              │
            │  - Form validation                                       │
            │  - Client-side routing                                   │
            │  - State management                                      │
            │  - JWT token storage                                     │
            │                                                          │
            │  Served via: IIS / Nginx / Azure Static Web Apps         │
            │  Port: 4200 (dev), 443 (prod)                            │
            └──────────────────────┬───────────────────────────────────┘
                                   │
                                   │ HTTPS / REST API
                                   │ JSON over HTTP
                                   │ Authorization: Bearer {JWT}
                                   │
                                   ▼
            ┌──────────────────────────────────────────────────────────┐
            │                                                          │
            │              ASP.NET Core Web API                        │
            │                                                          │
            │  RESTful API Server                                      │
            │  Technology: ASP.NET Core 8.0, C#                        │
            │  Pattern: Clean Architecture + CQRS                      │
            │  Framework: MediatR, FluentValidation, AutoMapper        │
            │                                                          │
            │  Responsibilities:                                       │
            │  - Business logic execution                              │
            │  - Authentication & authorization (JWT)                  │
            │  - Request validation                                    │
            │  - Data access orchestration                             │
            │  - API endpoints (/api/*)                                │
            │  - Swagger documentation                                 │
            │                                                          │
            │  Hosted on: Azure App Service / IIS / Kestrel            │
            │  Port: 5000 (HTTP), 5001 (HTTPS), 7001 (prod)            │
            └─────────┬─────────────────────────────┬──────────────────┘
                      │                             │
                      │                             │
                      │ SQL Queries                 │ Job Scheduling
                      │ EF Core                     │ Background Tasks
                      │ TCP/IP                      │
                      │                             │
                      ▼                             ▼
       ┌────────────────────────┐      ┌────────────────────────────┐
       │                        │      │                            │
       │   SQL Server Database  │      │   Hangfire Job Processor   │
       │                        │      │                            │
       │  Relational Database   │      │  Background Job Scheduler  │
       │  Technology: SQL Server│      │  Technology: Hangfire.NET  │
       │  Version: 2019/2022    │      │                            │
       │                        │      │  Responsibilities:         │
       │  Stores:               │      │  - Appointment reminders   │
       │  - Users               │      │  - Email queue processing  │
       │  - Doctors             │      │  - Recurring jobs          │
       │  - Patients            │      │  - Job retry logic         │
       │  - Appointments        │      │  - Dashboard at /hangfire  │
       │  - Availability        │      │                            │
       │  - Specialties         │      │  Port: Embedded in API     │
       │  - Notifications       │      └────────────────────────────┘
       │                        │
       │  Port: 1433            │
       └────────────────────────┘


                          API calls to External Services
                          ──────────────────────────────────

            ┌───────────────┐    ┌───────────────┐    ┌───────────────┐
            │               │    │               │    │               │
            │ Email Service │    │  SMS Gateway  │    │Payment Gateway│
            │  (SendGrid)   │    │   (Twilio)    │    │(Stripe/PayPal)│
            │               │    │               │    │               │
            │  HTTPS/REST   │    │  HTTPS/REST   │    │  HTTPS/REST   │
            └───────────────┘    └───────────────┘    └───────────────┘
                    ▲                    ▲                    ▲
                    │                    │                    │
                    └────────────────────┴────────────────────┘
                              API Calls from Web API
```

---

## Container Details

### 1. Angular Web Application
**Type**: Client-Side Web Application

**Technology**:
- Angular 17+ (TypeScript)
- Angular Material or PrimeNG (UI Components)
- NgRx or Angular Signals (State Management)
- RxJS (Reactive Programming)
- Angular Router (Client-Side Routing)

**Description**:
Single Page Application (SPA) that provides the user interface for patients, doctors, and administrators. Runs entirely in the user's web browser and communicates with the backend API via HTTPS.

**Key Responsibilities**:
- Render user interface components
- Handle user input and form validation
- Manage application state (user sessions, cached data)
- Route navigation between views
- Store and send JWT authentication tokens
- Display real-time notifications
- Responsive design for mobile and desktop

**Modules**:
- **Authentication Module**: Login, register, password reset
- **Patient Module**: Doctor search, appointment booking, appointment list
- **Doctor Module**: Dashboard, schedule, availability management
- **Admin Module**: User management, analytics, system configuration
- **Shared Module**: Reusable components, pipes, directives

**API Communication**:
- Protocol: HTTPS
- Format: JSON
- Authentication: JWT Bearer token in Authorization header
- Base URL: `https://api.hospital.com/api`

**Hosting**:
- Development: Angular CLI Dev Server (localhost:4200)
- Production: Static web server (IIS, Nginx, Azure Static Web Apps)

---

### 2. ASP.NET Core Web API
**Type**: Backend API Server

**Technology**:
- ASP.NET Core 8.0 (C#)
- MediatR (CQRS implementation)
- FluentValidation (Input validation)
- AutoMapper (Object mapping)
- Entity Framework Core (ORM)
- ASP.NET Core Identity (Authentication)
- Serilog (Logging)
- Swagger/OpenAPI (API documentation)

**Description**:
RESTful API server that implements the business logic using Clean Architecture principles. Handles all business operations, data access, authentication, and external service integrations.

**Architecture Layers**:
1. **Presentation Layer**: Controllers, middleware, filters
2. **Application Layer**: Use cases (CQRS commands/queries), DTOs, validators
3. **Domain Layer**: Entities, value objects, domain events
4. **Infrastructure Layer**: Database access, external services, background jobs

**Key Responsibilities**:
- Expose RESTful API endpoints
- Authenticate users via JWT tokens
- Authorize requests based on user roles
- Validate incoming requests
- Execute business logic via MediatR handlers
- Query and persist data via Entity Framework Core
- Send notifications via external services
- Schedule background jobs via Hangfire
- Log application events and errors
- Generate API documentation

**API Endpoints** (Examples):
- `POST /api/auth/login` - User authentication
- `GET /api/doctors` - List doctors with filters
- `POST /api/appointments` - Book appointment
- `GET /api/appointments/{id}` - Get appointment details
- `PUT /api/availability` - Update doctor availability
- `GET /api/admin/analytics` - System analytics

**Security**:
- JWT authentication with refresh tokens
- Role-based authorization (Patient, Doctor, Admin)
- HTTPS/TLS encryption
- Input validation and sanitization
- Rate limiting
- CORS configuration
- Secure headers (HSTS, CSP, X-Frame-Options)

**Hosting**:
- Development: Kestrel (localhost:5001)
- Production: Azure App Service, IIS, or Docker container

**Configuration**:
- appsettings.json: Connection strings, JWT settings, email config
- Environment variables: Secrets and environment-specific settings

---

### 3. SQL Server Database
**Type**: Relational Database Management System

**Technology**:
- SQL Server 2019 or 2022
- T-SQL (Transact-SQL)

**Description**:
Primary data store for all application data. Stores user accounts, doctor profiles, appointments, availability schedules, and all transactional data.

**Database Schema** (Key Tables):
- **Users**: User accounts with authentication info
- **Doctors**: Doctor profiles and credentials
- **Patients**: Patient records and medical information
- **Appointments**: Scheduled appointments
- **Availability**: Doctor availability schedules
- **DateBlocks**: Blocked dates for doctors
- **Specialties**: Medical specialties
- **Notifications**: Email/SMS notification history

**Key Features**:
- **ACID Compliance**: Ensures data integrity
- **Indexes**: Performance optimization on frequently queried columns
- **Foreign Keys**: Enforce referential integrity
- **Stored Procedures**: Complex data operations (optional)
- **Triggers**: Audit logging and data validation (optional)
- **Backup**: Automated daily backups

**Access Method**:
- Entity Framework Core from Web API
- Connection String: `Server=.;Database=HospitalAppointmentDB;...`
- Connection Pooling: Managed by EF Core

**Security**:
- Encrypted connections (TLS)
- SQL Server authentication or Windows authentication
- Least privilege access for application user
- Encrypted sensitive data (at rest encryption)
- Regular security updates

**Hosting**:
- Development: Local SQL Server instance or SQL Server Express
- Production: Azure SQL Database, Managed SQL Server, or on-premises

**Port**: 1433 (default SQL Server port)

---

### 4. Hangfire Job Processor
**Type**: Background Job Processing System

**Technology**:
- Hangfire.NET
- Integrated with ASP.NET Core Web API

**Description**:
Background job scheduler embedded within the Web API that handles asynchronous and recurring tasks. Uses SQL Server for job persistence and state management.

**Key Responsibilities**:
- **Appointment Reminders**: Send reminders 24h and 2h before appointments
- **Email Queue Processing**: Process queued email messages
- **Recurring Jobs**: Daily cleanup tasks, report generation
- **Retry Logic**: Automatically retry failed jobs
- **Job Monitoring**: Dashboard for monitoring job execution

**Job Types**:

1. **Recurring Jobs**:
   - `SendAppointmentReminders` - Runs every hour to check for upcoming appointments
   - `CleanupOldNotifications` - Runs daily to archive old notifications
   - `GenerateDailyReports` - Runs nightly to generate reports

2. **Fire-and-Forget Jobs**:
   - Send confirmation email after appointment booking
   - Send notification after appointment cancellation

3. **Delayed Jobs**:
   - Send reminder email exactly 24 hours before appointment
   - Send reminder email exactly 2 hours before appointment

**Dashboard**:
- URL: `/hangfire`
- Access: Admin only (role-based authorization)
- Features: View jobs, retry failed jobs, monitor performance

**Storage**:
- Uses SQL Server database for job persistence
- Separate Hangfire tables in application database

**Configuration**:
- Job retry attempts: 3
- Worker count: Based on CPU cores
- Job expiration: 7 days

---

## External Service Integrations

### Email Service (SendGrid)
**Integration Type**: HTTP REST API

**Communication**:
- Direction: Outbound from Web API
- Protocol: HTTPS
- Authentication: API Key

**Usage**:
- Send appointment confirmations
- Send appointment reminders
- Send password reset emails
- Send registration confirmations

**Error Handling**:
- Retry on failure via Hangfire
- Log email delivery status
- Handle bounces and delivery failures

---

### SMS Gateway (Twilio)
**Integration Type**: HTTP REST API

**Communication**:
- Direction: Outbound from Web API
- Protocol: HTTPS
- Authentication: Account SID + Auth Token

**Usage**:
- Send SMS reminders
- Send urgent notifications
- Two-factor authentication (optional)

**Error Handling**:
- Retry on failure
- Log delivery status
- Handle invalid phone numbers

---

### Payment Gateway (Stripe/PayPal)
**Integration Type**: HTTP REST API

**Communication**:
- Direction: Bidirectional from Web API
- Protocol: HTTPS
- Authentication: API Keys / OAuth

**Usage**:
- Process consultation payments
- Handle refunds for cancelled appointments
- Retrieve payment history
- Generate invoices

**Security**:
- PCI DSS compliance (handled by gateway)
- Tokenization for card data
- Webhook verification

---

## Data Flow Examples

### Example 1: Patient Books Appointment

1. **Angular App** → User fills booking form
2. **Angular App** → HTTP POST `/api/appointments` with JWT token
3. **Web API** → Validates JWT and user permissions
4. **Web API** → Executes `CreateAppointmentCommand` via MediatR
5. **Web API** → Checks doctor availability in **SQL Database**
6. **Web API** → Inserts appointment record in **SQL Database**
7. **Web API** → Enqueues confirmation email job in **Hangfire**
8. **Web API** → Returns success response to **Angular App**
9. **Hangfire** → Sends confirmation email via **SendGrid**
10. **Angular App** → Displays success message

---

### Example 2: Automated Appointment Reminder

1. **Hangfire** → Recurring job runs every hour
2. **Hangfire** → Queries **SQL Database** for appointments in next 24 hours
3. **Hangfire** → For each appointment, enqueues reminder email job
4. **Hangfire** → Processes email job
5. **Web API** → Sends email via **SendGrid** API
6. **Hangfire** → Sends SMS via **Twilio** API (if enabled)
7. **Hangfire** → Updates notification status in **SQL Database**

---

### Example 3: Doctor Updates Availability

1. **Angular App** → Doctor sets availability schedule
2. **Angular App** → HTTP POST `/api/availability` with JWT token
3. **Web API** → Validates JWT and doctor role
4. **Web API** → Executes `SetDoctorAvailabilityCommand` via MediatR
5. **Web API** → Updates availability records in **SQL Database**
6. **Web API** → Returns success response
7. **Angular App** → Updates UI to show new schedule

---

## Deployment Architecture

### Development Environment

```
Developer Machine:
- Angular App: http://localhost:4200
- Web API: https://localhost:5001
- SQL Server: localhost:1433
- Hangfire: Embedded in API (background thread)
```

### Production Environment

```
Azure Cloud / On-Premises:

Load Balancer (HTTPS:443)
    │
    ├─► Angular App (Static Web App / CDN)
    │
    └─► Web API (App Service / VM)
            │
            ├─► SQL Server (Azure SQL / Managed Instance)
            │
            └─► Hangfire (Same process as API)
```

---

## Technology Choices Rationale

### Why ASP.NET Core 8.0?
- High performance and low latency
- Cross-platform (Windows, Linux, macOS)
- Strong typing and compile-time checks
- Excellent tooling and debugging
- Built-in dependency injection
- Mature middleware pipeline
- Enterprise support from Microsoft

### Why Angular 17+?
- Component-based architecture
- Strong typing with TypeScript
- Reactive programming with RxJS
- Comprehensive CLI tooling
- Large ecosystem and community
- Excellent performance with Ivy compiler
- Built-in dependency injection
- Google backing and long-term support

### Why SQL Server?
- ACID compliance for critical healthcare data
- Excellent .NET integration via EF Core
- Mature and proven technology
- Strong security features
- Excellent management tools
- Azure integration for cloud deployment
- Built-in backup and disaster recovery

### Why Hangfire?
- Native .NET integration
- Persistent job storage in SQL Server
- Built-in dashboard for monitoring
- Automatic retry logic
- Easy to configure and use
- No separate infrastructure needed
- Open-source with commercial support

---

## Container Communication Patterns

### Synchronous Communication
- **Angular ↔ Web API**: HTTP REST calls (request/response)
- **Web API ↔ SQL Server**: EF Core queries (synchronous)
- **Web API ↔ External Services**: HTTP REST API calls

### Asynchronous Communication
- **Web API → Hangfire**: Job enqueuing (fire-and-forget)
- **Hangfire → External Services**: Background job execution

### Authentication Flow
1. User logs in via Angular app
2. Angular sends credentials to `/api/auth/login`
3. Web API validates credentials against SQL Server
4. Web API generates JWT token
5. Angular stores JWT in localStorage
6. Angular includes JWT in Authorization header for subsequent requests
7. Web API validates JWT for each protected endpoint

---

## Scalability Considerations

### Horizontal Scaling
- **Angular App**: Served via CDN, infinitely scalable
- **Web API**: Can run multiple instances behind load balancer
- **SQL Server**: Read replicas for query scaling
- **Hangfire**: Supports multiple worker instances

### Vertical Scaling
- **Web API**: Increase CPU and memory as needed
- **SQL Server**: Upgrade to higher tier with more resources

### Caching Strategy (Future)
- Add Redis cache container for session data
- Cache frequently accessed data (specialties, doctor lists)
- Reduce database load

---

## Security Architecture

### Network Security
- All external communication over HTTPS/TLS
- SQL Server accessible only from Web API (private network)
- Firewall rules to restrict access

### Application Security
- JWT authentication with expiration
- Role-based authorization
- Input validation at API layer
- SQL injection prevention via EF Core
- XSS prevention via Angular sanitization
- CSRF protection via Angular tokens

### Data Security
- Passwords hashed with bcrypt
- Sensitive data encrypted at rest
- PHI (Personal Health Information) protection
- HIPAA/GDPR compliance

---

## Monitoring and Observability

### Logging
- Serilog in Web API (structured logging)
- Log levels: Debug, Info, Warning, Error, Fatal
- Log sinks: File, Console, Azure Application Insights

### Monitoring
- Application Insights for performance monitoring
- Hangfire dashboard for job monitoring
- SQL Server performance monitoring
- Angular error tracking (Sentry, Rollbar)

### Health Checks
- API health endpoint: `/health`
- Database connectivity check
- External service availability check

---

## Document Metadata

- **Document Version**: 1.0
- **Last Updated**: 2026-01-06
- **Author**: System Architect
- **Status**: Draft
- **C4 Level**: 2 (Container)
