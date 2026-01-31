import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { AdminService } from '@core/services/admin.service';
import { UserListItem } from '@core/models/admin.model';
import { Appointment } from '@core/models/appointment.model';

@Component({
  selector: 'app-user-detail-modal',
  templateUrl: './user-detail-modal.component.html',
  styleUrls: ['./user-detail-modal.component.scss']
})
export class UserDetailModalComponent implements OnInit {
  @Input() user!: UserListItem;
  @Output() close = new EventEmitter<void>();

  appointments: Appointment[] = [];
  loadingAppointments: boolean = false;
  appointmentsError: string | null = null;

  constructor(private adminService: AdminService) {}

  ngOnInit(): void {
    this.loadAppointmentHistory();
  }

  /**
   * Load user's appointment history
   */
  loadAppointmentHistory(): void {
    this.loadingAppointments = true;
    this.appointmentsError = null;

    this.adminService.getUserAppointmentHistory(this.user.id).subscribe({
      next: (appointments) => {
        this.appointments = appointments;
        this.loadingAppointments = false;
      },
      error: (err) => {
        this.appointmentsError = err.error?.Error || 'Failed to load appointment history';
        this.loadingAppointments = false;
        console.error('Error loading appointment history:', err);
      }
    });
  }

  /**
   * Close the modal
   */
  onClose(): void {
    this.close.emit();
  }

  /**
   * Get status badge class
   */
  getStatusClass(status: string): string {
    const statusMap: { [key: string]: string } = {
      'Scheduled': 'status-scheduled',
      'Confirmed': 'status-confirmed',
      'InProgress': 'status-in-progress',
      'Completed': 'status-completed',
      'Cancelled': 'status-cancelled',
      'NoShow': 'status-no-show'
    };
    return statusMap[status] || 'status-default';
  }

  /**
   * Get user-friendly status display text
   */
  getStatusDisplay(status: string): string {
    const statusDisplayMap: { [key: string]: string } = {
      'Scheduled': 'Scheduled',
      'Confirmed': 'Confirmed',
      'InProgress': 'In Progress',
      'Completed': 'Completed',
      'Cancelled': 'Cancelled',
      'NoShow': 'No Show'
    };
    return statusDisplayMap[status] || status;
  }

  /**
   * Format date for display
   */
  formatDate(dateString: string): string {
    const date = new Date(dateString);
    return date.toLocaleDateString('en-US', {
      year: 'numeric',
      month: 'short',
      day: 'numeric'
    });
  }

  /**
   * Get role badge class
   */
  getRoleClass(role: string): string {
    const roleMap: { [key: string]: string } = {
      'Patient': 'role-patient',
      'Doctor': 'role-doctor',
      'Administrator': 'role-admin'
    };
    return roleMap[role] || 'role-default';
  }
}
