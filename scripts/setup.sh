#!/bin/bash

# Hospital Appointment Management System - Setup Script
# This script sets up the development environment

set -e  # Exit on error

echo "=========================================="
echo "HISMM - Development Setup"
echo "=========================================="
echo ""

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

# Check if .env exists
if [ ! -f .env ]; then
    echo -e "${YELLOW}Creating .env file from .env.example...${NC}"
    cp .env.example .env
    echo -e "${GREEN}✓ .env file created${NC}"
    echo -e "${YELLOW}Please update .env with your configuration before proceeding${NC}"
    exit 0
fi

# Check prerequisites
echo "Checking prerequisites..."

# Check Docker
if ! command -v docker &> /dev/null; then
    echo -e "${RED}✗ Docker is not installed${NC}"
    echo "Please install Docker Desktop from https://www.docker.com/products/docker-desktop"
    exit 1
fi
echo -e "${GREEN}✓ Docker is installed${NC}"

# Check Docker Compose
if ! command -v docker-compose &> /dev/null; then
    echo -e "${RED}✗ Docker Compose is not installed${NC}"
    echo "Please install Docker Compose"
    exit 1
fi
echo -e "${GREEN}✓ Docker Compose is installed${NC}"

# Check .NET SDK
if ! command -v dotnet &> /dev/null; then
    echo -e "${YELLOW}⚠ .NET SDK is not installed (required for backend)${NC}"
    echo "Install from: https://dotnet.microsoft.com/download/dotnet/8.0"
else
    DOTNET_VERSION=$(dotnet --version)
    echo -e "${GREEN}✓ .NET SDK ${DOTNET_VERSION} is installed${NC}"
fi

# Check Node.js
if ! command -v node &> /dev/null; then
    echo -e "${YELLOW}⚠ Node.js is not installed (required for frontend)${NC}"
    echo "Install from: https://nodejs.org/"
else
    NODE_VERSION=$(node --version)
    echo -e "${GREEN}✓ Node.js ${NODE_VERSION} is installed${NC}"
fi

# Check npm
if ! command -v npm &> /dev/null; then
    echo -e "${YELLOW}⚠ npm is not installed (required for frontend)${NC}"
else
    NPM_VERSION=$(npm --version)
    echo -e "${GREEN}✓ npm ${NPM_VERSION} is installed${NC}"
fi

echo ""
echo "=========================================="
echo "Starting Infrastructure Services"
echo "=========================================="
echo ""

# Start Docker services
echo "Starting Docker containers..."
docker-compose up -d

echo ""
echo "Waiting for SQL Server to be ready..."
sleep 10

# Check SQL Server health
if docker exec hismm-sqlserver /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P "${SQL_SA_PASSWORD:-Hospital@Strong2026!}" -Q "SELECT 1" &> /dev/null; then
    echo -e "${GREEN}✓ SQL Server is ready${NC}"
else
    echo -e "${RED}✗ SQL Server is not responding${NC}"
    echo "Check logs with: docker-compose logs sqlserver"
    exit 1
fi

echo ""
echo "=========================================="
echo "Setup Complete!"
echo "=========================================="
echo ""
echo "Services are running:"
echo "  - SQL Server:  localhost:1433"
echo "  - Adminer:     http://localhost:8080"
echo "  - Redis:       localhost:6379"
echo ""
echo "Next steps:"
echo "  1. cd src/backend"
echo "  2. dotnet restore"
echo "  3. dotnet ef database update --project Infrastructure --startup-project API"
echo "  4. dotnet run --project API"
echo ""
echo "For frontend:"
echo "  1. cd src/frontend/hospital-appointment-app"
echo "  2. npm install"
echo "  3. ng serve"
echo ""
echo "View logs: docker-compose logs -f"
echo "Stop services: docker-compose down"
echo ""
