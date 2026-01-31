# Admin Feature Testing Guide

## ✅ What Was Completed

### Backend
1. **Admin Queries & Commands Created:**
   - `GetSystemStatisticsQuery` - Returns system-wide statistics
   - `GetAllUsersQuery` - Lists all users with filters (role, active status, search)
   - `GetUserByIdQuery` - Gets single user details
   - `UpdateUserStatusCommand` - Activates/deactivates users

2. **Admin Controller Created:**
   - `GET /api/admin/statistics` - System statistics
   - `GET /api/admin/users?role=&isActive=&searchTerm=` - List users
   - `GET /api/admin/users/{id}` - User details
   - `PATCH /api/admin/users/{id}/status` - Toggle user status
   - **Authorization:** Requires `Administrator` role

3. **Admin User Seeded:**
   - Email: `admin@hospital.com`
   - Password: `Admin@123`
   - Role: Administrator

### Frontend
1. **Admin Models Created:** `admin.model.ts`
2. **Admin Service Created:** `admin.service.ts`
3. **Admin Dashboard:** Connected to real API (displays system statistics)
4. **Users List:** Connected to real API (displays all users, toggle status)

---

## 🚀 How to Test

### Step 1: Start Backend
```bash
cd D:\Work\hismm\src\backend\API
dotnet run
```

**Expected Output:**
```
[INFO] Starting Hospital Appointment System API...
[INFO] Seeded admin user: admin@hospital.com
[INFO] Database seeded successfully
[INFO] Hospital Appointment System API started successfully
[INFO] Now listening on: https://localhost:7001
```

### Step 2: Start Frontend
```bash
cd D:\Work\hismm\src\frontend
ng serve
```

**Access:** http://localhost:4200

### Step 3: Test Admin Login
1. Navigate to: http://localhost:4200/login
2. Enter credentials:
   - Email: `admin@hospital.com`
   - Password: `Admin@123`
3. Click Login
4. **Expected:** Redirected to `/admin/dashboard`

### Step 4: Test Admin Dashboard
Once logged in as admin:

**What You Should See:**
- System statistics (real data from database):
  - Total Users count
  - Total Patients, Doctors, Admins breakdown
  - Total Appointments count
  - Today's Appointments count
  - Pending Appointments
  - Completed Appointments
  - Cancelled Appointments
- Recent Activity section (mock data for now)

**Navigation:**
- Click "Manage Users" button → should navigate to `/admin/users`

### Step 5: Test Users Management
On `/admin/users` page:

**Features to Test:**
1. **View Users List:**
   - Should display all users (patients, doctors, admins) from database
   - Shows: Name, Email, Role badge, Status badge, Created date

2. **Search Users:**
   - Type in search box (searches by name or email)
   - Results filter in real-time

3. **Filter by Role:**
   - Select dropdown: All Roles / Patients / Doctors / Admins
   - List updates automatically

4. **Filter by Status:**
   - Select dropdown: All Status / Active / Inactive
   - List updates automatically

5. **Toggle User Status:**
   - Click "Deactivate" button on an active user
   - User status changes to Inactive (badge turns red)
   - Button text changes to "Activate"
   - Click "Activate" to reactivate
   - **Verify:** Status updates in database

---

## 🧪 API Testing (Using Postman/Curl)

### 1. Login as Admin
```bash
curl -k -X POST https://localhost:7001/api/authentication/login \
  -H "Content-Type: application/json" \
  -d '{
    "email": "admin@hospital.com",
    "password": "Admin@123"
  }'
```

**Expected Response:**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "userId": "...",
  "email": "admin@hospital.com",
  "firstName": "Admin",
  "lastName": "User",
  "role": "Administrator"
}
```

**Save the token** for subsequent requests.

### 2. Get System Statistics
```bash
curl -k -X GET https://localhost:7001/api/admin/statistics \
  -H "Authorization: Bearer YOUR_TOKEN_HERE"
```

**Expected Response:**
```json
{
  "totalUsers": 11,
  "totalPatients": 5,
  "totalDoctors": 6,
  "totalAdmins": 1,
  "totalAppointments": 0,
  "todayAppointments": 0,
  "pendingAppointments": 0,
  "completedAppointments": 0,
  "cancelledAppointments": 0
}
```

### 3. Get All Users
```bash
curl -k -X GET "https://localhost:7001/api/admin/users" \
  -H "Authorization: Bearer YOUR_TOKEN_HERE"
```

**Expected Response:**
```json
[
  {
    "id": "...",
    "email": "admin@hospital.com",
    "firstName": "Admin",
    "lastName": "User",
    "fullName": "Admin User",
    "phoneNumber": "+989131234500",
    "role": "Administrator",
    "isActive": true,
    "createdAt": "2026-01-28T..."
  },
  {
    "id": "...",
    "email": "dr.smith@hospital.com",
    "firstName": "John",
    "lastName": "Smith",
    "role": "Doctor",
    "isActive": true,
    "doctorId": "...",
    "doctorSpecialty": "Cardiology",
    "doctorLicenseNumber": "LIC-001"
  }
  // ... more users
]
```

### 4. Filter Users by Role
```bash
curl -k -X GET "https://localhost:7001/api/admin/users?role=Patient" \
  -H "Authorization: Bearer YOUR_TOKEN_HERE"
```

### 5. Search Users
```bash
curl -k -X GET "https://localhost:7001/api/admin/users?searchTerm=alice" \
  -H "Authorization: Bearer YOUR_TOKEN_HERE"
```

### 6. Get User by ID
```bash
curl -k -X GET https://localhost:7001/api/admin/users/USER_ID_HERE \
  -H "Authorization: Bearer YOUR_TOKEN_HERE"
```

### 7. Deactivate User
```bash
curl -k -X PATCH https://localhost:7001/api/admin/users/USER_ID_HERE/status \
  -H "Authorization: Bearer YOUR_TOKEN_HERE" \
  -H "Content-Type: application/json" \
  -d '{
    "isActive": false
  }'
```

**Expected Response:** 204 No Content

### 8. Activate User
```bash
curl -k -X PATCH https://localhost:7001/api/admin/users/USER_ID_HERE/status \
  -H "Authorization: Bearer YOUR_TOKEN_HERE" \
  -H "Content-Type: application/json" \
  -d '{
    "isActive": true
  }'
```

---

## ✅ Expected Test Results

### Database Check
After seeding, verify admin user exists:
```sql
SELECT * FROM Users WHERE Email = 'admin@hospital.com';
```

**Expected:**
- 1 row returned
- Role = 3 (Administrator)
- IsActive = 1 (True)
- PasswordHash exists

### Authorization Check
Try accessing admin endpoints WITHOUT token:
```bash
curl -k -X GET https://localhost:7001/api/admin/statistics
```

**Expected:** 401 Unauthorized

Try accessing admin endpoints with PATIENT token:
```bash
# Login as patient first
curl -k -X POST https://localhost:7001/api/authentication/login \
  -H "Content-Type: application/json" \
  -d '{
    "email": "alice.wilson@email.com",
    "password": "Patient@123"
  }'

# Try to access admin endpoint
curl -k -X GET https://localhost:7001/api/admin/statistics \
  -H "Authorization: Bearer PATIENT_TOKEN_HERE"
```

**Expected:** 403 Forbidden

---

## 🐛 Troubleshooting

### Backend Won't Start
```bash
# Check if port 7001 is in use
netstat -ano | findstr ":7001"

# If in use, kill the process
taskkill /PID <PID_NUMBER> /F

# Or use different port
dotnet run --urls "https://localhost:7002"
```

### Frontend Build Errors
```bash
cd src/frontend
npm install
npm run build
```

### Database Not Seeded
```bash
cd src/backend/API
dotnet ef database drop --force
dotnet ef database update
dotnet run
```

### Can't Login as Admin
Check database:
```sql
SELECT Email, Role, IsActive FROM Users WHERE Email = 'admin@hospital.com';
```

If user doesn't exist, run seeder again or create manually.

---

## 📊 Test Checklist

- [ ] Backend starts without errors
- [ ] Admin user seeded in database
- [ ] Can login as admin via API
- [ ] GET /api/admin/statistics returns data
- [ ] GET /api/admin/users returns all users
- [ ] Can filter users by role
- [ ] Can filter users by active status
- [ ] Can search users by name/email
- [ ] Can toggle user active status
- [ ] Frontend builds successfully
- [ ] Can login via UI as admin
- [ ] Admin dashboard shows real statistics
- [ ] Users list shows all users
- [ ] Can search/filter in UI
- [ ] Can toggle status in UI
- [ ] Non-admin users cannot access admin pages

---

## 📝 Test Data

### Test Users Available:
```
Admin:
- admin@hospital.com / Admin@123

Doctors:
- dr.smith@hospital.com / Doctor@123
- dr.johnson@hospital.com / Doctor@123
- dr.williams@hospital.com / Doctor@123

Patients:
- alice.wilson@email.com / Patient@123
- bob.anderson@email.com / Patient@123
- carol.martinez@email.com / Patient@123
```

---

## 🎯 Success Criteria

Admin feature is working correctly if:
1. ✅ Admin can login
2. ✅ Admin dashboard displays real database statistics
3. ✅ Admin can view all system users
4. ✅ Admin can search and filter users
5. ✅ Admin can activate/deactivate users
6. ✅ Non-admin users cannot access admin endpoints
7. ✅ All API endpoints return correct data
8. ✅ Frontend properly handles loading and error states

---

**Created:** 2026-01-28
**Last Updated:** 2026-01-28
