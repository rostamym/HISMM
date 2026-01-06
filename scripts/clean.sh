#!/bin/bash

# Hospital Appointment Management System - Clean Script
# This script stops and removes all containers and volumes

echo "=========================================="
echo "HISMM - Clean Development Environment"
echo "=========================================="
echo ""
echo "This will:"
echo "  - Stop all Docker containers"
echo "  - Remove all containers"
echo "  - Remove all volumes (DATABASE WILL BE DELETED)"
echo ""
read -p "Are you sure? (yes/no): " confirm

if [ "$confirm" != "yes" ]; then
    echo "Cancelled."
    exit 0
fi

echo ""
echo "Stopping and removing containers..."
docker-compose down -v

echo ""
echo "Removing orphaned containers..."
docker-compose down --remove-orphans

echo ""
echo "✓ Environment cleaned successfully"
echo ""
echo "To start fresh, run: ./scripts/setup.sh"
echo ""
