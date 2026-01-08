# Hospital Appointment System - Logging & Debugging Guide

This guide explains how to use the comprehensive logging system to debug issues like "Error 0: Unknown Error".

## Table of Contents
1. [Frontend Logging](#frontend-logging)
2. [Backend Logging](#backend-logging)
3. [Debugging Common Errors](#debugging-common-errors)
4. [Log Locations](#log-locations)

---

## Frontend Logging

### Browser Console Logs

All HTTP requests and responses are automatically logged to the browser console with detailed information.

#### Viewing Frontend Logs

1. Open your browser's Developer Tools (F12)
2. Go to the **Console** tab
3. You'll see color-coded logs:
   - 🚀 Blue: Outgoing HTTP requests
   - ✅ Green: Successful responses
   - ❌ Red: Error responses

#### Example Log Output

**Successful Request:**
```
🚀 HTTP Request: POST http://host.docker.internal:5000/api/authentication/register
  Headers: ["Content-Type: application/json"]
  Body: {email: "user@test.com", password: "***", ...}

✅ HTTP Response: POST http://host.docker.internal:5000/api/authentication/register (1234ms)
  Status: 200 OK
  Body: {token: "...", user: {...}}
```

**Error Request:**
```
❌ HTTP Error: POST http://host.docker.internal:5000/api/authentication/register (2000ms)
  Status: 0
  Error: {...}
  Message: Http failure response for ...: 0 Unknown Error

  ⚠️ Status 0 Error - This usually means:
    - Network connection failed
    - CORS issue
    - Backend server is not running
    - Request was blocked by browser
  Check:
    1. Is backend running at http://host.docker.internal:5000?
    2. Are CORS headers configured correctly?
    3. Check browser Network tab for more details
```

### Network Tab

For more detailed request/response inspection:

1. Open Developer Tools (F12)
2. Go to **Network** tab
3. Filter by XHR to see API calls
4. Click on any request to see:
   - Headers
   - Request payload
   - Response
   - Timing information

---

## Backend Logging

### Console Logs (Real-time)

The backend logs to the console with structured format using Serilog.

#### Log Format
```
[HH:mm:ss LEVEL] SourceContext
Message
Exception (if any)
```

#### Example Logs

**Application Startup:**
```
[21:00:00 INF] Starting Hospital Appointment System API...
[21:00:01 INF] Hospital Appointment System API started successfully
```

**HTTP Requests:**
```
[21:00:05 INF] HTTP POST /api/authentication/register responded 200 in 234.5678ms
```

**Errors:**
```
[21:00:10 ERR] Unhandled exception occurred
Path: /api/authentication/register
Method: POST
User: Anonymous
RemoteIP: ::1
Exception Type: SqlException
Message: Cannot open database 'HospitalAppointmentDB' requested by the login
StackTrace: at Microsoft.Data.SqlClient...
```

### File Logs

Logs are also written to files for persistent storage.

**Location:** `src/backend/API/logs/`

**Files:**
- `log-20260106.txt` - Daily log file (format: log-YYYYMMDD.txt)

**File Log Format:**
```
[2026-01-06 21:00:00.000 +03:30] [INF] SourceContext Message
```

**Viewing Log Files:**
```bash
# View today's log
cat src/backend/API/logs/log-$(date +%Y%m%d).txt

# Tail logs in real-time
tail -f src/backend/API/logs/log-$(date +%Y%m%d).txt

# Search for errors
grep "ERR" src/backend/API/logs/log-*.txt
```

---

## Debugging Common Errors

### Error 0: Unknown Error

**Symptom:** Frontend shows "Error 0: Unknown Error"

**Console Output:**
```
❌ HTTP Error: POST http://host.docker.internal:5000/api/authentication/register
  Status: 0
  ⚠️ Status 0 Error - This usually means:
    - Network connection failed
    - CORS issue
    - Backend server is not running
```

**Debugging Steps:**

1. **Check if backend is running:**
   ```bash
   # Check backend process
   curl http://localhost:5000/api/health

   # Or check docker logs for backend
   docker logs <backend-container>
   ```

2. **Verify API URL in frontend:**
   - Check `src/frontend/src/environments/environment.ts`
   - Should be: `http://host.docker.internal:5000/api`

3. **Check CORS configuration:**
   - Backend `appsettings.json`: `CorsSettings:AllowedOrigins`
   - Should include frontend URL

4. **Network Tab Analysis:**
   - Look for failed requests (red)
   - Check if request was even sent
   - Look for CORS errors in console

### Validation Errors (400 Bad Request)

**Console Output:**
```
❌ HTTP Error: POST /api/authentication/register
  Status: 400 Bad Request
  Error: {
    errors: {
      Password: ["Password must be at least 8 characters"],
      Email: ["Invalid email format"]
    }
  }
```

**Backend Log:**
```
[21:05:00 INF] HTTP POST /api/authentication/register responded 400 in 50ms
```

**Action:** Fix validation errors shown in the message.

### Network/Connection Errors

**Check:**
1. Backend is running: `docker ps` or check process
2. Database is running: `docker ps | grep sqlserver`
3. Network connectivity
4. Firewall settings

### Database Errors

**Backend Log Example:**
```
[21:10:00 ERR] Unhandled exception occurred
Exception Type: SqlException
Message: Cannot open database 'HospitalAppointmentDB_Dev'
```

**Actions:**
1. Verify database is running
2. Check connection string in `appsettings.json`
3. Run migrations: `dotnet ef database update`

---

## Log Locations

### Frontend

| What | Where |
|------|-------|
| Console logs | Browser DevTools > Console tab |
| Network requests | Browser DevTools > Network tab |
| Error notifications | On-screen toast messages |

### Backend

| What | Where |
|------|-------|
| Real-time logs | Terminal where backend is running |
| Persisted logs | `src/backend/API/logs/log-YYYYMMDD.txt` |
| Application errors | Exception middleware logs |
| HTTP requests | Serilog request logging |
| Database queries | EF Core logs (when enabled) |

---

## Advanced Debugging

### Enable Verbose Logging

**Frontend (environment.ts):**
```typescript
export const environment = {
  production: false,
  logLevel: 'debug', // Add this
  ...
};
```

**Backend (appsettings.json):**
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Debug",
      "Microsoft.EntityFrameworkCore": "Information"
    }
  }
}
```

### Filter Logs

**Frontend Console:**
```javascript
// Filter by keyword
// In console, use browser's filter feature

// Or check specific service
localStorage.setItem('debug', 'auth:*');
```

**Backend:**
```bash
# Filter file logs
grep "authentication" src/backend/API/logs/log-*.txt

# Filter by log level
grep "\[ERR\]" src/backend/API/logs/log-*.txt

# Show only SQL queries
grep "Microsoft.EntityFrameworkCore" src/backend/API/logs/log-*.txt
```

### Track Specific User

**Backend adds User context automatically:**
```
User: sarah.patient@hospital.com
RemoteIP: 172.18.0.1
```

Search logs for specific user:
```bash
grep "sarah.patient@hospital.com" src/backend/API/logs/log-*.txt
```

---

## Quick Troubleshooting Checklist

When you see an error:

- [ ] Check browser console for HTTP logs
- [ ] Check Network tab for request details
- [ ] Check backend console for error logs
- [ ] Check backend log file for full stack trace
- [ ] Verify all services are running (docker ps)
- [ ] Check CORS configuration
- [ ] Verify API URL in frontend environment
- [ ] Check database connection string
- [ ] Look for validation errors in response

---

## Example Debugging Session

**Problem:** Registration fails with "Error 0: Unknown Error"

**Step 1: Check Browser Console**
```
❌ HTTP Error: POST http://host.docker.internal:5000/api/authentication/register
Status: 0
⚠️ Network Error: Cannot connect to http://host.docker.internal:5000/api/authentication/register
```

**Step 2: Check if Backend is Running**
```bash
$ curl http://localhost:5000/api/health
# No response? Backend is not running!
```

**Step 3: Start Backend**
```bash
$ cd src/backend && dotnet run --project API
[21:15:00 INF] Starting Hospital Appointment System API...
[21:15:01 INF] Hospital Appointment System API started successfully
```

**Step 4: Try Again**
```
✅ HTTP Response: POST http://host.docker.internal:5000/api/authentication/register (250ms)
Status: 200 OK
```

**Success!** The issue was that the backend wasn't running.

---

## Need More Help?

If you still can't identify the issue:

1. Enable verbose logging in both frontend and backend
2. Collect logs from:
   - Browser console
   - Browser Network tab
   - Backend console
   - Backend log file
3. Check database logs: `docker logs hismm-sqlserver`
4. Review this guide's debugging sections

---

## Summary

You now have comprehensive logging at every level:

- ✅ **Frontend HTTP Interceptor** - Logs all API calls with details
- ✅ **Enhanced Error Interceptor** - Provides helpful error messages
- ✅ **Backend Serilog** - Structured logging to console and files
- ✅ **HTTP Request Logging** - Automatic request/response logging
- ✅ **Exception Middleware** - Detailed error information

Next time you see "Error 0: Unknown Error", just open the browser console to see exactly what's happening!
