import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AuthGuard } from '@core/guards/auth.guard';
import { RoleGuard } from '@core/guards/role.guard';
import { UserRole } from '@core/models/user.model';
import { DashboardComponent } from './components/dashboard/dashboard.component';
import { DoctorsListComponent } from './components/doctors-list/doctors-list.component';
import { DoctorDetailComponent } from './components/doctor-detail/doctor-detail.component';
import { BookAppointmentComponent } from './components/book-appointment/book-appointment.component';
import { AppointmentsListComponent } from './components/appointments-list/appointments-list.component';
import { AppointmentDetailComponent } from './components/appointment-detail/appointment-detail.component';

const routes: Routes = [
  {
    path: '',
    canActivate: [AuthGuard, RoleGuard],
    data: { roles: [UserRole.Patient] },
    children: [
      {
        path: '',
        redirectTo: 'dashboard',
        pathMatch: 'full'
      },
      {
        path: 'dashboard',
        component: DashboardComponent
      },
      {
        path: 'doctors',
        component: DoctorsListComponent
      },
      {
        path: 'doctors/:id',
        component: DoctorDetailComponent
      },
      {
        path: 'doctors/:id/book',
        component: BookAppointmentComponent
      },
      {
        path: 'appointments',
        component: AppointmentsListComponent
      },
      {
        path: 'appointments/:id',
        component: AppointmentDetailComponent
      }
      // TODO: Add more patient routes
      // - profile (patient profile management)
    ]
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class PatientRoutingModule {}
