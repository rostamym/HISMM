# Phase 4: Background Jobs Testing Guide

## Overview
This document provides comprehensive testing instructions for the Hangfire background jobs implemented in Phase 4.

## Background Jobs Implemented

### 1. AppointmentReminderJob
**Purpose:** Sends reminder emails to patients 24 hours before their scheduled appointments.

**Schedule:** Daily at 7:00 AM

**Logic:**
- Finds appointments scheduled for tomorrow (24-hour window)
- Only processes appointments with status: Scheduled or Confirmed
- Sends professional HTML email with appointment details
- Includes doctor name, specialty, date, time, and reason
- Logs success/failure for each reminder sent

**Testing:**
1. Create an appointment for tomorrow's date
2. Option A - Wait for scheduled run (7:00 AM)
3. Option B - Trigger manually from Hangfire dashboard:
   - Navigate to: https://localhost:7001/hangfire
   - Go to "Recurring jobs" tab
   - Find "appointment-reminders"
   - Click "Trigger now"
4. Check patient's email for reminder
5. Verify logs show successful email delivery

### 2. NoShowMarkerJob
**Purpose:** Automatically marks appointments as "No-Show" if patient didn't arrive.

**Schedule:** Every 30 minutes

**Logic:**
- Finds appointments with status: Scheduled or Confirmed
- Checks if appointment time is 30+ minutes in the past
- Uses domain method `appointment.MarkAsNoShow()`
- Updates appointment status to NoShow
- Logs each appointment marked

**Testing:**
1. Create an appointment with:
   - ScheduledDate: Today's date
   - StartTime: 1 hour ago (e.g., if now is 5:00 PM, set to 4:00 PM)
   - Status: Scheduled
2. Wait for next 30-minute cycle OR trigger manually:
   - Navigate to: https://localhost:7001/hangfire
   - Go to "Recurring jobs" tab
   - Find "no-show-marker"
   - Click "Trigger now"
3. Verify appointment status changed to "No-Show"
4. Check logs for confirmation

### 3. DatabaseCleanupJob
**Purpose:** Maintains database health by deleting old cancelled appointments.

**Schedule:** Weekly on Sunday at 2:00 AM

**Logic:**
- Finds cancelled appointments older than 6 months
- Calculates threshold: `DateTime.UtcNow.AddMonths(-6)`
- Permanently removes these appointments from database
- Logs count of deleted appointments

**Testing:**
1. **Setup Test Data** (using SQL or Entity Framework):
   ```sql
   -- Update an existing cancelled appointment to be old
   UPDATE Appointments
   SET UpdatedAt = DATEADD(MONTH, -7, GETUTCDATE())
   WHERE Status = 2 -- AppointmentStatus.Cancelled
   ```

2. **Trigger Job:**
   - Navigate to: https://localhost:7001/hangfire
   - Go to "Recurring jobs" tab
   - Find "database-cleanup"
   - Click "Trigger now"

3. **Verify:**
   - Check that old cancelled appointments were deleted
   - Check logs for deletion count
   - Verify recent cancelled appointments remain

---

## Hangfire Dashboard

### Accessing the Dashboard
**URL:** https://localhost:7001/hangfire

**Authorization:**
- Development (localhost): Allowed for all users
- Production: Requires Administrator role

### Dashboard Features

#### 1. Recurring Jobs Tab
- View all scheduled recurring jobs
- See next execution time
- Trigger jobs manually for testing
- Enable/disable jobs
- View execution history

#### 2. Jobs Tab
- View all job executions (successful, failed, processing)
- Filter by status
- Retry failed jobs
- View detailed error messages

#### 3. Servers Tab
- View active Hangfire servers
- Monitor worker count (default: 20)
- Check server health

#### 4. Retries Tab
- View failed jobs scheduled for retry
- Manual retry options

---

## Email Template Preview

All background jobs use professional HTML email templates with:
- Hospital branding and colors
- Responsive design (mobile-friendly)
- Clear call-to-action sections
- Professional formatting

### Appointment Reminder Email Includes:
- Patient name
- Doctor name and specialty
- Appointment date and time (formatted)
- Appointment reason
- Hospital contact information
- Professional styling with color accents

---

## Monitoring and Logs

### Application Logs
**Location:** `src/backend/API/logs/log-YYYY-MM-DD.txt`

**Log Entries to Look For:**

**AppointmentReminderJob:**
```
[INF] Starting AppointmentReminderJob execution
[INF] Found {Count} appointments to send reminders for
[INF] Reminder email sent successfully for appointment {AppointmentId}
[INF] AppointmentReminderJob completed. Success: {Success}, Failures: {Failures}
```

**NoShowMarkerJob:**
```
[INF] Starting NoShowMarkerJob execution
[INF] Found {Count} appointments to mark as no-show
[INF] Marked appointment {AppointmentId} as no-show
[INF] NoShowMarkerJob completed. Marked {Count} appointments as no-show
```

**DatabaseCleanupJob:**
```
[INF] Starting DatabaseCleanupJob execution
[INF] Found {Count} old cancelled appointments to delete
[INF] DatabaseCleanupJob completed. Deleted {Count} old cancelled appointments
```

### Hangfire Dashboard Logs
- Navigate to Jobs tab
- Click on specific job execution
- View detailed logs and timing
- Check for exceptions or errors

---

## Testing Checklist

### Initial Setup
- [ ] Backend running on https://localhost:7001
- [ ] Hangfire dashboard accessible at /hangfire
- [ ] All three recurring jobs visible in dashboard
- [ ] Jobs show correct schedule (Daily 7 AM, Every 30 min, Weekly Sunday 2 AM)

### AppointmentReminderJob Testing
- [ ] Created appointment for tomorrow
- [ ] Triggered job manually from dashboard
- [ ] Patient received email reminder
- [ ] Email contains correct appointment details
- [ ] Email is professionally formatted
- [ ] Logs show successful execution

### NoShowMarkerJob Testing
- [ ] Created appointment in the past (30+ minutes ago)
- [ ] Triggered job manually or waited for 30-minute cycle
- [ ] Appointment status changed to "No-Show"
- [ ] Logs confirm appointment was marked
- [ ] UI reflects status change

### DatabaseCleanupJob Testing
- [ ] Created test data (old cancelled appointments)
- [ ] Triggered job manually from dashboard
- [ ] Old appointments deleted from database
- [ ] Recent cancelled appointments remain
- [ ] Logs show correct deletion count

### Error Handling
- [ ] Test with invalid email addresses (check error logs)
- [ ] Test with deleted appointments (check error recovery)
- [ ] Verify jobs continue after single failure
- [ ] Check retry mechanism in Hangfire dashboard

---

## Configuration

### Email Settings
**Location:** `src/backend/API/appsettings.Development.json`

```json
"EmailSettings": {
  "SmtpServer": "smtp.gmail.com",
  "SmtpPort": 587,
  "SmtpUsername": "your-email@gmail.com",
  "SmtpPassword": "your-app-password",
  "FromEmail": "noreply@hospital.com",
  "FromName": "Hospital Appointment System"
}
```

**Note:** For Gmail, use App Password, not regular password.

### Hangfire Settings
**Location:** `src/backend/Infrastructure/DependencyInjection.cs`

**Current Configuration:**
- Storage: SQL Server (uses DefaultConnection)
- Queue Poll Interval: Zero (immediate processing)
- Command Timeout: 5 minutes
- Sliding Invisibility Timeout: 5 minutes
- Worker Count: 20 (default)

### Job Schedules
**Location:** `src/backend/API/Program.cs`

**To modify schedules:**
```csharp
// Daily at different time
RecurringJob.AddOrUpdate<AppointmentReminderJob>(
    "appointment-reminders",
    job => job.ExecuteAsync(),
    Cron.Daily(9), // 9:00 AM instead of 7:00 AM
    new RecurringJobOptions { TimeZone = TimeZoneInfo.Local }
);

// Different interval for no-show marker
RecurringJob.AddOrUpdate<NoShowMarkerJob>(
    "no-show-marker",
    job => job.ExecuteAsync(),
    Cron.Hourly(), // Every hour instead of 30 minutes
    new RecurringJobOptions { TimeZone = TimeZoneInfo.Local }
);
```

---

## Troubleshooting

### Issue: Jobs Not Running
**Check:**
1. Hangfire server is started (check logs for "Starting Hangfire Server")
2. Jobs are enabled in dashboard (not paused)
3. SQL Server connection is working
4. Check for exceptions in logs

### Issue: Emails Not Sending
**Check:**
1. Email settings configured correctly
2. SMTP server accessible
3. Firewall not blocking SMTP port
4. Check email service logs for errors
5. Verify email credentials (use App Password for Gmail)

### Issue: Jobs Failing Silently
**Check:**
1. Hangfire dashboard "Failed" tab
2. Application logs for exceptions
3. Database connectivity
4. Entity Framework migrations applied

### Issue: Hangfire Dashboard Not Accessible
**Check:**
1. Backend is running
2. URL is correct: https://localhost:7001/hangfire
3. Authorization filter configuration
4. Check browser console for errors

---

## Production Considerations

### Security
1. **Hangfire Dashboard Authorization:**
   - Update `HangfireAuthorizationFilter` to require Administrator role
   - Don't allow localhost bypass in production

2. **Email Credentials:**
   - Store in environment variables or Azure Key Vault
   - Never commit credentials to source control

3. **Job Schedules:**
   - Run resource-intensive jobs during off-peak hours
   - Consider time zones for user-facing features

### Performance
1. **Database Cleanup:**
   - Consider increasing retention period for compliance
   - Add indexes on Status and UpdatedAt columns

2. **Email Sending:**
   - Consider rate limiting for bulk emails
   - Use email queue service for high volume

3. **Monitoring:**
   - Set up alerts for failed jobs
   - Monitor job execution duration
   - Track email delivery success rates

---

## Next Steps

After verifying background jobs:

1. **Phase 5: Reporting & Analytics**
   - Appointment statistics
   - Doctor performance metrics
   - Patient appointment history reports
   - Export functionality

2. **Phase 6: Advanced Features**
   - Real-time notifications (SignalR)
   - SMS reminders
   - Multi-language support
   - Advanced search and filtering

3. **Production Deployment**
   - Configure production email service
   - Set up monitoring and alerting
   - Configure SSL certificates
   - Database backup and recovery

---

## Support

For issues or questions:
- Check application logs first
- Review Hangfire dashboard for job status
- Verify configuration settings
- Check email service connectivity
