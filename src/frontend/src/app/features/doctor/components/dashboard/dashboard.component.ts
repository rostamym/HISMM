import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '@core/services/auth.service';
import { AppointmentService } from '@core/services/appointment.service';
import { User } from '@core/models/user.model';
import { Appointment, AppointmentStatus } from '@core/models/appointment.model';

interface DashboardStats {
  todayAppointments: number;
  upcomingAppointments: number;
  completedToday: number;
  totalPatients: number;
}

@Component({
  selector: 'app-doctor-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.scss']
})
export class DashboardComponent implements OnInit {
  currentUser: User | null = null;
  doctorId: string = '';

  stats: DashboardStats = {
    todayAppointments: 0,
    upcomingAppointments: 0,
    completedToday: 0,
    totalPatients: 0
  };

  todayAppointments: Appointment[] = [];
  loading = true;
  error: string | null = null;

  AppointmentStatus = AppointmentStatus;

  constructor(
    private authService: AuthService,
    private appointmentService: AppointmentService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.currentUser = this.authService.currentUserValue;

    if (this.currentUser?.doctorId) {
      this.doctorId = this.currentUser.doctorId;
      this.loadDashboardData();
    } else {
      this.error = 'Doctor information not found. Please contact support.';
      this.loading = false;
    }
  }

  loadDashboardData(): void {
    this.loading = true;
    this.error = null;

    const today = new Date();
    const startOfDay = new Date(today.setHours(0, 0, 0, 0));
    const endOfDay = new Date(today.setHours(23, 59, 59, 999));

    // Load today's appointments
    this.appointmentService.getDoctorAppointments(
      this.doctorId,
      startOfDay,
      endOfDay
    ).subscribe({
      next: (appointments) => {
        this.todayAppointments = appointments.sort((a, b) =>
          a.startTime.localeCompare(b.startTime)
        );
        this.calculateStats(appointments);
        this.loading = false;
      },
      error: (err) => {
        this.error = 'Failed to load dashboard data.';
        this.loading = false;
        console.error('Error loading dashboard:', err);
      }
    });

    // Load all appointments for stats
    this.appointmentService.getDoctorAppointments(this.doctorId).subscribe({
      next: (allAppointments) => {
        this.calculateAdditionalStats(allAppointments);
      },
      error: (err) => {
        console.error('Error loading all appointments:', err);
      }
    });
  }

  calculateStats(todayAppointments: Appointment[]): void {
    const now = new Date();
    const todayStr = now.toISOString().split('T')[0];

    this.stats.todayAppointments = todayAppointments.length;

    this.stats.completedToday = todayAppointments.filter(apt =>
      apt.status === AppointmentStatus.Completed
    ).length;

    this.stats.upcomingAppointments = todayAppointments.filter(apt => {
      const aptDate = new Date(apt.scheduledDate);
      return aptDate.toISOString().split('T')[0] === todayStr &&
             (apt.status === AppointmentStatus.Scheduled ||
              apt.status === AppointmentStatus.Confirmed);
    }).length;
  }

  calculateAdditionalStats(allAppointments: Appointment[]): void {
    // Count unique patients
    const uniquePatients = new Set(
      allAppointments
        .filter(apt => apt.status !== AppointmentStatus.Cancelled)
        .map(apt => apt.patient.id)
    );
    this.stats.totalPatients = uniquePatients.size;
  }

  viewAllAppointments(): void {
    this.router.navigate(['/doctor/appointments']);
  }

  viewAppointmentDetails(appointmentId: string): void {
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

  getTimeUntilAppointment(appointment: Appointment): string {
    const now = new Date();
    const aptDateTime = new Date(`${appointment.scheduledDate}T${appointment.startTime}`);
    const diffMs = aptDateTime.getTime() - now.getTime();
    const diffMins = Math.floor(diffMs / 60000);

    if (diffMins < 0) {
      return 'In progress or passed';
    } else if (diffMins < 60) {
      return `In ${diffMins} minutes`;
    } else {
      const hours = Math.floor(diffMins / 60);
      return `In ${hours} hour${hours > 1 ? 's' : ''}`;
    }
  }

  logout(): void {
    this.authService.logout();
  }
}
