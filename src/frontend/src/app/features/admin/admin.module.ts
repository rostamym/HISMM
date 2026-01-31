import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { AdminRoutingModule } from './admin-routing.module';
import { BaseChartDirective } from 'ng2-charts';
import { DashboardComponent } from './components/dashboard/dashboard.component';
import { UsersListComponent } from './components/users-list/users-list.component';
import { AppointmentsListComponent } from './components/appointments-list/appointments-list.component';
import { UserDetailModalComponent } from './components/user-detail-modal/user-detail-modal.component';
import { AnalyticsDashboardComponent } from './components/analytics-dashboard/analytics-dashboard.component';
import { AppointmentTrendsChartComponent } from './components/analytics-dashboard/appointment-trends-chart/appointment-trends-chart.component';
import { StatusDistributionChartComponent } from './components/analytics-dashboard/status-distribution-chart/status-distribution-chart.component';
import { SpecialtyDistributionChartComponent } from './components/analytics-dashboard/specialty-distribution-chart/specialty-distribution-chart.component';
import { DoctorPerformanceTableComponent } from './components/analytics-dashboard/doctor-performance-table/doctor-performance-table.component';
import { RevenueChartComponent } from './components/analytics-dashboard/revenue-chart/revenue-chart.component';

@NgModule({
  declarations: [
    DashboardComponent,
    UsersListComponent,
    AppointmentsListComponent,
    UserDetailModalComponent,
    AnalyticsDashboardComponent,
    AppointmentTrendsChartComponent,
    StatusDistributionChartComponent,
    SpecialtyDistributionChartComponent,
    DoctorPerformanceTableComponent,
    RevenueChartComponent
  ],
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    AdminRoutingModule,
    BaseChartDirective
  ]
})
export class AdminModule {}
