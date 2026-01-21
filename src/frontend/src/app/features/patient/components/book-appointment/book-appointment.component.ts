import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { DoctorService } from '@core/services/doctor.service';
import { AppointmentService } from '@core/services/appointment.service';
import { AuthService } from '@core/services/auth.service';
import { Doctor } from '@core/models/doctor.model';
import { CreateAppointmentRequest } from '@core/models/appointment.model';

@Component({
  selector: 'app-book-appointment',
  templateUrl: './book-appointment.component.html',
  styleUrls: ['./book-appointment.component.scss']
})
export class BookAppointmentComponent implements OnInit {
  doctor: Doctor | null = null;
  bookingForm: FormGroup;
  loading = false;
  error: string | null = null;
  success = false;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private formBuilder: FormBuilder,
    private doctorService: DoctorService,
    private appointmentService: AppointmentService,
    private authService: AuthService
  ) {
    this.bookingForm = this.formBuilder.group({
      scheduledDate: ['', Validators.required],
      startTime: ['', Validators.required],
      endTime: ['', Validators.required],
      reason: ['', [Validators.required, Validators.minLength(5), Validators.maxLength(500)]]
    });
  }

  ngOnInit(): void {
    const doctorId = this.route.snapshot.paramMap.get('id');
    if (doctorId) {
      this.loadDoctorDetails(doctorId);
      this.prefillFromQueryParams();
    }
  }

  loadDoctorDetails(doctorId: string): void {
    this.loading = true;

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

  prefillFromQueryParams(): void {
    this.route.queryParams.subscribe(params => {
      if (params['date']) {
        this.bookingForm.patchValue({ scheduledDate: params['date'] });
      }
      if (params['startTime']) {
        this.bookingForm.patchValue({ startTime: params['startTime'] });
      }
      if (params['endTime']) {
        this.bookingForm.patchValue({ endTime: params['endTime'] });
      }
    });
  }

  onSubmit(): void {
    if (this.bookingForm.invalid) {
      this.markFormGroupTouched(this.bookingForm);
      return;
    }

    if (!this.doctor) {
      this.error = 'Doctor information not available.';
      return;
    }

    const currentUser = this.authService.currentUserValue;
    if (!currentUser?.patientId) {
      this.error = 'Patient information not available. Please log in again.';
      return;
    }

    this.loading = true;
    this.error = null;

    const formValue = this.bookingForm.value;
    const request: CreateAppointmentRequest = {
      patientId: currentUser.patientId,
      doctorId: this.doctor.id,
      scheduledDate: formValue.scheduledDate,
      startTime: formValue.startTime,
      endTime: formValue.endTime,
      reason: formValue.reason
    };

    this.appointmentService.createAppointment(request).subscribe({
      next: (appointmentId) => {
        this.success = true;
        this.loading = false;

        // Redirect to appointment details after 2 seconds
        setTimeout(() => {
          this.router.navigate(['/patient/appointments', appointmentId]);
        }, 2000);
      },
      error: (err) => {
        this.loading = false;

        // Extract error message from response
        if (err.error?.error) {
          this.error = err.error.error;
        } else if (err.error?.Error) {
          this.error = err.error.Error;
        } else if (err.message) {
          this.error = err.message;
        } else {
          this.error = 'Failed to book appointment. Please try again.';
        }

        console.error('Error booking appointment:', err);
      }
    });
  }

  cancel(): void {
    if (this.doctor) {
      this.router.navigate(['/patient/doctors', this.doctor.id]);
    } else {
      this.router.navigate(['/patient/doctors']);
    }
  }

  private markFormGroupTouched(formGroup: FormGroup): void {
    Object.keys(formGroup.controls).forEach(key => {
      const control = formGroup.get(key);
      control?.markAsTouched();
    });
  }

  get minDate(): string {
    return new Date().toISOString().split('T')[0];
  }

  get maxDate(): string {
    const date = new Date();
    date.setMonth(date.getMonth() + 3);
    return date.toISOString().split('T')[0];
  }

  isFieldInvalid(fieldName: string): boolean {
    const field = this.bookingForm.get(fieldName);
    return !!(field && field.invalid && field.touched);
  }

  getFieldError(fieldName: string): string {
    const field = this.bookingForm.get(fieldName);
    if (field?.errors) {
      if (field.errors['required']) {
        return 'This field is required';
      }
      if (field.errors['minlength']) {
        return `Minimum length is ${field.errors['minlength'].requiredLength}`;
      }
      if (field.errors['maxlength']) {
        return `Maximum length is ${field.errors['maxlength'].requiredLength}`;
      }
    }
    return '';
  }
}
