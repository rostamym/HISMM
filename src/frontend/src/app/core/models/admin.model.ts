/**
 * Admin-related models and interfaces
 */

export interface SystemStatistics {
  totalUsers: number;
  totalPatients: number;
  totalDoctors: number;
  totalAdmins: number;
  totalAppointments: number;
  todayAppointments: number;
  pendingAppointments: number;
  completedAppointments: number;
  cancelledAppointments: number;
}

export interface UserListItem {
  id: string;
  email: string;
  firstName: string;
  lastName: string;
  fullName: string;
  phoneNumber?: string;
  role: string;
  isActive: boolean;
  createdAt: string;
  updatedAt?: string;
  patientId?: string;
  doctorId?: string;
  doctorSpecialty?: string;
  doctorLicenseNumber?: string;
}

export interface UpdateUserStatusRequest {
  isActive: boolean;
}

export interface UserFilters {
  role?: string;
  isActive?: boolean;
  searchTerm?: string;
}

export interface AppointmentFilters {
  fromDate?: string;
  toDate?: string;
  status?: string;
  patientId?: string;
  doctorId?: string;
  searchTerm?: string;
}
