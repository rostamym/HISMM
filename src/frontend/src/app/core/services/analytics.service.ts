import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { API_ENDPOINTS } from '../constants/api-endpoints';

export interface AppointmentTrend {
  label: string;
  date: string;
  totalAppointments: number;
  completedAppointments: number;
  cancelledAppointments: number;
  noShowAppointments: number;
  scheduledAppointments: number;
}

export interface AppointmentStatusDistribution {
  status: string;
  count: number;
  percentage: number;
}

export interface AppointmentsBySpecialty {
  specialtyName: string;
  totalAppointments: number;
  completedAppointments: number;
  percentage: number;
  totalRevenue: number;
}

export interface DoctorPerformance {
  doctorId: string;
  doctorName: string;
  specialty: string;
  totalAppointments: number;
  completedAppointments: number;
  cancelledAppointments: number;
  noShowAppointments: number;
  completionRate: number;
  totalRevenue: number;
  averageConsultationFee: number;
  uniquePatients: number;
}

export interface RevenueAnalytics {
  period: string;
  date: string;
  totalRevenue: number;
  completedAppointments: number;
  averageRevenuePerAppointment: number;
  potentialRevenue: number;
  lostRevenue: number;
}

@Injectable({
  providedIn: 'root'
})
export class AnalyticsService {

  constructor(private http: HttpClient) { }

  /**
   * Get appointment trends over time
   * @param period - Period type: 'daily', 'weekly', or 'monthly'
   * @param startDate - Optional start date
   * @param endDate - Optional end date
   */
  getAppointmentTrends(
    period: 'daily' | 'weekly' | 'monthly' = 'daily',
    startDate?: string,
    endDate?: string
  ): Observable<AppointmentTrend[]> {
    let params = new HttpParams().set('period', period);

    if (startDate) {
      params = params.set('startDate', startDate);
    }

    if (endDate) {
      params = params.set('endDate', endDate);
    }

    return this.http.get<AppointmentTrend[]>(
      API_ENDPOINTS.ANALYTICS.TRENDS,
      { params }
    );
  }

  /**
   * Get appointment distribution by status
   * @param startDate - Optional start date for filtering
   * @param endDate - Optional end date for filtering
   */
  getAppointmentsByStatus(
    startDate?: string,
    endDate?: string
  ): Observable<AppointmentStatusDistribution[]> {
    let params = new HttpParams();

    if (startDate) {
      params = params.set('startDate', startDate);
    }

    if (endDate) {
      params = params.set('endDate', endDate);
    }

    return this.http.get<AppointmentStatusDistribution[]>(
      API_ENDPOINTS.ANALYTICS.BY_STATUS,
      { params }
    );
  }

  /**
   * Get appointment distribution by specialty
   * @param startDate - Optional start date for filtering
   * @param endDate - Optional end date for filtering
   */
  getAppointmentsBySpecialty(
    startDate?: string,
    endDate?: string
  ): Observable<AppointmentsBySpecialty[]> {
    let params = new HttpParams();

    if (startDate) {
      params = params.set('startDate', startDate);
    }

    if (endDate) {
      params = params.set('endDate', endDate);
    }

    return this.http.get<AppointmentsBySpecialty[]>(
      API_ENDPOINTS.ANALYTICS.BY_SPECIALTY,
      { params }
    );
  }

  /**
   * Get doctor performance metrics
   * @param startDate - Optional start date for filtering
   * @param endDate - Optional end date for filtering
   * @param specialty - Optional specialty filter
   * @param topCount - Optional top N doctors for leaderboard
   */
  getDoctorPerformance(
    startDate?: string,
    endDate?: string,
    specialty?: string,
    topCount?: number
  ): Observable<DoctorPerformance[]> {
    let params = new HttpParams();

    if (startDate) {
      params = params.set('startDate', startDate);
    }

    if (endDate) {
      params = params.set('endDate', endDate);
    }

    if (specialty) {
      params = params.set('specialty', specialty);
    }

    if (topCount) {
      params = params.set('topCount', topCount.toString());
    }

    return this.http.get<DoctorPerformance[]>(
      API_ENDPOINTS.ANALYTICS.DOCTOR_PERFORMANCE,
      { params }
    );
  }

  /**
   * Get revenue analytics over time
   * @param period - Period type: 'daily', 'weekly', or 'monthly'
   * @param startDate - Optional start date
   * @param endDate - Optional end date
   */
  getRevenueAnalytics(
    period: 'daily' | 'weekly' | 'monthly' = 'daily',
    startDate?: string,
    endDate?: string
  ): Observable<RevenueAnalytics[]> {
    let params = new HttpParams().set('period', period);

    if (startDate) {
      params = params.set('startDate', startDate);
    }

    if (endDate) {
      params = params.set('endDate', endDate);
    }

    return this.http.get<RevenueAnalytics[]>(
      API_ENDPOINTS.ANALYTICS.REVENUE,
      { params }
    );
  }
}
