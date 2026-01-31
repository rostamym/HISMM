import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { API_ENDPOINTS } from '@core/constants/api-endpoints';
import {
  SystemStatistics,
  UserListItem,
  UpdateUserStatusRequest,
  UserFilters,
  AppointmentFilters
} from '@core/models/admin.model';
import { Appointment } from '@core/models/appointment.model';

/**
 * Service for admin operations
 */
@Injectable({
  providedIn: 'root'
})
export class AdminService {
  constructor(private http: HttpClient) {}

  /**
   * Get system statistics
   * @returns Observable with system statistics
   */
  getSystemStatistics(): Observable<SystemStatistics> {
    return this.http.get<SystemStatistics>(API_ENDPOINTS.ADMIN.STATISTICS);
  }

  /**
   * Get all users with optional filters
   * @param filters Optional filters (role, isActive, searchTerm)
   * @returns Observable with list of users
   */
  getAllUsers(filters?: UserFilters): Observable<UserListItem[]> {
    let params = new HttpParams();

    if (filters) {
      if (filters.role) {
        params = params.set('role', filters.role);
      }
      if (filters.isActive !== undefined) {
        params = params.set('isActive', filters.isActive.toString());
      }
      if (filters.searchTerm) {
        params = params.set('searchTerm', filters.searchTerm);
      }
    }

    return this.http.get<UserListItem[]>(API_ENDPOINTS.ADMIN.USERS, { params });
  }

  /**
   * Get user by ID
   * @param userId User ID
   * @returns Observable with user details
   */
  getUserById(userId: string): Observable<UserListItem> {
    return this.http.get<UserListItem>(`${API_ENDPOINTS.ADMIN.USERS}/${userId}`);
  }

  /**
   * Update user active status
   * @param userId User ID
   * @param isActive New active status
   * @returns Observable indicating success
   */
  updateUserStatus(userId: string, isActive: boolean): Observable<void> {
    const request: UpdateUserStatusRequest = { isActive };
    return this.http.patch<void>(
      `${API_ENDPOINTS.ADMIN.USERS}/${userId}/status`,
      request
    );
  }

  /**
   * Activate user
   * @param userId User ID
   * @returns Observable indicating success
   */
  activateUser(userId: string): Observable<void> {
    return this.updateUserStatus(userId, true);
  }

  /**
   * Deactivate user
   * @param userId User ID
   * @returns Observable indicating success
   */
  deactivateUser(userId: string): Observable<void> {
    return this.updateUserStatus(userId, false);
  }

  /**
   * Get all appointments with optional filters
   * @param filters Optional filters (dates, status, patient/doctor IDs, search)
   * @returns Observable with list of appointments
   */
  getAllAppointments(filters?: AppointmentFilters): Observable<Appointment[]> {
    let params = new HttpParams();

    if (filters) {
      if (filters.fromDate) {
        params = params.set('fromDate', filters.fromDate);
      }
      if (filters.toDate) {
        params = params.set('toDate', filters.toDate);
      }
      if (filters.status) {
        params = params.set('status', filters.status);
      }
      if (filters.patientId) {
        params = params.set('patientId', filters.patientId);
      }
      if (filters.doctorId) {
        params = params.set('doctorId', filters.doctorId);
      }
      if (filters.searchTerm) {
        params = params.set('searchTerm', filters.searchTerm);
      }
    }

    return this.http.get<Appointment[]>(API_ENDPOINTS.ADMIN.APPOINTMENTS, { params });
  }

  /**
   * Get appointment history for a specific user
   * @param userId User ID
   * @returns Observable with list of user's appointments
   */
  getUserAppointmentHistory(userId: string): Observable<Appointment[]> {
    return this.http.get<Appointment[]>(`${API_ENDPOINTS.ADMIN.USERS}/${userId}/appointments`);
  }
}
