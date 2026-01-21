export interface Doctor {
  id: string;
  userId: string;
  licenseNumber: string;
  specialtyId: string;
  specialtyName?: string;
  biography: string;
  yearsOfExperience: number;
  consultationFee: number;
  rating: number;
  // User information (flat structure for backwards compatibility)
  firstName: string;
  lastName: string;
  email: string;
  phoneNumber: string;
  // Nested structures for template binding
  user: {
    firstName: string;
    lastName: string;
    email: string;
    phoneNumber: string;
  };
  specialty: {
    id: string;
    name: string;
  };
}

export interface DoctorSearchParams {
  name?: string;
  searchTerm?: string;
  specialtyId?: string;
  minConsultationFee?: number;
  maxConsultationFee?: number;
  minRating?: number;
  isAvailable?: boolean;
  page?: number;
  pageSize?: number;
  sortBy?: string;
  sortOrder?: string;
}

export interface DoctorAvailability {
  id: string;
  doctorId: string;
  dayOfWeek: number;
  dayOfWeekName: string;
  startTime: string;
  endTime: string;
  startTimeFormatted: string;
  endTimeFormatted: string;
  formattedStartTime: string; // Alias for template binding
  formattedEndTime: string; // Alias for template binding
  slotDurationMinutes: number;
  isActive: boolean;
}

export interface TimeSlot {
  startTime: string;
  endTime: string;
  startTimeFormatted: string;
  endTimeFormatted: string;
  formattedTime: string; // Alias for template binding
  isAvailable: boolean;
  isBooked: boolean; // Opposite of isAvailable for template binding
  displayText: string;
}

export interface SetAvailabilityRequest {
  dayOfWeek: number;
  startTime: string;
  endTime: string;
  slotDurationMinutes: number;
}

export interface PaginatedDoctors {
  items: Doctor[];
  pageNumber: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
  hasPreviousPage: boolean;
  hasNextPage: boolean;
}
