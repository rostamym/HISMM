# Hospital Appointment System - Testing Guide

**Date:** 2026-01-18
**Version:** 1.0
**Status:** Ready for Testing

---

## 🎉 What's Been Completed

### ✅ Backend API (100%)
- Complete appointment booking system
- Doctor availability management
- Email notifications
- All CRUD operations working
- Running on https://localhost:7001

### ✅ Frontend UI (100%)
- Patient dashboard
- Doctor list with search
- Doctor profiles with availability
- Appointment booking flow
- Appointments management

---

## 🚀 Quick Start - Testing the Application

### Step 1: Ensure Backend is Running

The backend should already be running. Verify:

```bash
# Check if the backend is running
# Open browser: https://localhost:7001/swagger

# If not running, start it:
cd D:\Work\hismm\src\backend\API
dotnet run
```

### Step 2: Start the Frontend

```bash
# Navigate to frontend directory
cd D:\Work\hismm\src\frontend

# Install dependencies (if not already done)
npm install

# Start the development server
ng serve

# The app will be available at: http://localhost:4200
```

### Step 3: Access the Application

Open your browser and navigate to: **http://localhost:4200**

---

## 📋 Complete Test Flow

### Test 1: Login as Patient

1. Navigate to http://localhost:4200
2. You should be redirected to the login page
3. Use these credentials:
   - **Email:** alice.wilson@email.com
   - **Password:** Patient@123

4. Click "Login"
5. You should be redirected to `/patient/dashboard`

**Expected Result:** Patient dashboard loads with quick action cards

---

### Test 2: Browse Doctors

1. From the dashboard, click "Find a Doctor" card
2. Or navigate to http://localhost:4200/patient/doctors

**Test the following features:**

#### Search Functionality
- Type a doctor name in the search box
- Click "Search"
- Results should filter based on the name

#### Filters
- Select a specialty from dropdown (if specialties load)
- Set min/max consultation fee
- Check "Available Today Only"
- Click "Search"

#### Clear Filters
- Click "Clear Filters"
- All doctors should display again

#### Pagination
- Click "Next" and "Previous" buttons
- Page numbers should update

**Expected Result:** Doctor cards display with:
- Doctor avatar (initials)
- Name and specialty
- Years of experience
- Consultation fee
- Biography snippet
- "View Profile" and "Book Appointment" buttons

---

### Test 3: View Doctor Profile

1. Click "View Profile" on any doctor card
2. Or navigate to `/patient/doctors/{doctorId}`

**Verify the following:**

#### Doctor Information
- Complete profile with avatar
- Specialty and rating
- License number
- Experience and consultation fee
- Contact information
- Biography

#### Weekly Schedule
- List of days with availability
- Time ranges for each day
- "Not Available" for unavailable days

#### Available Time Slots
- Date picker for selecting appointment date
- Time slots displayed for selected date
- Slots show as available or booked
- Can click a slot to book

**Expected Result:** Complete doctor profile with interactive booking interface

---

### Test 4: Book an Appointment

#### Method 1: From Time Slot
1. On doctor detail page, select a date
2. Click an available time slot
3. You'll be taken to booking form with pre-filled date/time

#### Method 2: Direct Booking
1. Click "Book Appointment" button
2. Manually fill in date and time

**Fill out the booking form:**
- **Date:** Select future date
- **Start Time:** e.g., 09:00
- **End Time:** e.g., 09:30
- **Reason:** "Regular checkup and consultation" (min 5 characters)

**Click "Book Appointment"**

**Expected Result:**
- Form validates correctly
- Success message appears
- Redirected to appointment details (or appointments list)
- Email confirmation generated in `src/backend/API/emails/`

**Test Validation:**
- Try leaving reason empty → Should show error
- Try end time before start time → Should be caught by validation
- Try past date → Should show error

---

### Test 5: View Appointments List

1. Navigate to http://localhost:4200/patient/appointments
2. Or click "My Appointments" from dashboard

**Test the filter tabs:**
- Click "All" → Shows all appointments
- Click "Upcoming" → Shows only future appointments
- Click "Past" → Shows completed/past appointments
- Click "Cancelled" → Shows cancelled appointments

**Verify each appointment card shows:**
- Doctor information with avatar
- Appointment date and time
- Duration
- Consultation fee
- Reason for visit
- Status badge (color-coded)
- "View Details" button

**Expected Result:** Appointments display correctly with filters working

---

### Test 6: View Appointment Details

1. Click "View Details" on any appointment
2. Should navigate to `/patient/appointments/{appointmentId}`

**Note:** This page hasn't been created yet, so you might see a 404 or blank page. This is expected and is a future enhancement.

---

## 🔍 Things to Test

### Frontend Functionality
- [ ] Login works with patient credentials
- [ ] Dashboard loads with action cards
- [ ] Doctor list displays with pagination
- [ ] Search filters work correctly
- [ ] Doctor profile shows complete information
- [ ] Weekly schedule displays correctly
- [ ] Time slots load for selected date
- [ ] Booking form validates input
- [ ] Appointment booking succeeds
- [ ] Appointments list shows all appointments
- [ ] Filter tabs work correctly
- [ ] Responsive design works on mobile (resize browser)

### Backend Integration
- [ ] API calls succeed (check browser console)
- [ ] Error messages display correctly
- [ ] Loading states show during API calls
- [ ] Success confirmations appear

### User Experience
- [ ] Navigation is intuitive
- [ ] Forms are easy to use
- [ ] Error messages are helpful
- [ ] Styling is consistent
- [ ] Mobile responsive layout works

---

## 🐛 Common Issues & Solutions

### Issue 1: Frontend Won't Start
```bash
# Error: Module not found
npm install

# Error: Port 4200 in use
# Kill the process or use different port:
ng serve --port 4201
```

### Issue 2: Backend API Not Responding
```bash
# Check if API is running
# Open: https://localhost:7001/swagger

# If not, start it:
cd D:\Work\hismm\src\backend\API
dotnet run
```

### Issue 3: CORS Errors in Browser Console
- Backend should already have CORS configured
- Check `Program.cs` for CORS policy
- Ensure frontend URL (http://localhost:4200) is allowed

### Issue 4: 401 Unauthorized Errors
- Token might have expired
- Log out and log in again
- Check browser console for authentication errors

### Issue 5: Doctors Not Loading
- Check browser console for errors
- Verify backend API is running
- Test endpoint directly: https://localhost:7001/api/doctors

---

## 📊 Test Data Available

### Patient Accounts
All passwords: **Patient@123**

- alice.wilson@email.com (Blood Group: A+)
- bob.anderson@email.com (Blood Group: O+)
- carol.martinez@email.com (Blood Group: B+)
- david.thompson@email.com (Blood Group: AB+)
- emma.garcia@email.com (Blood Group: A-)

### Doctor Accounts
All passwords: **Doctor@123**

- dr.smith@hospital.com (Cardiologist)
- dr.johnson@hospital.com (Dermatologist)
- dr.williams@hospital.com (Pediatrician)
- dr.brown@hospital.com (Orthopedic Surgeon)
- dr.davis@hospital.com (Neurologist)
- dr.miller@hospital.com (General Practitioner)

### Specialties
- Cardiology
- Dermatology
- Pediatrics
- Orthopedics
- Neurology
- General Practice
- Ophthalmology
- Dentistry

---

## ✅ Success Checklist

Mark these as you test:

- [ ] Backend API running and accessible
- [ ] Frontend starts without errors
- [ ] Can login as patient
- [ ] Dashboard displays correctly
- [ ] Can browse doctor list
- [ ] Can view doctor profiles
- [ ] Can select time slots
- [ ] Can book appointments
- [ ] Can view appointments list
- [ ] Filters work on appointments list
- [ ] Responsive design works
- [ ] No console errors (except expected 404s)

---

## 🎯 Next Steps After Testing

Once you've verified everything works:

1. **Document any bugs found**
   - Create a list of issues
   - Note steps to reproduce
   - Include console errors if any

2. **Provide Feedback**
   - What works well?
   - What needs improvement?
   - Any missing features?

3. **Choose Next Development Phase:**
   - **Option A:** Appointment details page + Cancel/Reschedule
   - **Option B:** Doctor dashboard
   - **Option C:** Background jobs (automated reminders)
   - **Option D:** Admin panel

---

## 🔧 Development Commands Reference

```bash
# Backend
cd D:\Work\hismm\src\backend\API
dotnet build              # Build project
dotnet run                # Run API
dotnet watch run          # Run with hot reload

# Frontend
cd D:\Work\hismm\src\frontend
npm install               # Install dependencies
ng serve                  # Start dev server
ng serve --open           # Start and open browser
ng build                  # Production build

# Database
cd D:\Work\hismm\src\backend\API
dotnet ef database update # Apply migrations
dotnet ef migrations add <Name> --project ..\Infrastructure  # Create migration

# Docker
docker ps                 # List running containers
docker logs hismm-sqlserver  # View SQL Server logs
```

---

## 📸 Screenshots to Capture

For documentation, consider capturing:
1. Patient dashboard
2. Doctor list page
3. Doctor profile with time slots
4. Booking form
5. Appointments list with filters
6. Success confirmation

---

## 📞 Support

If you encounter issues:
1. Check browser console for errors
2. Check backend logs
3. Verify all services are running
4. Review this guide for common solutions

---

**Ready to Test!** 🚀

Start with Step 1 above and work through the test flows. The system should be fully functional for the patient booking experience!

**Estimated Testing Time:** 30-45 minutes for complete flow
