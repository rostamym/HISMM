import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

import { API_ENDPOINTS } from '../constants/api-endpoints';
import {
  Doctor,
  DoctorSearchParams,
  DoctorAvailability,
  TimeSlot,
  SetAvailabilityRequest,
  PaginatedDoctors
} from '../models/doctor.model';

@Injectable({
  providedIn: 'root'
})
export class DoctorService {
  constructor(private http: HttpClient) {}

  /**
   * Transform doctor response to ensure nested structures exist
   */
  private transformDoctor(doctor: any): Doctor {
    return {
      ...doctor,
      user: {
        firstName: doctor.firstName,
        lastName: doctor.lastName,
        email: doctor.email,
        phoneNumber: doctor.phoneNumber
      },
      specialty: {
        id: doctor.specialtyId,
        name: doctor.specialtyName || 'General'
      }
    };
  }

  /**
   * Transform availability to include both formats
   */
  private transformAvailability(availability: any): DoctorAvailability {
    return {
      ...availability,
      formattedStartTime: availability.startTimeFormatted,
      formattedEndTime: availability.endTimeFormatted
    };
  }

  /**
   * Transform time slot to include formatted time and isBooked
   */
  private transformTimeSlot(slot: any): TimeSlot {
    return {
      ...slot,
      formattedTime: slot.displayText || `${slot.startTimeFormatted} - ${slot.endTimeFormatted}`,
      isBooked: !slot.isAvailable
    };
  }

  /**
   * Get all doctors with pagination
   */
  getDoctors(
    page: number = 1,
    pageSize: number = 10,
    sortBy: string = 'rating',
    sortOrder: string = 'desc'
  ): Observable<PaginatedDoctors> {
    let params = new HttpParams()
      .set('page', page.toString())
      .set('pageSize', pageSize.toString())
      .set('sortBy', sortBy)
      .set('sortOrder', sortOrder);

    return this.http.get<any>(API_ENDPOINTS.DOCTORS.BASE, { params }).pipe(
      map(response => ({
        ...response,
        items: response.items.map((doctor: any) => this.transformDoctor(doctor))
      }))
    );
  }

  /**
   * Get doctor by ID
   */
  getDoctorById(id: string): Observable<Doctor> {
    return this.http.get<any>(API_ENDPOINTS.DOCTORS.BY_ID(id)).pipe(
      map(doctor => this.transformDoctor(doctor))
    );
  }

  /**
   * Search doctors with filters
   */
  searchDoctors(searchParams: DoctorSearchParams): Observable<PaginatedDoctors> {
    let params = new HttpParams();

    if (searchParams.searchTerm) {
      params = params.set('searchTerm', searchParams.searchTerm);
    }
    if (searchParams.specialtyId) {
      params = params.set('specialtyId', searchParams.specialtyId);
    }
    if (searchParams.minRating !== undefined) {
      params = params.set('minRating', searchParams.minRating.toString());
    }
    if (searchParams.maxConsultationFee !== undefined) {
      params = params.set(
        'maxConsultationFee',
        searchParams.maxConsultationFee.toString()
      );
    }
    if (searchParams.isAvailable !== undefined) {
      params = params.set('isAvailable', searchParams.isAvailable.toString());
    }
    if (searchParams.page) {
      params = params.set('page', searchParams.page.toString());
    }
    if (searchParams.pageSize) {
      params = params.set('pageSize', searchParams.pageSize.toString());
    }
    if (searchParams.sortBy) {
      params = params.set('sortBy', searchParams.sortBy);
    }
    if (searchParams.sortOrder) {
      params = params.set('sortOrder', searchParams.sortOrder);
    }

    return this.http.get<any>(API_ENDPOINTS.DOCTORS.SEARCH, { params }).pipe(
      map(response => ({
        ...response,
        items: response.items.map((doctor: any) => this.transformDoctor(doctor))
      }))
    );
  }

  /**
   * Get doctor's availability schedule (all days and times)
   */
  getDoctorAvailability(doctorId: string): Observable<DoctorAvailability[]> {
    return this.http.get<any[]>(
      API_ENDPOINTS.DOCTORS.AVAILABILITY(doctorId)
    ).pipe(
      map(availabilities => availabilities.map(a => this.transformAvailability(a)))
    );
  }

  /**
   * Get available time slots for a specific date
   */
  getAvailableTimeSlots(
    doctorId: string,
    date: Date
  ): Observable<TimeSlot[]> {
    const params = new HttpParams().set(
      'date',
      date.toISOString().split('T')[0]
    );

    return this.http.get<any[]>(
      API_ENDPOINTS.DOCTORS.AVAILABLE_SLOTS(doctorId),
      { params }
    ).pipe(
      map(slots => slots.map(slot => this.transformTimeSlot(slot)))
    );
  }

  /**
   * Set doctor availability (Doctor/Admin only)
   */
  setAvailability(
    doctorId: string,
    request: SetAvailabilityRequest
  ): Observable<{ availabilityId: string }> {
    return this.http.post<{ availabilityId: string }>(
      API_ENDPOINTS.DOCTORS.AVAILABILITY(doctorId),
      request
    );
  }

  /**
   * Get doctor's full name
   */
  getDoctorFullName(doctor: Doctor): string {
    return `Dr. ${doctor.firstName} ${doctor.lastName}`;
  }

  /**
   * Format consultation fee for display
   */
  formatConsultationFee(fee: number): string {
    return new Intl.NumberFormat('en-US', {
      style: 'currency',
      currency: 'USD',
      minimumFractionDigits: 0,
      maximumFractionDigits: 0
    }).format(fee);
  }

  /**
   * Get day of week name from number
   */
  getDayOfWeekName(dayOfWeek: number): string {
    const days = [
      'Sunday',
      'Monday',
      'Tuesday',
      'Wednesday',
      'Thursday',
      'Friday',
      'Saturday'
    ];
    return days[dayOfWeek] || '';
  }
}
