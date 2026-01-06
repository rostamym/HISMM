# Hospital Appointment Management System (HISMM)

A comprehensive web-based hospital appointment management system built with Clean Architecture principles using .NET 8 and Angular 17+.

## Overview

This system enables patients to book appointments with doctors, allows doctors to manage their schedules, and provides administrators with tools to oversee operations.

### Key Features

- **Patient Portal**: Search doctors, book appointments, manage bookings
- **Doctor Portal**: Manage availability, view schedules, handle appointments
- **Admin Portal**: User management, analytics, system configuration
- **Automated Notifications**: Email and SMS reminders
- **Payment Processing**: Integrated payment gateway
- **Reporting & Analytics**: Comprehensive dashboards and reports

## Technology Stack

### Backend
- **Framework**: ASP.NET Core 8.0 Web API
- **Language**: C# 12
- **Architecture**: Clean Architecture + CQRS
- **Database**: SQL Server 2022
- **ORM**: Entity Framework Core 8.0
- **Authentication**: JWT with ASP.NET Core Identity
- **Background Jobs**: Hangfire
- **Validation**: FluentValidation
- **Mapping**: AutoMapper
- **Logging**: Serilog

### Frontend
- **Framework**: Angular 17+
- **Language**: TypeScript 5.x
- **UI Library**: Angular Material
- **State Management**: NgRx / Signals
- **Testing**: Jasmine, Karma, Cypress

### Infrastructure
- **Database**: SQL Server (Docker)
- **Cache**: Redis (Optional)
- **Email**: SendGrid / SMTP
- **SMS**: Twilio
- **Payments**: Stripe

## Project Structure

```
HISMM/
├── src/
│   ├── backend/
│   │   ├── Domain/                    # Core business logic
│   │   ├── Application/               # Use cases (CQRS)
│   │   ├── Infrastructure/            # External services
│   │   └── API/                       # Web API
│   └── frontend/
│       └── hospital-appointment-app/  # Angular application
├── tests/
│   ├── Domain.Tests/
│   ├── Application.Tests/
│   ├── Infrastructure.Tests/
│   └── API.Tests/
├── doc/                               # Documentation
│   ├── c4/                           # C4 Model diagrams
│   ├── adr/                          # Architecture Decision Records
│   └── architecture/                 # Detailed architecture docs
├── scripts/                          # Setup and utility scripts
├── docker-compose.yml                # Docker configuration
├── .env.example                      # Environment variables template
└── README.md                         # This file
```

## Documentation

Comprehensive documentation is available in the [doc/](doc/) directory:

### Architecture Documentation
- **[C4 Model](doc/c4/)**: System architecture at 4 levels (Context, Container, Component, Code)
- **[ADRs](doc/adr/)**: Architecture Decision Records explaining key decisions
- **[Architecture Docs](doc/architecture/)**: Detailed implementation documentation

### Product Documentation
- **[Epic](doc/epic.md)**: Product vision and business objectives
- **[User Stories](doc/user-story.md)**: Functional requirements
- **[Tasks](doc/task.md)**: Technical implementation tasks

## Prerequisites

### Required
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Node.js 18+](https://nodejs.org/) and npm
- [Docker Desktop](https://www.docker.com/products/docker-desktop)
- [Git](https://git-scm.com/)

### Recommended
- [Visual Studio 2022](https://visualstudio.microsoft.com/) or [Rider](https://www.jetbrains.com/rider/)
- [VS Code](https://code.visualstudio.com/)
- [SQL Server Management Studio](https://docs.microsoft.com/en-us/sql/ssms/) or [Azure Data Studio](https://docs.microsoft.com/en-us/sql/azure-data-studio/)
- [Postman](https://www.postman.com/) or [Insomnia](https://insomnia.rest/) for API testing

## Quick Start

### 1. Clone the Repository

```bash
git clone https://github.com/rostamym/HISMM.git
cd HISMM
```

### 2. Configure Environment

Copy the example environment file and update with your settings:

```bash
cp .env.example .env
# Edit .env with your configuration
```

**Important**: Update the following in `.env`:
- `SQL_SA_PASSWORD`: Strong password for SQL Server
- `JWT_SECRET`: Generate a secure secret key (min 32 characters)
- Email service credentials (if using SendGrid)
- SMS service credentials (if using Twilio)

### 3. Start Infrastructure Services

Start SQL Server and other services using Docker:

```bash
docker-compose up -d
```

Verify services are running:

```bash
docker-compose ps
```

**Services:**
- SQL Server: `localhost:1433`
- Adminer (DB UI): `http://localhost:8080`
- Redis: `localhost:6379`

### 4. Access Database Management

Open Adminer in your browser: `http://localhost:8080`

**Login credentials:**
- **System**: SQL Server
- **Server**: sqlserver
- **Username**: sa
- **Password**: (from your `.env` file)
- **Database**: (leave empty to see all databases)

### 5. Setup Backend (Coming Soon)

```bash
cd src/backend

# Restore dependencies
dotnet restore

# Create database and run migrations
dotnet ef database update --project Infrastructure --startup-project API

# Run the API
dotnet run --project API
```

API will be available at:
- HTTP: `http://localhost:5000`
- HTTPS: `https://localhost:5001`
- Swagger: `https://localhost:5001/swagger`

### 6. Setup Frontend (Coming Soon)

```bash
cd src/frontend/hospital-appointment-app

# Install dependencies
npm install

# Start development server
ng serve
```

Frontend will be available at: `http://localhost:4200`

## Development Workflow

### Running the Full Stack

1. **Start Infrastructure**:
   ```bash
   docker-compose up -d
   ```

2. **Start Backend**:
   ```bash
   cd src/backend
   dotnet watch run --project API
   ```

3. **Start Frontend**:
   ```bash
   cd src/frontend/hospital-appointment-app
   ng serve
   ```

### Database Migrations

```bash
# Create a new migration
dotnet ef migrations add MigrationName --project Infrastructure --startup-project API

# Update database
dotnet ef database update --project Infrastructure --startup-project API

# Rollback migration
dotnet ef database update PreviousMigrationName --project Infrastructure --startup-project API

# Generate SQL script
dotnet ef migrations script --project Infrastructure --startup-project API --output migration.sql
```

### Running Tests

**Backend:**
```bash
cd src/backend
dotnet test
```

**Frontend:**
```bash
cd src/frontend/hospital-appointment-app

# Unit tests
ng test

# E2E tests
ng e2e
```

## Docker Commands

### Start all services
```bash
docker-compose up -d
```

### Stop all services
```bash
docker-compose down
```

### View logs
```bash
# All services
docker-compose logs -f

# Specific service
docker-compose logs -f sqlserver
```

### Rebuild containers
```bash
docker-compose up -d --build
```

### Remove all data (reset)
```bash
docker-compose down -v
```

## Environment Variables

See [.env.example](.env.example) for all available configuration options.

### Critical Settings

| Variable | Description | Required |
|----------|-------------|----------|
| `SQL_SA_PASSWORD` | SQL Server SA password | Yes |
| `JWT_SECRET` | JWT signing key (min 32 chars) | Yes |
| `JWT_ISSUER` | JWT issuer | Yes |
| `JWT_AUDIENCE` | JWT audience | Yes |
| `SENDGRID_API_KEY` | SendGrid API key for emails | No* |
| `TWILIO_ACCOUNT_SID` | Twilio account SID for SMS | No* |
| `STRIPE_SECRET_KEY` | Stripe secret key for payments | No* |

*Optional but required for full functionality

## Architecture

This project follows **Clean Architecture** principles with clear separation of concerns:

### Layers

1. **Domain Layer** (Core)
   - Entities, Value Objects, Domain Events
   - Business rules and logic
   - No dependencies on other layers

2. **Application Layer**
   - Use cases (Commands & Queries)
   - DTOs, Validators, Interfaces
   - Depends only on Domain

3. **Infrastructure Layer**
   - Database access (EF Core)
   - External services (Email, SMS, Payment)
   - Background jobs (Hangfire)
   - Depends on Application

4. **Presentation Layer** (API)
   - Controllers, Middleware
   - API endpoints
   - Swagger documentation
   - Depends on Application

### Design Patterns

- **CQRS**: Separate read and write operations
- **Mediator**: MediatR for request handling
- **Repository**: Implicit via EF Core DbContext
- **Unit of Work**: EF Core transaction management
- **Dependency Injection**: Built-in ASP.NET Core DI

## API Documentation

Once the backend is running, access Swagger UI at:

**https://localhost:5001/swagger**

### Key Endpoints

- **Authentication**: `/api/auth/*`
- **Doctors**: `/api/doctors/*`
- **Appointments**: `/api/appointments/*`
- **Patients**: `/api/patients/*`
- **Admin**: `/api/admin/*`

## Security

### Implemented Security Measures

- ✅ JWT Authentication
- ✅ Role-based Authorization
- ✅ HTTPS/TLS Encryption
- ✅ Password Hashing (ASP.NET Core Identity)
- ✅ Input Validation (FluentValidation)
- ✅ SQL Injection Prevention (EF Core)
- ✅ XSS Protection (Angular sanitization)
- ✅ CSRF Protection (Angular tokens)
- ✅ Rate Limiting
- ✅ Secure Headers (HSTS, CSP, X-Frame-Options)

### Compliance

- **HIPAA**: Health Insurance Portability and Accountability Act
- **GDPR**: General Data Protection Regulation
- **OWASP Top 10**: Protection against common vulnerabilities

## Troubleshooting

### SQL Server Connection Issues

```bash
# Check if SQL Server is running
docker ps | grep sqlserver

# View SQL Server logs
docker logs hismm-sqlserver

# Test connection
docker exec -it hismm-sqlserver /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P "YourPassword"
```

### Port Already in Use

```bash
# Find process using port
netstat -ano | findstr :1433

# Kill process (Windows)
taskkill /PID <process_id> /F
```

### Database Migration Issues

```bash
# Drop database and recreate
dotnet ef database drop --project Infrastructure --startup-project API
dotnet ef database update --project Infrastructure --startup-project API
```

## Contributing

### Branch Strategy

- `main`: Production-ready code
- `develop`: Development branch
- `feature/*`: Feature branches
- `bugfix/*`: Bug fix branches
- `hotfix/*`: Production hotfixes

### Commit Convention

We follow [Conventional Commits](https://www.conventionalcommits.org/):

```
feat: add appointment booking feature
fix: resolve double booking issue
docs: update API documentation
refactor: improve query performance
test: add unit tests for appointment service
```

### Code Review Process

1. Create feature branch from `develop`
2. Make changes with tests
3. Run all tests locally
4. Create pull request
5. Address review comments
6. Merge after approval

## Deployment

### Development
- Local development with Docker
- SQL Server Developer Edition
- Hot reload enabled

### Staging
- Azure App Service or Docker containers
- Azure SQL Database
- Environment-specific configuration

### Production
- Azure App Service or Kubernetes
- Azure SQL Database with geo-replication
- CDN for static assets
- Application Insights for monitoring

## Monitoring

### Application Insights (Azure)
- Performance monitoring
- Error tracking
- Usage analytics

### Hangfire Dashboard
- Background job monitoring
- Job retry management
- Access: `/hangfire`

### Health Checks
- API health: `/health`
- Database connectivity check
- External service availability

## Support

### Documentation
- [Architecture Documentation](doc/)
- [C4 Model Diagrams](doc/c4/)
- [ADRs](doc/adr/)

### Contact
- **Project Lead**: Mahdi (rostamy.m@gmail.com)
- **GitHub**: https://github.com/rostamym/HISMM
- **Issues**: https://github.com/rostamym/HISMM/issues

## License

[Specify your license here]

## Acknowledgments

- Clean Architecture by Robert C. Martin
- C4 Model by Simon Brown
- .NET Team at Microsoft
- Angular Team at Google

---

**Built with ❤️ using Clean Architecture principles**

Last Updated: 2026-01-06
