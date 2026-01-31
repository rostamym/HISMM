# Hospital Appointment Management System (HISMM)

A comprehensive web-based hospital appointment management system built with Clean Architecture principles using .NET 8 and Angular 17+.

[![.NET](https://img.shields.io/badge/.NET-8.0-512BD4)](https://dotnet.microsoft.com/)
[![Angular](https://img.shields.io/badge/Angular-17-DD0031)](https://angular.io/)
[![SQL Server](https://img.shields.io/badge/SQL%20Server-2022-CC2927)](https://www.microsoft.com/sql-server/)
[![Docker](https://img.shields.io/badge/Docker-Ready-2496ED)](https://www.docker.com/)

---

## 📋 Table of Contents

- [Overview](#overview)
- [Features](#features)
- [Technology Stack](#technology-stack)
- [Project Structure](#project-structure)
- [Getting Started](#getting-started)
  - [Prerequisites](#prerequisites)
  - [Quick Start (Docker-Based)](#quick-start-docker-based)
  - [Manual Setup](#manual-setup)
- [Configuration](#configuration)
- [Running the Application](#running-the-application)
- [Test Accounts](#test-accounts)
- [Access URLs](#access-urls)
- [Development](#development)
- [Architecture](#architecture)
- [API Documentation](#api-documentation)
- [Troubleshooting](#troubleshooting)
- [Contributing](#contributing)

---

## 🎯 Overview

HISMM is a modern hospital appointment management system that enables patients to book appointments with doctors online, allows doctors to manage their schedules and appointments, and provides administrators with comprehensive tools to oversee operations, manage users, and view analytics.

### Current Status

**Completion: ~88%**

- ✅ Phase 1: Patient Features (100%)
- ✅ Phase 2: Doctor Features (100%)
- ✅ Phase 3: Admin Features (100%)
- ✅ Phase 4: Background Jobs & Notifications (100%)
- 🚧 Phase 5: Reporting & Analytics (30%)

---

## ✨ Features

### Patient Portal
- ✅ User registration and login
- ✅ Search and filter doctors by specialty, fee, and availability
- ✅ View doctor profiles with detailed information
- ✅ View available time slots
- ✅ Book appointments with time slot selection
- ✅ View appointment history with status filters
- ✅ Cancel and reschedule appointments
- ✅ Email notifications for all appointment actions

### Doctor Portal
- ✅ Comprehensive dashboard with statistics
- ✅ View today's schedule
- ✅ Manage all appointments with filtering
- ✅ Mark appointments as completed
- ✅ Weekly performance statistics
- ✅ Email notifications for new appointments

### Admin Portal
- ✅ Real-time system statistics dashboard
- ✅ User management (view, search, filter by role/status)
- ✅ User detail modal with appointment history
- ✅ Activate/deactivate user accounts
- ✅ System-wide appointment view with advanced filtering
- ✅ Analytics Dashboard with interactive charts:
  - Appointment trends (daily/weekly/monthly)
  - Status distribution visualization
  - Specialty distribution analysis

### Background Jobs & Notifications
- ✅ Appointment reminder emails (24 hours before)
- ✅ Automatic no-show status marking
- ✅ Database cleanup job for old cancelled appointments
- ✅ Professional HTML email templates
- ✅ Hangfire dashboard for job monitoring

---

## 🛠️ Technology Stack

### Backend
- **Framework**: ASP.NET Core 8.0 Web API
- **Language**: C# 12
- **Architecture**: Clean Architecture + CQRS (MediatR)
- **Database**: SQL Server 2022
- **ORM**: Entity Framework Core 8.0
- **Authentication**: JWT with BCrypt password hashing
- **Background Jobs**: Hangfire with SQL Server storage
- **Validation**: FluentValidation
- **Logging**: Serilog (Console + File)
- **Email**: SMTP (configured for Gmail/SendGrid)

### Frontend
- **Framework**: Angular 17+
- **Language**: TypeScript 5.x
- **UI**: Custom CSS with responsive design
- **Charts**: Chart.js 4.0 + ng2-charts 6.0
- **State Management**: Services with RxJS
- **HTTP Client**: Angular HttpClient with interceptors
- **Routing**: Angular Router with guards

### Infrastructure
- **Database**: SQL Server 2022 (Docker)
- **Database UI**: Adminer (Docker)
- **Containerization**: Docker & Docker Compose

---

## 📁 Project Structure

```
HISMM/
├── src/
│   ├── backend/
│   │   ├── Domain/                      # Entities, Enums, Events
│   │   ├── Application/                 # CQRS Commands/Queries
│   │   │   ├── Features/
│   │   │   │   ├── Authentication/      # Login, Register
│   │   │   │   ├── Appointments/        # Booking, Cancellation, etc.
│   │   │   │   ├── Doctors/             # Doctor queries
│   │   │   │   ├── Admin/               # Admin operations
│   │   │   │   └── Analytics/           # Analytics queries
│   │   │   └── Common/
│   │   │       ├── Interfaces/          # IApplicationDbContext, IEmailService
│   │   │       └── Services/            # EmailTemplateService
│   │   ├── Infrastructure/              # EF Core, Services, Jobs
│   │   │   ├── Persistence/             # DbContext, Migrations, Seeder
│   │   │   ├── Services/                # EmailService
│   │   │   └── BackgroundJobs/          # Hangfire Jobs
│   │   └── API/                         # Controllers, Middleware
│   │       ├── Controllers/
│   │       └── Middleware/
│   └── frontend/
│       └── src/
│           ├── app/
│           │   ├── core/                # Guards, Interceptors, Services
│           │   ├── shared/              # Shared components
│           │   └── features/
│           │       ├── authentication/  # Login, Register
│           │       ├── patient/         # Patient features (8 components)
│           │       ├── doctor/          # Doctor features (3 components)
│           │       └── admin/           # Admin features (9 components)
│           └── environments/
├── docker-compose.yml                   # Docker services configuration
├── nextstep.md                          # Progress tracker
└── README.md                            # This file
```

---

## 🚀 Getting Started

### Prerequisites

**Required:**
- [Docker Desktop](https://www.docker.com/products/docker-desktop) (includes Docker Compose)
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Node.js 18+](https://nodejs.org/) and npm
- [Git](https://git-scm.com/)

**Recommended:**
- [Visual Studio Code](https://code.visualstudio.com/) with C# and Angular extensions
- [SQL Server Management Studio](https://docs.microsoft.com/en-us/sql/ssms/) or [Azure Data Studio](https://docs.microsoft.com/en-us/sql/azure-data-studio/)
- [Postman](https://www.postman.com/) for API testing

### System Requirements
- **OS**: Windows 10/11, macOS, or Linux
- **RAM**: Minimum 8GB (16GB recommended)
- **Disk Space**: 10GB free space
- **Ports**: 1433 (SQL Server), 7001 (Backend), 4200 (Frontend), 8080 (Adminer)

---

## 🐳 Quick Start (Docker-Based)

This is the **recommended approach** for setting up the project in a new environment.

### Step 1: Clone the Repository

```bash
git clone https://github.com/rostamym/HISMM.git
cd HISMM
```

### Step 2: Start Docker Desktop

Make sure Docker Desktop is running on your machine.

**Windows:**
```bash
# Open Docker Desktop from Start Menu
```

**macOS/Linux:**
```bash
# Docker Desktop should be running (check the menu bar/tray)
```

### Step 3: Start SQL Server with Docker Compose

The project includes a `docker-compose.yml` file that sets up SQL Server and Adminer:

```bash
docker-compose up -d
```

This command will:
- Download SQL Server 2022 image (if not already downloaded)
- Download Adminer image (database management UI)
- Create containers for both services
- Start both services in the background

**Verify services are running:**

```bash
docker-compose ps
```

You should see:
```
NAME               IMAGE                                       STATUS
hismm-adminer      adminer:latest                             Up
hismm-sqlserver    mcr.microsoft.com/mssql/server:2022-latest Up (healthy)
```

**View logs (if needed):**

```bash
# All services
docker-compose logs -f

# SQL Server only
docker-compose logs -f sqlserver

# Stop viewing logs: Ctrl+C
```

### Step 4: Setup and Run Backend

Navigate to the backend API directory:

```bash
cd src/backend/API
```

**Restore dependencies:**

```bash
dotnet restore
```

**Build the project:**

```bash
dotnet build
```

**Apply database migrations and seed data:**

The database will be automatically created and seeded when you first run the application. However, you can manually apply migrations if needed:

```bash
cd ..
dotnet ef database update --project Infrastructure --startup-project API
cd API
```

**Run the backend API:**

```bash
dotnet run
```

Or for development with hot reload:

```bash
dotnet watch run
```

The backend will start and you should see:
```
[INFO] Starting Hospital Appointment System API...
[INFO] Hospital Appointment System API started successfully
Now listening on: https://localhost:7001
```

**Keep this terminal running** and open a new terminal for the frontend.

### Step 5: Setup and Run Frontend

Open a **new terminal** and navigate to the frontend directory:

```bash
cd src/frontend
```

**Install dependencies:**

```bash
npm install
```

This will install all required Angular packages (may take a few minutes on first run).

**Start the development server:**

```bash
npm start
```

Or using Angular CLI directly:

```bash
ng serve
```

The frontend will compile and you should see:
```
** Angular Live Development Server is listening on localhost:4200 **
✔ Compiled successfully.
```

### Step 6: Access the Application

Open your browser and navigate to:

**Frontend Application:**
```
http://localhost:4200
```

You should see the Hospital Appointment System login page.

---

## 🔑 Test Accounts

The system is pre-seeded with test accounts for all roles:

### Administrator Account
```
Email: admin@hospital.com
Password: Admin@123
```
**Access:** Admin dashboard, user management, system statistics, analytics

### Doctor Accounts
All doctors have the password: `Doctor@123`

| Email                      | Specialty         | Fee     |
|----------------------------|-------------------|---------|
| dr.smith@hospital.com      | Cardiology        | $150.00 |
| dr.johnson@hospital.com    | Dermatology       | $120.00 |
| dr.williams@hospital.com   | Pediatrics        | $110.00 |
| dr.brown@hospital.com      | Orthopedics       | $140.00 |
| dr.davis@hospital.com      | Neurology         | $180.00 |
| dr.miller@hospital.com     | General Practice  | $100.00 |

**Access:** Doctor dashboard, appointment management

### Patient Accounts
All patients have the password: `Patient@123`

| Email                      | Name             |
|----------------------------|------------------|
| alice.wilson@email.com     | Alice Wilson     |
| bob.anderson@email.com     | Bob Anderson     |
| carol.martinez@email.com   | Carol Martinez   |
| david.thompson@email.com   | David Thompson   |
| emma.garcia@email.com      | Emma Garcia      |

**Access:** Doctor search, appointment booking, appointment management

---

## 🌐 Access URLs

### Frontend
- **Application**: http://localhost:4200
- **Login Page**: http://localhost:4200/login
- **Patient Dashboard**: http://localhost:4200/patient/doctors
- **Doctor Dashboard**: http://localhost:4200/doctor/dashboard
- **Admin Dashboard**: http://localhost:4200/admin/dashboard
- **Analytics Dashboard**: http://localhost:4200/admin/analytics

### Backend
- **API**: https://localhost:7001
- **Swagger UI** (API Documentation): https://localhost:7001/swagger
- **Hangfire Dashboard** (Background Jobs): https://localhost:7001/hangfire

### Database
- **Adminer** (Database Management): http://localhost:8080
  - System: `SQL Server`
  - Server: `sqlserver` (or `localhost,1433` from host)
  - Username: `sa`
  - Password: `Hospital@Strong2026!`
  - Database: `HospitalAppointmentDB_Dev`

---

## ⚙️ Configuration

### Backend Configuration

The backend uses `appsettings.json` and `appsettings.Development.json` for configuration.

**Connection String** (`src/backend/API/appsettings.Development.json`):
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost,1433;Database=HospitalAppointmentDB_Dev;User Id=sa;Password=Hospital@Strong2026!;TrustServerCertificate=true;MultipleActiveResultSets=true"
  }
}
```

**JWT Settings:**
```json
{
  "JwtSettings": {
    "SecretKey": "HospitalAppointmentSystem-SecretKey-Min32Characters-2026-Development",
    "Issuer": "HospitalAppointmentAPI",
    "Audience": "HospitalAppointmentClient",
    "ExpiryInMinutes": 120,
    "RefreshTokenExpiryDays": 30
  }
}
```

**Email Settings** (for Gmail SMTP):
```json
{
  "EmailSettings": {
    "SmtpServer": "smtp.gmail.com",
    "SmtpPort": 587,
    "SenderEmail": "your-email@gmail.com",
    "SenderName": "Hospital Appointment System",
    "Username": "your-email@gmail.com",
    "Password": "your-app-password",
    "EnableSsl": true
  }
}
```

**Note**: For Gmail, you need to generate an [App Password](https://support.google.com/accounts/answer/185833).

### Frontend Configuration

**API Base URL** (`src/frontend/src/environments/environment.ts`):
```typescript
export const environment = {
  production: false,
  apiUrl: 'https://localhost:7001/api'
};
```

### Docker Configuration

**SQL Server Password** (`docker-compose.yml`):

If you want to change the SQL Server password, edit `docker-compose.yml`:

```yaml
services:
  sqlserver:
    environment:
      - SA_PASSWORD=YourNewStrongPassword!
```

Then update the connection string in `appsettings.Development.json` to match.

---

## 🏃 Running the Application

### Full Stack Development Mode

**Terminal 1 - Docker Services:**
```bash
docker-compose up -d
```

**Terminal 2 - Backend:**
```bash
cd src/backend/API
dotnet watch run
```

**Terminal 3 - Frontend:**
```bash
cd src/frontend
npm start
```

### Stopping Services

**Stop Backend:** Press `Ctrl+C` in the backend terminal

**Stop Frontend:** Press `Ctrl+C` in the frontend terminal

**Stop Docker Services:**
```bash
docker-compose down
```

**Stop Docker and Remove Data** (complete reset):
```bash
docker-compose down -v
```

---

## 💻 Development

### Backend Development

**Create a new migration:**
```bash
cd src/backend
dotnet ef migrations add MigrationName --project Infrastructure --startup-project API
```

**Apply migrations:**
```bash
dotnet ef database update --project Infrastructure --startup-project API
```

**Rollback migration:**
```bash
dotnet ef database update PreviousMigrationName --project Infrastructure --startup-project API
```

**Generate SQL script:**
```bash
dotnet ef migrations script --project Infrastructure --startup-project API --output migration.sql
```

### Frontend Development

**Generate a new component:**
```bash
cd src/frontend
ng generate component features/module-name/components/component-name
```

**Build for production:**
```bash
ng build --configuration production
```

**Lint code:**
```bash
ng lint
```

### Code Structure

**Backend follows CQRS pattern:**
- **Commands**: Create, Update, Delete operations (`src/backend/Application/Features/*/Commands`)
- **Queries**: Read operations (`src/backend/Application/Features/*/Queries`)
- **DTOs**: Data Transfer Objects for responses
- **Validators**: FluentValidation validators for request validation

**Frontend follows feature-based structure:**
- **Core**: Shared services, guards, interceptors
- **Features**: Patient, Doctor, Admin modules
- **Components**: Reusable UI components within each feature

---

## 🏗️ Architecture

This project follows **Clean Architecture** principles:

### Architecture Layers

```
┌─────────────────────────────────────────┐
│           Presentation Layer            │
│      (API Controllers, Angular)         │
├─────────────────────────────────────────┤
│         Application Layer               │
│    (Use Cases, Commands, Queries)       │
├─────────────────────────────────────────┤
│           Domain Layer                  │
│    (Entities, Value Objects, Events)    │
├─────────────────────────────────────────┤
│       Infrastructure Layer              │
│  (EF Core, External Services, Jobs)     │
└─────────────────────────────────────────┘
```

### Design Patterns Used

- **CQRS** (Command Query Responsibility Segregation)
- **Mediator Pattern** (MediatR)
- **Repository Pattern** (via EF Core DbContext)
- **Unit of Work** (EF Core transaction management)
- **Dependency Injection** (ASP.NET Core DI container)
- **Builder Pattern** (Fluent API for entity configuration)
- **Observer Pattern** (Domain Events)

### Key Technologies

- **MediatR**: Mediator pattern implementation for CQRS
- **FluentValidation**: Input validation
- **AutoMapper**: Object-to-object mapping (DTOs)
- **Serilog**: Structured logging
- **Hangfire**: Background job processing
- **BCrypt**: Password hashing
- **JWT**: Stateless authentication

---

## 📚 API Documentation

Once the backend is running, access **Swagger UI** at:

### https://localhost:7001/swagger

### Key API Endpoints

#### Authentication
- `POST /api/authentication/register` - Register new user
- `POST /api/authentication/login` - User login
- `POST /api/authentication/refresh-token` - Refresh JWT token

#### Doctors
- `GET /api/doctors` - Get all doctors with pagination
- `GET /api/doctors/{id}` - Get doctor by ID
- `GET /api/doctors/search` - Search doctors with filters
- `GET /api/doctors/{id}/availability` - Get doctor availability
- `GET /api/doctors/{id}/available-slots` - Get available time slots

#### Appointments
- `POST /api/appointments` - Create appointment
- `GET /api/appointments/patient/{patientId}` - Get patient appointments
- `GET /api/appointments/doctor/{doctorId}` - Get doctor appointments
- `GET /api/appointments/{id}` - Get appointment details
- `PATCH /api/appointments/{id}/cancel` - Cancel appointment
- `PATCH /api/appointments/{id}/reschedule` - Reschedule appointment
- `PATCH /api/appointments/{id}/complete` - Mark appointment completed

#### Admin
- `GET /api/admin/statistics` - Get system statistics
- `GET /api/admin/users` - Get all users
- `GET /api/admin/users/{id}` - Get user details
- `GET /api/admin/users/{id}/appointment-history` - Get user appointment history
- `GET /api/admin/appointments` - Get all appointments with filters
- `PATCH /api/admin/users/{id}/toggle-status` - Activate/deactivate user

#### Analytics
- `GET /api/analytics/appointments/trends` - Appointment trends
- `GET /api/analytics/appointments/by-status` - Status distribution
- `GET /api/analytics/appointments/by-specialty` - Specialty distribution
- `GET /api/analytics/doctors/performance` - Doctor performance metrics
- `GET /api/analytics/revenue` - Revenue analytics

---

## 🐛 Troubleshooting

### Docker Issues

**Problem: Docker containers won't start**

```bash
# Check if Docker Desktop is running
docker --version

# Check Docker Compose version
docker-compose --version

# View detailed error logs
docker-compose logs
```

**Problem: Port 1433 already in use**

```bash
# Windows: Find process using port
netstat -ano | findstr :1433

# Kill the process
taskkill /PID <process_id> /F

# macOS/Linux: Find and kill process
lsof -ti:1433 | xargs kill -9
```

**Solution: Stop local SQL Server service** (Windows)
```bash
# Open Services (services.msc)
# Find "SQL Server (SQLEXPRESS)" or similar
# Stop the service
```

### Backend Issues

**Problem: Database connection failed**

1. Ensure SQL Server container is running:
```bash
docker ps | grep sqlserver
```

2. Check SQL Server logs:
```bash
docker logs hismm-sqlserver
```

3. Test connection:
```bash
docker exec -it hismm-sqlserver /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P "Hospital@Strong2026!"
```

4. Verify connection string in `appsettings.Development.json`

**Problem: Database not seeded**

Delete and recreate the database:
```bash
cd src/backend
dotnet ef database drop --project Infrastructure --startup-project API --force
dotnet ef database update --project Infrastructure --startup-project API
```

**Problem: Backend won't start - Port 7001 in use**

```bash
# Windows
netstat -ano | findstr :7001
taskkill /PID <process_id> /F

# macOS/Linux
lsof -ti:7001 | xargs kill -9
```

### Frontend Issues

**Problem: npm install fails**

```bash
# Clear npm cache
npm cache clean --force

# Delete node_modules and package-lock.json
rm -rf node_modules package-lock.json

# Reinstall
npm install
```

**Problem: Angular CLI not found**

```bash
# Install Angular CLI globally
npm install -g @angular/cli

# Verify installation
ng version
```

**Problem: CORS errors**

- Backend CORS is configured to allow `http://localhost:4200`
- Check `src/backend/API/Program.cs` CORS configuration
- Ensure frontend is running on port 4200

**Problem: API calls fail (401 Unauthorized)**

- Ensure you're logged in
- Check JWT token in browser dev tools (Application > Local Storage)
- Token might be expired (default: 120 minutes)

### Database Issues

**Problem: Can't access Adminer**

1. Check if Adminer container is running:
```bash
docker ps | grep adminer
```

2. Access Adminer at: http://localhost:8080

3. Login with:
   - System: SQL Server
   - Server: `sqlserver`
   - Username: `sa`
   - Password: `Hospital@Strong2026!`

**Problem: Migrations fail**

```bash
# Remove all migrations
rm -rf src/backend/Infrastructure/Migrations

# Create initial migration
cd src/backend
dotnet ef migrations add InitialCreate --project Infrastructure --startup-project API

# Apply migration
dotnet ef database update --project Infrastructure --startup-project API
```

### Complete Reset

If nothing works, perform a complete reset:

```bash
# 1. Stop all services
docker-compose down -v

# 2. Remove all containers and volumes
docker system prune -a --volumes

# 3. Delete database in backend
cd src/backend
dotnet ef database drop --project Infrastructure --startup-project API --force

# 4. Clean backend build
dotnet clean
rm -rf */bin */obj

# 5. Clean frontend
cd ../frontend
rm -rf node_modules package-lock.json dist

# 6. Start fresh
cd ../..
docker-compose up -d
cd src/backend/API
dotnet restore
dotnet run

# In new terminal
cd src/frontend
npm install
npm start
```

---

## 🤝 Contributing

### Development Workflow

1. **Create a feature branch:**
```bash
git checkout -b feature/your-feature-name
```

2. **Make changes and commit:**
```bash
git add .
git commit -m "feat: add your feature description"
```

3. **Push to GitHub:**
```bash
git push origin feature/your-feature-name
```

4. **Create a Pull Request** on GitHub

### Commit Convention

We follow [Conventional Commits](https://www.conventionalcommits.org/):

- `feat:` New feature
- `fix:` Bug fix
- `docs:` Documentation changes
- `refactor:` Code refactoring
- `test:` Adding tests
- `chore:` Maintenance tasks

### Code Standards

**Backend (C#):**
- Follow Microsoft C# Coding Conventions
- Use meaningful variable names
- Add XML documentation comments for public APIs
- Write unit tests for business logic

**Frontend (TypeScript/Angular):**
- Follow Angular Style Guide
- Use TypeScript strict mode
- Implement proper error handling
- Use RxJS operators appropriately

---

## 📄 License

This project is proprietary software. All rights reserved.

---

## 🙏 Acknowledgments

- **Clean Architecture** by Robert C. Martin
- **Domain-Driven Design** by Eric Evans
- **.NET Team** at Microsoft
- **Angular Team** at Google
- **Hangfire** by Sergey Odinokov

---

## 📞 Support

### Documentation
- **Progress Tracker**: [nextstep.md](nextstep.md)
- **Repository**: https://github.com/rostamym/HISMM
- **Issues**: https://github.com/rostamym/HISMM/issues

### Contact
- **Project Lead**: Mahdi Rostamy
- **Email**: rostamy.m@gmail.com
- **GitHub**: [@rostamym](https://github.com/rostamym)

---

## 📊 Project Status

**Current Phase**: Phase 5 - Reporting & Analytics (30% Complete)

**Last Updated**: 2026-01-31

**Next Milestone**: Complete Analytics Dashboard with Doctor Performance and Revenue Charts

---

**Built with ❤️ using Clean Architecture and Modern Web Technologies**

**Ready for Production Deployment**
