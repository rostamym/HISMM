import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { AppointmentService } from '@core/services/appointment.service';
import { Appointment, AppointmentStatus } from '@core/models/appointment.model';

@Component({
  selector: 'app-appointment-detail',
  templateUrl: './appointment-detail.component.html',
  styleUrls: ['./appointment-detail.component.scss']
})
export class AppointmentDetailComponent implements OnInit {
  appointment: Appointment | null = null;
  loading = false;
  error: string | null = null;

  // Cancel form
  showCancelDialog = false;
  cancelForm: FormGroup;
  canceling = false;

  // Reschedule form
  showRescheduleDialog = false;
  rescheduleForm: FormGroup;
  rescheduling = false;

  AppointmentStatus = AppointmentStatus;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private formBuilder: FormBuilder,
    private appointmentService: AppointmentService
  ) {
    this.cancelForm = this.formBuilder.group({
      reason: ['', [Validators.required, Validators.minLength(10)]]
    });

    this.rescheduleForm = this.formBuilder.group({
      newDate: ['', Validators.required],
      newStartTime: ['', Validators.required],
      newEndTime: ['', Validators.required]
    });
  }

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.loadAppointment(id);
    }
  }

  loadAppointment(id: string): void {
    this.loading = true;
    this.error = null;

    this.appointmentService.getAppointmentById(id).subscribe({
      next: (appointment) => {
        this.appointment = appointment;
        this.loading = false;
      },
      error: (err) => {
        this.error = 'Failed to load appointment details.';
        this.loading = false;
        console.error('Error loading appointment:', err);
      }
    });
  }

  canCancel(): boolean {
    return this.appointment !== null &&
           this.appointmentService.canCancelAppointment(this.appointment);
  }

  canReschedule(): boolean {
    return this.appointment !== null &&
           this.appointmentService.canRescheduleAppointment(this.appointment);
  }

  openCancelDialog(): void {
    this.showCancelDialog = true;
  }

  closeCancelDialog(): void {
    this.showCancelDialog = false;
    this.cancelForm.reset();
  }

  confirmCancel(): void {
    if (this.cancelForm.invalid || !this.appointment) {
      return;
    }

    this.canceling = true;
    const reason = this.cancelForm.value.reason;

    this.appointmentService.cancelAppointment(this.appointment.id, reason).subscribe({
      next: () => {
        this.canceling = false;
        this.showCancelDialog = false;
        // Reload appointment to show updated status
        this.loadAppointment(this.appointment!.id);
      },
      error: (err) => {
        this.canceling = false;
        this.error = err.error?.error || 'Failed to cancel appointment.';
        console.error('Error canceling appointment:', err);
      }
    });
  }

  openRescheduleDialog(): void {
    this.showRescheduleDialog = true;
  }

  closeRescheduleDialog(): void {
    this.showRescheduleDialog = false;
    this.rescheduleForm.reset();
  }

  confirmReschedule(): void {
    if (this.rescheduleForm.invalid || !this.appointment) {
      return;
    }

    this.rescheduling = true;
    const formValue = this.rescheduleForm.value;

    const request = {
      newScheduledDate: formValue.newDate,
      newStartTime: formValue.newStartTime,
      newEndTime: formValue.newEndTime
    };

    this.appointmentService.rescheduleAppointment(this.appointment.id, request).subscribe({
      next: () => {
        this.rescheduling = false;
        this.showRescheduleDialog = false;
        // Reload appointment to show updated details
        this.loadAppointment(this.appointment!.id);
      },
      error: (err) => {
        this.rescheduling = false;
        this.error = err.error?.error || 'Failed to reschedule appointment.';
        console.error('Error rescheduling appointment:', err);
      }
    });
  }

  goBack(): void {
    this.router.navigate(['/patient/appointments']);
  }

  getStatusClass(status: AppointmentStatus): string {
    switch (status) {
      case AppointmentStatus.Scheduled:
        return 'status-scheduled';
      case AppointmentStatus.Confirmed:
        return 'status-confirmed';
      case AppointmentStatus.InProgress:
        return 'status-in-progress';
      case AppointmentStatus.Completed:
        return 'status-completed';
      case AppointmentStatus.Cancelled:
        return 'status-cancelled';
      case AppointmentStatus.NoShow:
        return 'status-no-show';
      default:
        return '';
    }
  }

  get minRescheduleDate(): string {
    return new Date().toISOString().split('T')[0];
  }

  get maxRescheduleDate(): string {
    const date = new Date();
    date.setMonth(date.getMonth() + 3);
    return date.toISOString().split('T')[0];
  }
}
