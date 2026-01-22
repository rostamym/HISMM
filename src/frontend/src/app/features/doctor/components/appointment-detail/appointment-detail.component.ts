import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { AppointmentService } from '@core/services/appointment.service';
import { Appointment, AppointmentStatus } from '@core/models/appointment.model';

@Component({
  selector: 'app-doctor-appointment-detail',
  templateUrl: './appointment-detail.component.html',
  styleUrls: ['./appointment-detail.component.scss']
})
export class DoctorAppointmentDetailComponent implements OnInit {
  appointment: Appointment | null = null;
  loading = true;
  error: string | null = null;

  // Complete dialog
  showCompleteDialog = false;
  completeForm: FormGroup;
  completing = false;

  AppointmentStatus = AppointmentStatus;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private appointmentService: AppointmentService,
    private fb: FormBuilder
  ) {
    this.completeForm = this.fb.group({
      notes: ['', [Validators.maxLength(500)]]
    });
  }

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.loadAppointment(id);
    } else {
      this.error = 'Invalid appointment ID';
      this.loading = false;
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
        this.error = 'Failed to load appointment details';
        this.loading = false;
        console.error('Error loading appointment:', err);
      }
    });
  }

  canComplete(): boolean {
    if (!this.appointment) return false;

    return (
      this.appointment.status === AppointmentStatus.Scheduled ||
      this.appointment.status === AppointmentStatus.Confirmed ||
      this.appointment.status === AppointmentStatus.InProgress
    );
  }

  openCompleteDialog(): void {
    if (this.canComplete()) {
      this.showCompleteDialog = true;
      this.completeForm.reset();
    }
  }

  closeCompleteDialog(): void {
    this.showCompleteDialog = false;
    this.completeForm.reset();
  }

  confirmComplete(): void {
    if (this.completeForm.invalid || !this.appointment) return;

    this.completing = true;
    const notes = this.completeForm.value.notes || '';

    this.appointmentService.completeAppointment(this.appointment.id, notes).subscribe({
      next: () => {
        this.completing = false;
        this.showCompleteDialog = false;
        this.loadAppointment(this.appointment!.id);
      },
      error: (err) => {
        this.completing = false;
        this.error = err.error?.error || 'Failed to complete appointment';
        console.error('Error completing appointment:', err);
      }
    });
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

  getStatusText(status: AppointmentStatus): string {
    switch (status) {
      case AppointmentStatus.Scheduled:
        return 'Scheduled';
      case AppointmentStatus.Confirmed:
        return 'Confirmed';
      case AppointmentStatus.InProgress:
        return 'In Progress';
      case AppointmentStatus.Completed:
        return 'Completed';
      case AppointmentStatus.Cancelled:
        return 'Cancelled';
      case AppointmentStatus.NoShow:
        return 'No Show';
      default:
        return status;
    }
  }

  goBack(): void {
    this.router.navigate(['/doctor/appointments']);
  }
}
