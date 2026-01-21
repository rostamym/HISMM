import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { DoctorService } from '@core/services/doctor.service';
import { Doctor, DoctorSearchParams } from '@core/models/doctor.model';

@Component({
  selector: 'app-doctors-list',
  templateUrl: './doctors-list.component.html',
  styleUrls: ['./doctors-list.component.scss']
})
export class DoctorsListComponent implements OnInit {
  doctors: Doctor[] = [];
  loading = false;
  error: string | null = null;

  // Pagination
  currentPage = 1;
  pageSize = 10;
  totalCount = 0;

  // Search and filters
  searchParams: DoctorSearchParams = {
    page: 1,
    pageSize: 10
  };

  specialties: any[] = [];
  searchName = '';
  selectedSpecialtyId: string | null = null;
  minFee: number | null = null;
  maxFee: number | null = null;
  isAvailableOnly = false;

  constructor(
    private doctorService: DoctorService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.loadDoctors();
  }

  loadDoctors(): void {
    this.loading = true;
    this.error = null;

    this.doctorService.getDoctors(this.currentPage, this.pageSize)
      .subscribe({
        next: (response) => {
          this.doctors = response.items;
          this.totalCount = response.totalCount;
          this.loading = false;
        },
        error: (err) => {
          this.error = 'Failed to load doctors. Please try again.';
          this.loading = false;
          console.error('Error loading doctors:', err);
        }
      });
  }

  searchDoctors(): void {
    this.loading = true;
    this.error = null;

    this.searchParams = {
      name: this.searchName || undefined,
      specialtyId: this.selectedSpecialtyId || undefined,
      minConsultationFee: this.minFee || undefined,
      maxConsultationFee: this.maxFee || undefined,
      isAvailable: this.isAvailableOnly || undefined,
      page: this.currentPage,
      pageSize: this.pageSize
    };

    this.doctorService.searchDoctors(this.searchParams)
      .subscribe({
        next: (response) => {
          this.doctors = response.items;
          this.totalCount = response.totalCount;
          this.loading = false;
        },
        error: (err) => {
          this.error = 'Failed to search doctors. Please try again.';
          this.loading = false;
          console.error('Error searching doctors:', err);
        }
      });
  }

  clearFilters(): void {
    this.searchName = '';
    this.selectedSpecialtyId = null;
    this.minFee = null;
    this.maxFee = null;
    this.isAvailableOnly = false;
    this.currentPage = 1;
    this.loadDoctors();
  }

  viewDoctorDetails(doctorId: string): void {
    this.router.navigate(['/patient/doctors', doctorId]);
  }

  bookAppointment(doctorId: string): void {
    this.router.navigate(['/patient/doctors', doctorId, 'book']);
  }

  onPageChange(page: number): void {
    this.currentPage = page;
    if (this.hasActiveFilters()) {
      this.searchDoctors();
    } else {
      this.loadDoctors();
    }
  }

  hasActiveFilters(): boolean {
    return !!(this.searchName || this.selectedSpecialtyId || this.minFee || this.maxFee || this.isAvailableOnly);
  }

  get totalPages(): number {
    return Math.ceil(this.totalCount / this.pageSize);
  }

  get pages(): number[] {
    const pages = [];
    for (let i = 1; i <= this.totalPages; i++) {
      pages.push(i);
    }
    return pages;
  }
}
