import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { PatientRoutingModule } from './patient-routing.module';

// Components
import { DashboardComponent } from './components/dashboard/dashboard.component';
import { DoctorsListComponent } from './components/doctors-list/doctors-list.component';
import { DoctorDetailComponent } from './components/doctor-detail/doctor-detail.component';
import { BookAppointmentComponent } from './components/book-appointment/book-appointment.component';
import { AppointmentsListComponent } from './components/appointments-list/appointments-list.component';
import { AppointmentDetailComponent } from './components/appointment-detail/appointment-detail.component';

@NgModule({
  declarations: [
    DashboardComponent,
    DoctorsListComponent,
    DoctorDetailComponent,
    BookAppointmentComponent,
    AppointmentsListComponent,
    AppointmentDetailComponent
  ],
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    PatientRoutingModule
  ]
})
export class PatientModule {}
