import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { AdminService } from '@core/services/admin.service';
import { SystemStatistics } from '@core/models/admin.model';

@Component({
  selector: 'app-admin-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.scss']
})
export class DashboardComponent implements OnInit {
  stats: SystemStatistics = {
    totalUsers: 0,
    totalPatients: 0,
    totalDoctors: 0,
    totalAdmins: 0,
    totalAppointments: 0,
    todayAppointments: 0,
    pendingAppointments: 0,
    completedAppointments: 0,
    cancelledAppointments: 0
  };

  loading = true;
  error: string | null = null;

  // Mock data for now - will be replaced with actual API calls
  recentActivity: any[] = [];

  constructor(
    private adminService: AdminService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.loadDashboardData();
  }

  loadDashboardData(): void {
    this.loading = true;
    this.error = null;

    this.adminService.getSystemStatistics().subscribe({
      next: (statistics) => {
        this.stats = statistics;

        // Mock recent activity until we have a real endpoint
        this.recentActivity = [
          { type: 'user_registered', user: 'John Doe', role: 'Patient', time: '2 hours ago' },
          { type: 'appointment_booked', patient: 'Jane Smith', doctor: 'Dr. Brown', time: '3 hours ago' },
          { type: 'appointment_completed', patient: 'Mike Johnson', doctor: 'Dr. Wilson', time: '5 hours ago' },
          { type: 'user_registered', user: 'Sarah Williams', role: 'Patient', time: '6 hours ago' },
          { type: 'appointment_cancelled', patient: 'Tom Davis', doctor: 'Dr. Smith', time: '8 hours ago' }
        ];

        this.loading = false;
      },
      error: (err) => {
        console.error('Error loading dashboard data:', err);
        this.error = 'Failed to load dashboard data. Please try again.';
        this.loading = false;
      }
    });
  }

  getActivityIcon(type: string): string {
    switch (type) {
      case 'user_registered':
        return '👤';
      case 'appointment_booked':
        return '📅';
      case 'appointment_completed':
        return '✅';
      case 'appointment_cancelled':
        return '❌';
      default:
        return '📌';
    }
  }

  getActivityClass(type: string): string {
    switch (type) {
      case 'user_registered':
        return 'activity-registered';
      case 'appointment_booked':
        return 'activity-booked';
      case 'appointment_completed':
        return 'activity-completed';
      case 'appointment_cancelled':
        return 'activity-cancelled';
      default:
        return '';
    }
  }

  navigateToAppointments(): void {
    this.router.navigate(['/admin/appointments']);
  }
}
