import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { AdminService } from '@core/services/admin.service';
import { Appointment, AppointmentStatus } from '@core/models/appointment.model';
import { AppointmentFilters } from '@core/models/admin.model';

@Component({
  selector: 'app-appointments-list',
  templateUrl: './appointments-list.component.html',
  styleUrls: ['./appointments-list.component.scss']
})
export class AppointmentsListComponent implements OnInit {
  appointments: Appointment[] = [];
  filteredAppointments: Appointment[] = [];

  // Filter properties
  searchTerm: string = '';
  statusFilter: string = 'all';
  dateRangeFilter: string = 'all';
  fromDate: string = '';
  toDate: string = '';

  // State properties
  loading: boolean = false;
  error: string | null = null;

  constructor(
    private adminService: AdminService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.loadAppointments();
  }

  /**
   * Load appointments from the API
   */
  loadAppointments(): void {
    this.loading = true;
    this.error = null;

    const filters = this.buildFilters();

    this.adminService.getAllAppointments(filters).subscribe({
      next: (appointments) => {
        this.appointments = appointments;
        this.filteredAppointments = appointments;
        this.loading = false;
      },
      error: (err) => {
        this.error = err.error?.Error || 'Failed to load appointments';
        this.loading = false;
        console.error('Error loading appointments:', err);
      }
    });
  }

  /**
   * Build filters object from current filter states
   */
  private buildFilters(): AppointmentFilters | undefined {
    const filters: AppointmentFilters = {};

    if (this.statusFilter && this.statusFilter !== 'all') {
      filters.status = this.statusFilter;
    }

    if (this.searchTerm) {
      filters.searchTerm = this.searchTerm;
    }

    // Handle date range filters
    if (this.dateRangeFilter !== 'all') {
      const today = new Date();
      today.setHours(0, 0, 0, 0);

      if (this.dateRangeFilter === 'today') {
        filters.fromDate = this.formatDateForApi(today);
        filters.toDate = this.formatDateForApi(today);
      } else if (this.dateRangeFilter === 'week') {
        const weekFromNow = new Date(today);
        weekFromNow.setDate(today.getDate() + 7);
        filters.fromDate = this.formatDateForApi(today);
        filters.toDate = this.formatDateForApi(weekFromNow);
      } else if (this.dateRangeFilter === 'month') {
        const monthFromNow = new Date(today);
        monthFromNow.setMonth(today.getMonth() + 1);
        filters.fromDate = this.formatDateForApi(today);
        filters.toDate = this.formatDateForApi(monthFromNow);
      } else if (this.dateRangeFilter === 'custom') {
        if (this.fromDate) {
          filters.fromDate = this.fromDate;
        }
        if (this.toDate) {
          filters.toDate = this.toDate;
        }
      }
    }

    return Object.keys(filters).length > 0 ? filters : undefined;
  }

  /**
   * Format date for API (YYYY-MM-DD)
   */
  private formatDateForApi(date: Date): string {
    return date.toISOString().split('T')[0];
  }

  /**
   * Apply filters and reload appointments
   */
  applyFilters(): void {
    this.loadAppointments();
  }

  /**
   * Handle search input change
   */
  onSearchChange(): void {
    this.applyFilters();
  }

  /**
   * Handle status filter change
   */
  onStatusFilterChange(): void {
    this.applyFilters();
  }

  /**
   * Handle date range filter change
   */
  onDateRangeFilterChange(): void {
    // Clear custom dates when switching away from custom
    if (this.dateRangeFilter !== 'custom') {
      this.fromDate = '';
      this.toDate = '';
    }
    this.applyFilters();
  }

  /**
   * Handle custom date change
   */
  onCustomDateChange(): void {
    if (this.dateRangeFilter === 'custom') {
      this.applyFilters();
    }
  }

  /**
   * Clear all filters
   */
  clearFilters(): void {
    this.searchTerm = '';
    this.statusFilter = 'all';
    this.dateRangeFilter = 'all';
    this.fromDate = '';
    this.toDate = '';
    this.applyFilters();
  }

  /**
   * Get CSS class for appointment status
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
      weekday: 'short',
      year: 'numeric',
      month: 'short',
      day: 'numeric'
    });
  }

  /**
   * Navigate to appointment details
   */
  viewAppointmentDetails(id: string): void {
    // For now, just log - can be implemented later
    console.log('View appointment details:', id);
  }

  /**
   * Retry loading after error
   */
  retry(): void {
    this.loadAppointments();
  }

  // Computed properties
  get totalCount(): number {
    return this.appointments.length;
  }

  get filteredCount(): number {
    return this.filteredAppointments.length;
  }

  get scheduledCount(): number {
    return this.appointments.filter(a => a.status === AppointmentStatus.Scheduled).length;
  }

  get confirmedCount(): number {
    return this.appointments.filter(a => a.status === AppointmentStatus.Confirmed).length;
  }

  get completedCount(): number {
    return this.appointments.filter(a => a.status === AppointmentStatus.Completed).length;
  }

  get cancelledCount(): number {
    return this.appointments.filter(a => a.status === AppointmentStatus.Cancelled).length;
  }

  get showCustomDateInputs(): boolean {
    return this.dateRangeFilter === 'custom';
  }
}
