/**
 * Appointment model interfaces
 */

/**
 * Appointment status enum
 */
export enum AppointmentStatus {
  Scheduled = 'Scheduled',
  Confirmed = 'Confirmed',
  InProgress = 'InProgress',
  Completed = 'Completed',
  Cancelled = 'Cancelled',
  NoShow = 'NoShow'
}

/**
 * Patient information in appointment
 */
export interface PatientInfo {
  id: string;
  firstName: string;
  lastName: string;
  email: string;
  phoneNumber: string;
  fullName?: string;
}

/**
 * Doctor information in appointment
 */
export interface DoctorInfo {
  id: string;
  firstName: string;
  lastName: string;
  email: string;
  phoneNumber: string;
  specialtyName: string;
  licenseNumber: string;
  consultationFee: number;
  fullName?: string;
}

/**
 * Appointment entity
 */
export interface Appointment {
  id: string;
  scheduledDate: string;
  startTime: string;
  endTime: string;
  status: AppointmentStatus;
  reason: string;
  notes?: string;
  createdAt: string;
  updatedAt?: string;
  patient: PatientInfo;
  doctor: DoctorInfo;
  formattedDate?: string;
  formattedTime?: string;
  durationMinutes?: number;
}

/**
 * Create appointment request
 */
export interface CreateAppointmentRequest {
  patientId: string;
  doctorId: string;
  scheduledDate: string;
  startTime: string;
  endTime: string;
  reason: string;
}

/**
 * Reschedule appointment request
 */
export interface RescheduleAppointmentRequest {
  newScheduledDate: string;
  newStartTime: string;
  newEndTime: string;
}

/**
 * Cancel appointment request
 */
export interface CancelAppointmentRequest {
  cancellationReason: string;
}
