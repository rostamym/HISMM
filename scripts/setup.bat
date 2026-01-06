@echo off
REM Hospital Appointment Management System - Setup Script (Windows)
REM This script sets up the development environment

echo ==========================================
echo HISMM - Development Setup
echo ==========================================
echo.

REM Check if .env exists
if not exist .env (
    echo Creating .env file from .env.example...
    copy .env.example .env
    echo [SUCCESS] .env file created
    echo [WARNING] Please update .env with your configuration before proceeding
    exit /b 0
)

echo Checking prerequisites...

REM Check Docker
docker --version >nul 2>&1
if errorlevel 1 (
    echo [ERROR] Docker is not installed
    echo Please install Docker Desktop from https://www.docker.com/products/docker-desktop
    exit /b 1
)
echo [OK] Docker is installed

REM Check Docker Compose
docker-compose --version >nul 2>&1
if errorlevel 1 (
    echo [ERROR] Docker Compose is not installed
    exit /b 1
)
echo [OK] Docker Compose is installed

REM Check .NET SDK
dotnet --version >nul 2>&1
if errorlevel 1 (
    echo [WARNING] .NET SDK is not installed (required for backend)
    echo Install from: https://dotnet.microsoft.com/download/dotnet/8.0
) else (
    for /f "tokens=*" %%i in ('dotnet --version') do set DOTNET_VERSION=%%i
    echo [OK] .NET SDK %DOTNET_VERSION% is installed
)

REM Check Node.js
node --version >nul 2>&1
if errorlevel 1 (
    echo [WARNING] Node.js is not installed (required for frontend)
    echo Install from: https://nodejs.org/
) else (
    for /f "tokens=*" %%i in ('node --version') do set NODE_VERSION=%%i
    echo [OK] Node.js %NODE_VERSION% is installed
)

REM Check npm
npm --version >nul 2>&1
if errorlevel 1 (
    echo [WARNING] npm is not installed (required for frontend)
) else (
    for /f "tokens=*" %%i in ('npm --version') do set NPM_VERSION=%%i
    echo [OK] npm %NPM_VERSION% is installed
)

echo.
echo ==========================================
echo Starting Infrastructure Services
echo ==========================================
echo.

REM Start Docker services
echo Starting Docker containers...
docker-compose up -d

echo.
echo Waiting for SQL Server to be ready...
timeout /t 10 /nobreak >nul

REM Check SQL Server health
docker exec hismm-sqlserver /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P "Hospital@Strong2026!" -Q "SELECT 1" >nul 2>&1
if errorlevel 1 (
    echo [ERROR] SQL Server is not responding
    echo Check logs with: docker-compose logs sqlserver
    exit /b 1
)
echo [OK] SQL Server is ready

echo.
echo ==========================================
echo Setup Complete!
echo ==========================================
echo.
echo Services are running:
echo   - SQL Server:  localhost:1433
echo   - Adminer:     http://localhost:8080
echo   - Redis:       localhost:6379
echo.
echo Next steps:
echo   1. cd src\backend
echo   2. dotnet restore
echo   3. dotnet ef database update --project Infrastructure --startup-project API
echo   4. dotnet run --project API
echo.
echo For frontend:
echo   1. cd src\frontend\hospital-appointment-app
echo   2. npm install
echo   3. ng serve
echo.
echo View logs: docker-compose logs -f
echo Stop services: docker-compose down
echo.

pause
