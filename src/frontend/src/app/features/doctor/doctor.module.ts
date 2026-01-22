import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { DoctorRoutingModule } from './doctor-routing.module';
import { DashboardComponent } from './components/dashboard/dashboard.component';
import { DoctorAppointmentsListComponent } from './components/appointments-list/appointments-list.component';
import { DoctorAppointmentDetailComponent } from './components/appointment-detail/appointment-detail.component';

@NgModule({
  declarations: [
    DashboardComponent,
    DoctorAppointmentsListComponent,
    DoctorAppointmentDetailComponent
  ],
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    DoctorRoutingModule
  ]
})
export class DoctorModule {}
