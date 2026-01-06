import { environment } from '@environments/environment';

const API_BASE = environment.apiUrl;

export const API_ENDPOINTS = {
  // Authentication
  AUTH: {
    LOGIN: `${API_BASE}/authentication/login`,
    REGISTER: `${API_BASE}/authentication/register`,
    REFRESH: `${API_BASE}/authentication/refresh`,
    FORGOT_PASSWORD: `${API_BASE}/authentication/forgot-password`,
    RESET_PASSWORD: `${API_BASE}/authentication/reset-password`
  },

  // Doctors
  DOCTORS: {
    BASE: `${API_BASE}/doctors`,
    BY_ID: (id: string) => `${API_BASE}/doctors/${id}`,
    SEARCH: `${API_BASE}/doctors/search`,
    AVAILABILITY: (id: string) => `${API_BASE}/doctors/${id}/availability`
  },

  // Appointments
  APPOINTMENTS: {
    BASE: `${API_BASE}/appointments`,
    BY_ID: (id: string) => `${API_BASE}/appointments/${id}`,
    BY_PATIENT: (patientId: string) =>
      `${API_BASE}/appointments/patient/${patientId}`,
    BY_DOCTOR: (doctorId: string) =>
      `${API_BASE}/appointments/doctor/${doctorId}`,
    RESCHEDULE: (id: string) => `${API_BASE}/appointments/${id}/reschedule`
  },

  // Availability
  AVAILABILITY: {
    BASE: `${API_BASE}/availability`,
    BY_DOCTOR: (doctorId: string) =>
      `${API_BASE}/availability/doctor/${doctorId}`,
    TIME_SLOTS: `${API_BASE}/availability/timeslots`,
    BLOCK_DATE: `${API_BASE}/availability/block-date`
  },

  // Admin
  ADMIN: {
    USERS: `${API_BASE}/admin/users`,
    DOCTORS: `${API_BASE}/admin/doctors`,
    APPOINTMENTS: `${API_BASE}/admin/appointments`,
    ANALYTICS: `${API_BASE}/admin/analytics`,
    ACTIVATE_USER: (id: string) => `${API_BASE}/admin/users/${id}/activate`,
    DEACTIVATE_USER: (id: string) => `${API_BASE}/admin/users/${id}/deactivate`
  }
};
