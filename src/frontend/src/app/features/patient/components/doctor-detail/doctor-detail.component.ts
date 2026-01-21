import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { DoctorService } from '@core/services/doctor.service';
import { Doctor, DoctorAvailability, TimeSlot } from '@core/models/doctor.model';

@Component({
  selector: 'app-doctor-detail',
  templateUrl: './doctor-detail.component.html',
  styleUrls: ['./doctor-detail.component.scss']
})
export class DoctorDetailComponent implements OnInit {
  doctor: Doctor | null = null;
  availability: DoctorAvailability[] = [];
  availableSlots: TimeSlot[] = [];

  loading = false;
  error: string | null = null;

  selectedDate: Date = new Date();
  loadingSlots = false;
  slotsError: string | null = null;

  daysOfWeek = ['Sunday', 'Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday'];

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private doctorService: DoctorService
  ) {}

  ngOnInit(): void {
    const doctorId = this.route.snapshot.paramMap.get('id');
    if (doctorId) {
      this.loadDoctorDetails(doctorId);
      this.loadAvailability(doctorId);
      this.loadAvailableSlots(doctorId, this.selectedDate);
    }
  }

  loadDoctorDetails(doctorId: string): void {
    this.loading = true;
    this.error = null;

    this.doctorService.getDoctorById(doctorId).subscribe({
      next: (doctor) => {
        this.doctor = doctor;
        this.loading = false;
      },
      error: (err) => {
        this.error = 'Failed to load doctor details.';
        this.loading = false;
        console.error('Error loading doctor:', err);
      }
    });
  }

  loadAvailability(doctorId: string): void {
    this.doctorService.getDoctorAvailability(doctorId).subscribe({
      next: (availability) => {
        this.availability = availability;
      },
      error: (err) => {
        console.error('Error loading availability:', err);
      }
    });
  }

  loadAvailableSlots(doctorId: string, date: Date): void {
    this.loadingSlots = true;
    this.slotsError = null;

    this.doctorService.getAvailableTimeSlots(doctorId, date).subscribe({
      next: (slots) => {
        this.availableSlots = slots;
        this.loadingSlots = false;
      },
      error: (err) => {
        this.slotsError = 'Failed to load available time slots.';
        this.loadingSlots = false;
        console.error('Error loading slots:', err);
      }
    });
  }

  onDateChange(date: Date | string): void {
    if (typeof date === 'string') {
      this.selectedDate = new Date(date);
    } else {
      this.selectedDate = date;
    }

    if (this.doctor) {
      this.loadAvailableSlots(this.doctor.id, this.selectedDate);
    }
  }

  onDateInputChange(event: Event): void {
    const input = event.target as HTMLInputElement;
    if (input.value) {
      this.onDateChange(input.value);
    }
  }

  bookAppointment(slot?: TimeSlot): void {
    if (this.doctor) {
      const queryParams: any = {};
      if (slot) {
        queryParams.date = this.formatDate(this.selectedDate);
        queryParams.startTime = slot.startTime;
        queryParams.endTime = slot.endTime;
      }

      this.router.navigate(['/patient/doctors', this.doctor.id, 'book'], {
        queryParams
      });
    }
  }

  goBack(): void {
    this.router.navigate(['/patient/doctors']);
  }

  formatDate(date: Date): string {
    return date.toISOString().split('T')[0];
  }

  get minDate(): string {
    return this.formatDate(new Date());
  }

  get maxDate(): string {
    const date = new Date();
    date.setMonth(date.getMonth() + 3); // Allow booking up to 3 months in advance
    return this.formatDate(date);
  }

  getDaySchedule(dayOfWeek: number): DoctorAvailability | undefined {
    return this.availability.find(a => a.dayOfWeek === dayOfWeek);
  }

  isAvailableOnDate(date: Date): boolean {
    const dayOfWeek = date.getDay();
    return this.availability.some(a => a.dayOfWeek === dayOfWeek && a.isActive);
  }
}
