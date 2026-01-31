import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { AdminRoutingModule } from './admin-routing.module';
import { DashboardComponent } from './components/dashboard/dashboard.component';
import { UsersListComponent } from './components/users-list/users-list.component';
import { AppointmentsListComponent } from './components/appointments-list/appointments-list.component';
import { UserDetailModalComponent } from './components/user-detail-modal/user-detail-modal.component';

@NgModule({
  declarations: [
    DashboardComponent,
    UsersListComponent,
    AppointmentsListComponent,
    UserDetailModalComponent
  ],
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    AdminRoutingModule
  ]
})
export class AdminModule {}
