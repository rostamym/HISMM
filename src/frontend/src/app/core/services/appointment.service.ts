import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable, catchError, map } from 'rxjs';
import { API_ENDPOINTS } from '@core/constants/api-endpoints';
import {
  Appointment,
  CreateAppointmentRequest,
  RescheduleAppointmentRequest,
  CancelAppointmentRequest,
  AppointmentStatus
} from '@core/models/appointment.model';

/**
 * Service for managing appointments
 */
@Injectable({
  providedIn: 'root'
})
export class AppointmentService {
  constructor(private http: HttpClient) {}

  /**
   * Create a new appointment
   * @param request Appointment details
   * @returns Observable with the created appointment ID
   */
  createAppointment(request: CreateAppointmentRequest): Observable<string> {
    return this.http.post<{ id: string }>(
      API_ENDPOINTS.APPOINTMENTS.BASE,
      request
    ).pipe(
      map(response => response.id)
    );
  }

  /**
   * Get appointment by ID
   * @param id Appointment ID
   * @returns Observable with appointment details
   */
  getAppointmentById(id: string): Observable<Appointment> {
    return this.http.get<Appointment>(
      API_ENDPOINTS.APPOINTMENTS.BY_ID(id)
    );
  }

  /**
   * Get all appointments for a patient
   * @param patientId Patient ID
   * @param statusFilter Optional status filter
   * @param upcomingOnly Filter for upcoming appointments only
   * @returns Observable with list of appointments
   */
  getPatientAppointments(
    patientId: string,
    statusFilter?: AppointmentStatus,
    upcomingOnly: boolean = false
  ): Observable<Appointment[]> {
    let params = new HttpParams();

    if (statusFilter) {
      params = params.set('statusFilter', statusFilter);
    }

    if (upcomingOnly) {
      params = params.set('upcomingOnly', 'true');
    }

    return this.http.get<Appointment[]>(
      API_ENDPOINTS.APPOINTMENTS.BY_PATIENT(patientId),
      { params }
    );
  }

  /**
   * Get all appointments for a doctor
   * @param doctorId Doctor ID
   * @param fromDate Optional start date filter
   * @param toDate Optional end date filter
   * @param statusFilter Optional status filter
   * @returns Observable with list of appointments
   */
  getDoctorAppointments(
    doctorId: string,
    fromDate?: Date,
    toDate?: Date,
    statusFilter?: AppointmentStatus
  ): Observable<Appointment[]> {
    let params = new HttpParams();

    if (fromDate) {
      params = params.set('fromDate', fromDate.toISOString());
    }

    if (toDate) {
      params = params.set('toDate', toDate.toISOString());
    }

    if (statusFilter) {
      params = params.set('statusFilter', statusFilter);
    }

    return this.http.get<Appointment[]>(
      API_ENDPOINTS.APPOINTMENTS.BY_DOCTOR(doctorId),
      { params }
    );
  }

  /**
   * Cancel an appointment
   * @param id Appointment ID
   * @param reason Cancellation reason
   * @returns Observable that completes when the appointment is cancelled
   */
  cancelAppointment(id: string, reason: string): Observable<void> {
    const request: CancelAppointmentRequest = {
      cancellationReason: reason
    };

    return this.http.delete<void>(
      API_ENDPOINTS.APPOINTMENTS.BY_ID(id),
      { body: request }
    );
  }

  /**
   * Reschedule an appointment
   * @param id Appointment ID
   * @param request New appointment details
   * @returns Observable that completes when the appointment is rescheduled
   */
  rescheduleAppointment(
    id: string,
    request: RescheduleAppointmentRequest
  ): Observable<void> {
    return this.http.put<void>(
      API_ENDPOINTS.APPOINTMENTS.RESCHEDULE(id),
      request
    );
  }

  /**
   * Complete an appointment (Doctor only)
   * @param id Appointment ID
   * @param notes Optional completion notes
   * @returns Observable that completes when the appointment is marked as completed
   */
  completeAppointment(id: string, notes: string = ''): Observable<void> {
    return this.http.patch<void>(
      `${API_ENDPOINTS.APPOINTMENTS.BASE}/${id}/complete`,
      { notes }
    );
  }

  /**
   * Get upcoming appointments for a patient
   * @param patientId Patient ID
   * @returns Observable with list of upcoming appointments
   */
  getUpcomingAppointments(patientId: string): Observable<Appointment[]> {
    return this.getPatientAppointments(patientId, undefined, true);
  }

  /**
   * Get appointment history for a patient
   * @param patientId Patient ID
   * @returns Observable with list of past appointments
   */
  getAppointmentHistory(patientId: string): Observable<Appointment[]> {
    return this.getPatientAppointments(patientId);
  }

  /**
   * Get appointments by status
   * @param patientId Patient ID
   * @param status Appointment status
   * @returns Observable with filtered appointments
   */
  getAppointmentsByStatus(
    patientId: string,
    status: AppointmentStatus
  ): Observable<Appointment[]> {
    return this.getPatientAppointments(patientId, status);
  }

  /**
   * Check if an appointment can be cancelled
   * @param appointment Appointment to check
   * @returns True if the appointment can be cancelled
   */
  canCancelAppointment(appointment: Appointment): boolean {
    // Cannot cancel if already cancelled or completed
    if (
      appointment.status === AppointmentStatus.Cancelled ||
      appointment.status === AppointmentStatus.Completed
    ) {
      return false;
    }

    // Cannot cancel appointments in the past
    const appointmentDateTime = new Date(appointment.scheduledDate);
    const now = new Date();

    return appointmentDateTime > now;
  }

  /**
   * Check if an appointment can be rescheduled
   * @param appointment Appointment to check
   * @returns True if the appointment can be rescheduled
   */
  canRescheduleAppointment(appointment: Appointment): boolean {
    // Cannot reschedule if cancelled or completed
    if (
      appointment.status === AppointmentStatus.Cancelled ||
      appointment.status === AppointmentStatus.Completed
    ) {
      return false;
    }

    return true;
  }

  /**
   * Format appointment date and time
   * @param appointment Appointment to format
   * @returns Formatted date and time string
   */
  formatAppointmentDateTime(appointment: Appointment): string {
    const date = new Date(appointment.scheduledDate);
    const dateStr = date.toLocaleDateString('en-US', {
      weekday: 'long',
      year: 'numeric',
      month: 'long',
      day: 'numeric'
    });

    return `${dateStr} at ${appointment.formattedTime || appointment.startTime}`;
  }
}
