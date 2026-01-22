import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AuthGuard } from '@core/guards/auth.guard';
import { RoleGuard } from '@core/guards/role.guard';
import { UserRole } from '@core/models/user.model';
import { DashboardComponent } from './components/dashboard/dashboard.component';
import { DoctorAppointmentsListComponent } from './components/appointments-list/appointments-list.component';
import { DoctorAppointmentDetailComponent } from './components/appointment-detail/appointment-detail.component';

const routes: Routes = [
  {
    path: '',
    canActivate: [AuthGuard, RoleGuard],
    data: { roles: [UserRole.Doctor] },
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
        path: 'appointments',
        component: DoctorAppointmentsListComponent
      },
      {
        path: 'appointments/:id',
        component: DoctorAppointmentDetailComponent
      }
      // TODO: Add more doctor routes
      // - schedule
      // - availability
      // - profile
    ]
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class DoctorRoutingModule {}
