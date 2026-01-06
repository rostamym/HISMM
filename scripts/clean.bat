@echo off
REM Hospital Appointment Management System - Clean Script (Windows)
REM This script stops and removes all containers and volumes

echo ==========================================
echo HISMM - Clean Development Environment
echo ==========================================
echo.
echo This will:
echo   - Stop all Docker containers
echo   - Remove all containers
echo   - Remove all volumes (DATABASE WILL BE DELETED)
echo.

set /p confirm="Are you sure? (yes/no): "

if not "%confirm%"=="yes" (
    echo Cancelled.
    exit /b 0
)

echo.
echo Stopping and removing containers...
docker-compose down -v

echo.
echo Removing orphaned containers...
docker-compose down --remove-orphans

echo.
echo [SUCCESS] Environment cleaned successfully
echo.
echo To start fresh, run: scripts\setup.bat
echo.

pause
