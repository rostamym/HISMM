import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { AppointmentService } from '@core/services/appointment.service';
import { AuthService } from '@core/services/auth.service';
import { Appointment, AppointmentStatus } from '@core/models/appointment.model';

@Component({
  selector: 'app-appointments-list',
  templateUrl: './appointments-list.component.html',
  styleUrls: ['./appointments-list.component.scss']
})
export class AppointmentsListComponent implements OnInit {
  appointments: Appointment[] = [];
  filteredAppointments: Appointment[] = [];
  loading = false;
  error: string | null = null;

  selectedFilter: 'all' | 'upcoming' | 'past' | 'cancelled' = 'upcoming';

  constructor(
    private appointmentService: AppointmentService,
    private authService: AuthService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.loadAppointments();
  }

  loadAppointments(): void {
    const currentUser = this.authService.currentUserValue;
    if (!currentUser?.patientId) {
      this.error = 'Patient information not available.';
      return;
    }

    this.loading = true;
    this.error = null;

    this.appointmentService.getPatientAppointments(currentUser.patientId).subscribe({
      next: (appointments) => {
        this.appointments = appointments;
        this.applyFilter(this.selectedFilter);
        this.loading = false;
      },
      error: (err) => {
        this.error = 'Failed to load appointments. Please try again.';
        this.loading = false;
        console.error('Error loading appointments:', err);
      }
    });
  }

  applyFilter(filter: 'all' | 'upcoming' | 'past' | 'cancelled'): void {
    this.selectedFilter = filter;
    const now = new Date();

    switch (filter) {
      case 'all':
        this.filteredAppointments = this.appointments;
        break;

      case 'upcoming':
        this.filteredAppointments = this.appointments.filter(apt => {
          const aptDate = new Date(apt.scheduledDate);
          return aptDate >= now &&
                 apt.status !== AppointmentStatus.Cancelled &&
                 apt.status !== AppointmentStatus.Completed;
        });
        break;

      case 'past':
        this.filteredAppointments = this.appointments.filter(apt => {
          const aptDate = new Date(apt.scheduledDate);
          return aptDate < now || apt.status === AppointmentStatus.Completed;
        });
        break;

      case 'cancelled':
        this.filteredAppointments = this.appointments.filter(
          apt => apt.status === AppointmentStatus.Cancelled
        );
        break;
    }

    // Sort by date descending (most recent first)
    this.filteredAppointments.sort((a, b) => {
      return new Date(b.scheduledDate).getTime() - new Date(a.scheduledDate).getTime();
    });
  }

  viewAppointmentDetails(appointmentId: string): void {
    this.router.navigate(['/patient/appointments', appointmentId]);
  }

  getStatusClass(status: string): string {
    switch (status) {
      case AppointmentStatus.Scheduled:
      case AppointmentStatus.Confirmed:
        return 'status-scheduled';
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

  getStatusDisplay(status: string): string {
    switch (status) {
      case AppointmentStatus.InProgress:
        return 'In Progress';
      case AppointmentStatus.NoShow:
        return 'No Show';
      default:
        return status;
    }
  }

  formatDate(dateString: string): string {
    const date = new Date(dateString);
    return date.toLocaleDateString('en-US', {
      weekday: 'short',
      year: 'numeric',
      month: 'short',
      day: 'numeric'
    });
  }

  canCancelAppointment(appointment: Appointment): boolean {
    return this.appointmentService.canCancelAppointment(appointment);
  }

  canRescheduleAppointment(appointment: Appointment): boolean {
    return this.appointmentService.canRescheduleAppointment(appointment);
  }

  get upcomingCount(): number {
    const now = new Date();
    return this.appointments.filter(apt => {
      const aptDate = new Date(apt.scheduledDate);
      return aptDate >= now &&
             apt.status !== AppointmentStatus.Cancelled &&
             apt.status !== AppointmentStatus.Completed;
    }).length;
  }

  get pastCount(): number {
    const now = new Date();
    return this.appointments.filter(apt => {
      const aptDate = new Date(apt.scheduledDate);
      return aptDate < now || apt.status === AppointmentStatus.Completed;
    }).length;
  }

  get cancelledCount(): number {
    return this.appointments.filter(apt => apt.status === AppointmentStatus.Cancelled).length;
  }
}
