import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '@core/services/auth.service';
import { AppointmentService } from '@core/services/appointment.service';
import { Appointment, AppointmentStatus } from '@core/models/appointment.model';

@Component({
  selector: 'app-doctor-appointments-list',
  templateUrl: './appointments-list.component.html',
  styleUrls: ['./appointments-list.component.scss']
})
export class DoctorAppointmentsListComponent implements OnInit {
  appointments: Appointment[] = [];
  filteredAppointments: Appointment[] = [];
  doctorId: string = '';

  loading = true;
  error: string | null = null;

  // Filters
  statusFilter: string = 'all';
  dateFilter: string = 'all'; // all, today, week, month
  searchTerm: string = '';

  AppointmentStatus = AppointmentStatus;

  constructor(
    private authService: AuthService,
    private appointmentService: AppointmentService,
    private router: Router
  ) {}

  ngOnInit(): void {
    const currentUser = this.authService.currentUserValue;
    if (currentUser?.doctorId) {
      this.doctorId = currentUser.doctorId;
      this.loadAppointments();
    } else {
      this.error = 'Doctor information not found';
      this.loading = false;
    }
  }

  loadAppointments(): void {
    this.loading = true;
    this.error = null;

    this.appointmentService.getDoctorAppointments(this.doctorId).subscribe({
      next: (appointments) => {
        this.appointments = appointments.sort((a, b) => {
          const dateA = new Date(a.scheduledDate + 'T' + a.startTime);
          const dateB = new Date(b.scheduledDate + 'T' + b.startTime);
          return dateB.getTime() - dateA.getTime();
        });
        this.applyFilters();
        this.loading = false;
      },
      error: (err) => {
        this.error = 'Failed to load appointments';
        this.loading = false;
        console.error('Error loading appointments:', err);
      }
    });
  }

  applyFilters(): void {
    let filtered = [...this.appointments];

    // Status filter
    if (this.statusFilter !== 'all') {
      filtered = filtered.filter(apt => apt.status.toLowerCase() === this.statusFilter);
    }

    // Date filter
    const now = new Date();
    if (this.dateFilter === 'today') {
      const today = now.toISOString().split('T')[0];
      filtered = filtered.filter(apt =>
        new Date(apt.scheduledDate).toISOString().split('T')[0] === today
      );
    } else if (this.dateFilter === 'week') {
      const weekAgo = new Date(now.getTime() - 7 * 24 * 60 * 60 * 1000);
      const weekAhead = new Date(now.getTime() + 7 * 24 * 60 * 60 * 1000);
      filtered = filtered.filter(apt => {
        const aptDate = new Date(apt.scheduledDate);
        return aptDate >= weekAgo && aptDate <= weekAhead;
      });
    } else if (this.dateFilter === 'month') {
      const monthAgo = new Date(now.getTime() - 30 * 24 * 60 * 60 * 1000);
      const monthAhead = new Date(now.getTime() + 30 * 24 * 60 * 60 * 1000);
      filtered = filtered.filter(apt => {
        const aptDate = new Date(apt.scheduledDate);
        return aptDate >= monthAgo && aptDate <= monthAhead;
      });
    }

    // Search term
    if (this.searchTerm) {
      const term = this.searchTerm.toLowerCase();
      filtered = filtered.filter(apt =>
        apt.patient.firstName.toLowerCase().includes(term) ||
        apt.patient.lastName.toLowerCase().includes(term) ||
        apt.reason.toLowerCase().includes(term)
      );
    }

    this.filteredAppointments = filtered;
  }

  onStatusFilterChange(status: string): void {
    this.statusFilter = status;
    this.applyFilters();
  }

  onDateFilterChange(filter: string): void {
    this.dateFilter = filter;
    this.applyFilters();
  }

  onSearchChange(): void {
    this.applyFilters();
  }

  viewDetails(appointmentId: string): void {
    this.router.navigate(['/doctor/appointments', appointmentId]);
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

  get totalAppointments(): number {
    return this.appointments.length;
  }

  get filteredCount(): number {
    return this.filteredAppointments.length;
  }
}
