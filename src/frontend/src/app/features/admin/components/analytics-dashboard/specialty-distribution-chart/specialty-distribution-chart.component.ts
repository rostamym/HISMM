import { Component, OnInit, ViewChild } from '@angular/core';
import { AnalyticsService, AppointmentsBySpecialty } from '../../../../../core/services/analytics.service';
import { ChartConfiguration, ChartType } from 'chart.js';
import { BaseChartDirective } from 'ng2-charts';

@Component({
  selector: 'app-specialty-distribution-chart',
  templateUrl: './specialty-distribution-chart.component.html',
  styleUrls: ['./specialty-distribution-chart.component.scss']
})
export class SpecialtyDistributionChartComponent implements OnInit {
  @ViewChild(BaseChartDirective) chart?: BaseChartDirective;

  public barChartData: ChartConfiguration['data'] = {
    datasets: [],
    labels: []
  };

  public barChartOptions: ChartConfiguration['options'] = {
    responsive: true,
    maintainAspectRatio: false,
    plugins: {
      legend: {
        display: true,
        position: 'bottom'
      },
      title: {
        display: true,
        text: 'Appointments by Specialty',
        font: {
          size: 18
        }
      },
      tooltip: {
        callbacks: {
          label: (context) => {
            const label = context.dataset.label || '';
            const value = context.parsed.y || 0;
            return `${label}: ${value}`;
          }
        }
      }
    },
    scales: {
      y: {
        beginAtZero: true,
        ticks: {
          stepSize: 1
        }
      }
    }
  };

  public barChartType: ChartType = 'bar';

  loading = false;
  error: string | null = null;

  constructor(private analyticsService: AnalyticsService) { }

  ngOnInit(): void {
    this.loadSpecialtyData();
  }

  loadSpecialtyData(): void {
    this.loading = true;
    this.error = null;

    this.analyticsService.getAppointmentsBySpecialty()
      .subscribe({
        next: (specialtyData: AppointmentsBySpecialty[]) => {
          this.updateChartData(specialtyData);
          this.loading = false;
        },
        error: (err) => {
          console.error('Error loading specialty data:', err);
          this.error = 'Failed to load specialty data';
          this.loading = false;
        }
      });
  }

  private updateChartData(specialtyData: AppointmentsBySpecialty[]): void {
    const labels = specialtyData.map(s => s.specialtyName);
    const totalAppointments = specialtyData.map(s => s.totalAppointments);
    const completedAppointments = specialtyData.map(s => s.completedAppointments);

    this.barChartData = {
      labels: labels,
      datasets: [
        {
          data: totalAppointments,
          label: 'Total Appointments',
          backgroundColor: 'rgba(0, 123, 255, 0.7)',
          borderColor: '#007bff',
          borderWidth: 1
        },
        {
          data: completedAppointments,
          label: 'Completed',
          backgroundColor: 'rgba(40, 167, 69, 0.7)',
          borderColor: '#28a745',
          borderWidth: 1
        }
      ]
    };

    this.chart?.update();
  }
}
