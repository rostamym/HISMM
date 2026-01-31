import { Component, OnInit } from '@angular/core';
import { AdminService } from '@core/services/admin.service';
import { UserListItem } from '@core/models/admin.model';

@Component({
  selector: 'app-users-list',
  templateUrl: './users-list.component.html',
  styleUrls: ['./users-list.component.scss']
})
export class UsersListComponent implements OnInit {
  users: UserListItem[] = [];
  filteredUsers: UserListItem[] = [];

  loading = true;
  error: string | null = null;

  // Filters
  searchTerm = '';
  roleFilter = 'all'; // all, patient, doctor, administrator
  statusFilter = 'all'; // all, active, inactive

  constructor(private adminService: AdminService) {}

  ngOnInit(): void {
    this.loadUsers();
  }

  loadUsers(): void {
    this.loading = true;
    this.error = null;

    this.adminService.getAllUsers().subscribe({
      next: (users) => {
        this.users = users;
        this.applyFilters();
        this.loading = false;
      },
      error: (err) => {
        console.error('Error loading users:', err);
        this.error = 'Failed to load users. Please try again.';
        this.loading = false;
      }
    });
  }

  applyFilters(): void {
    let filtered = [...this.users];

    // Search filter
    if (this.searchTerm) {
      const term = this.searchTerm.toLowerCase();
      filtered = filtered.filter(user =>
        user.email.toLowerCase().includes(term) ||
        user.firstName.toLowerCase().includes(term) ||
        user.lastName.toLowerCase().includes(term)
      );
    }

    // Role filter
    if (this.roleFilter !== 'all') {
      filtered = filtered.filter(user =>
        user.role.toLowerCase() === this.roleFilter.toLowerCase()
      );
    }

    // Status filter
    if (this.statusFilter === 'active') {
      filtered = filtered.filter(user => user.isActive);
    } else if (this.statusFilter === 'inactive') {
      filtered = filtered.filter(user => !user.isActive);
    }

    this.filteredUsers = filtered;
  }

  onSearchChange(): void {
    this.applyFilters();
  }

  onRoleFilterChange(role: string): void {
    this.roleFilter = role;
    this.applyFilters();
  }

  onStatusFilterChange(status: string): void {
    this.statusFilter = status;
    this.applyFilters();
  }

  toggleUserStatus(userId: string): void {
    const user = this.users.find(u => u.id === userId);
    if (!user) return;

    const newStatus = !user.isActive;

    this.adminService.updateUserStatus(userId, newStatus).subscribe({
      next: () => {
        user.isActive = newStatus;
        this.applyFilters();
      },
      error: (err) => {
        console.error('Error updating user status:', err);
        alert('Failed to update user status. Please try again.');
      }
    });
  }

  getRoleBadgeClass(role: string): string {
    switch (role.toLowerCase()) {
      case 'patient':
        return 'badge-patient';
      case 'doctor':
        return 'badge-doctor';
      case 'admin':
        return 'badge-admin';
      default:
        return '';
    }
  }

  getStatusBadgeClass(isActive: boolean): string {
    return isActive ? 'badge-active' : 'badge-inactive';
  }

  // Modal properties
  selectedUser: UserListItem | null = null;
  showModal: boolean = false;

  /**
   * Open user detail modal
   */
  viewUserDetails(user: UserListItem): void {
    this.selectedUser = user;
    this.showModal = true;
  }

  /**
   * Close user detail modal
   */
  closeModal(): void {
    this.showModal = false;
    this.selectedUser = null;
  }
}
