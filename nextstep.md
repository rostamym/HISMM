# HISMM - Next Steps After Restart

## Prerequisites Installation Checklist

### ✅ Already Completed:
- [x] Environment configuration (.env created)
- [x] Frontend dependencies installed (npm packages)

### 🔲 Required Installations:

#### 1. Docker Desktop
- **Download**: https://www.docker.com/products/docker-desktop
- **Install**: Run Docker Desktop Installer.exe
- **Restart**: Computer restart required after installation
- **Verify**: Open terminal and run `docker --version`

#### 2. .NET 8 SDK
- **Download**: https://dotnet.microsoft.com/download/dotnet/8.0
- **Choose**: Windows x64 installer
- **Install**: Run the installer and follow wizard
- **Verify**: Open terminal and run `dotnet --version` (should show 8.x.x)

---

## After Installation & Restart

### Step 1: Verify Prerequisites
Open a **new terminal** (important for PATH updates) and run:

```bash
docker --version
dotnet --version
node --version
```

All should return version numbers.

### Step 2: Start Docker Desktop
1. Launch Docker Desktop application
2. Wait until it shows "Docker Desktop is running" (check system tray)
3. This may take 1-2 minutes on first launch

### Step 3: Start Infrastructure Services

```bash
# Navigate to project root
cd D:\Work\hismm

# Start SQL Server and Redis containers
docker-compose up -d

# Wait 15-20 seconds for SQL Server to initialize
timeout /t 20

# Verify services are running
docker-compose ps
```

Expected output:
- `hismm-sqlserver` - running on port 1433
- `hismm-adminer` - running on port 8080
- `hismm-redis` - running on port 6379

### Step 4: Setup Backend (.NET API)

```bash
# Navigate to backend directory
cd src\backend

# Restore NuGet packages
dotnet restore

# Run database migrations to create tables
dotnet ef database update --project Infrastructure --startup-project API

# Start the API (in development mode with hot reload)
dotnet watch run --project API
```

**Expected Result:**
- API running at: http://localhost:5000
- Swagger UI at: https://localhost:5001/swagger

**Note:** Leave this terminal open (API will be running)

### Step 5: Start Frontend (Angular)

Open a **new terminal**:

```bash
# Navigate to frontend directory
cd D:\Work\hismm\src\frontend

# Start Angular development server
ng serve
```

**Expected Result:**
- Frontend running at: http://localhost:4200

---

## Access Points

After all services are running:

| Service | URL | Purpose |
|---------|-----|---------|
| **Frontend** | http://localhost:4200 | Angular web application |
| **Backend API** | https://localhost:5001 | REST API |
| **Swagger UI** | https://localhost:5001/swagger | API documentation & testing |
| **Adminer** | http://localhost:8080 | Database management UI |
| **SQL Server** | localhost:1433 | Database (use SSMS or Azure Data Studio) |

### Database Connection (Adminer):
- System: **SQL Server**
- Server: **sqlserver**
- Username: **sa**
- Password: **Hospital@Strong2026!** (from .env file)

---

## Quick Commands Reference

### Docker Management
```bash
# View all containers status
docker-compose ps

# View logs (all services)
docker-compose logs -f

# View logs (specific service)
docker-compose logs -f sqlserver

# Stop all services
docker-compose down

# Stop and remove all data (reset)
docker-compose down -v
```

### Backend Development
```bash
# Run backend with hot reload
cd src\backend
dotnet watch run --project API

# Run all tests
dotnet test

# Create new migration
dotnet ef migrations add MigrationName --project Infrastructure --startup-project API

# Update database
dotnet ef database update --project Infrastructure --startup-project API
```

### Frontend Development
```bash
cd src\frontend

# Start dev server
ng serve

# Run unit tests
ng test

# Build for production
ng build --configuration production
```

---

## Troubleshooting

### Docker Issues

**Problem:** "Cannot connect to Docker daemon"
```bash
# Solution: Ensure Docker Desktop is running
# Check system tray for Docker icon
```

**Problem:** Port 1433 already in use
```bash
# Solution: Stop other SQL Server instances or change port in docker-compose.yml
netstat -ano | findstr :1433
taskkill /PID <process_id> /F
```

### SQL Server Issues

**Problem:** "Login failed for user 'sa'"
```bash
# Solution: Check password in .env matches docker-compose.yml
# Restart containers:
docker-compose down
docker-compose up -d
```

**Problem:** SQL Server container keeps restarting
```bash
# Check logs:
docker-compose logs sqlserver

# Common cause: Insufficient password strength
# Update SQL_SA_PASSWORD in .env with stronger password
```

### .NET Issues

**Problem:** "The EF Core tools version is older than that of the runtime"
```bash
# Solution: Update EF Core tools
dotnet tool update --global dotnet-ef
```

**Problem:** "Could not execute because the specified command or file was not found"
```bash
# Solution: Install EF Core tools
dotnet tool install --global dotnet-ef
```

### Frontend Issues

**Problem:** npm vulnerabilities
```bash
cd src\frontend
npm audit fix
# or for major updates (may break):
npm audit fix --force
```

---

## Next Development Steps

After everything is running, you can:

1. **Explore the API**: Visit https://localhost:5001/swagger
2. **Check Database**: Visit http://localhost:8080 (Adminer)
3. **Develop Features**: See `doc/task.md` for implementation tasks
4. **Review Architecture**: See `doc/architecture/` for detailed docs
5. **Run Tests**: Execute `dotnet test` in backend directory

---

## Getting Help

- **Documentation**: See `doc/` directory
- **README**: Main project documentation
- **Architecture**: `doc/architecture/clean-architecture.md`
- **API Endpoints**: https://localhost:5001/swagger (when running)

---

**Last Updated**: 2026-01-06

**Status**: System initialized, awaiting Docker Desktop and .NET SDK installation

**Resume Command**: After restart and installations, just tell Claude "continue" or run the commands in Step 3 onwards.
