import { Component, OnInit, ViewChild } from '@angular/core';
import { AnalyticsService, AppointmentTrend } from '../../../../../core/services/analytics.service';
import { ChartConfiguration, ChartType } from 'chart.js';
import { BaseChartDirective } from 'ng2-charts';

@Component({
  selector: 'app-appointment-trends-chart',
  templateUrl: './appointment-trends-chart.component.html',
  styleUrls: ['./appointment-trends-chart.component.scss']
})
export class AppointmentTrendsChartComponent implements OnInit {
  @ViewChild(BaseChartDirective) chart?: BaseChartDirective;

  public lineChartData: ChartConfiguration['data'] = {
    datasets: [],
    labels: []
  };

  public lineChartOptions: ChartConfiguration['options'] = {
    responsive: true,
    maintainAspectRatio: false,
    plugins: {
      legend: {
        display: true,
        position: 'bottom'
      },
      title: {
        display: true,
        text: 'Appointment Trends',
        font: {
          size: 18
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

  public lineChartType: ChartType = 'line';

  selectedPeriod: 'daily' | 'weekly' | 'monthly' = 'daily';
  loading = false;
  error: string | null = null;

  constructor(private analyticsService: AnalyticsService) { }

  ngOnInit(): void {
    this.loadTrendsData();
  }

  onPeriodChange(period: 'daily' | 'weekly' | 'monthly'): void {
    this.selectedPeriod = period;
    this.loadTrendsData();
  }

  loadTrendsData(): void {
    this.loading = true;
    this.error = null;

    this.analyticsService.getAppointmentTrends(this.selectedPeriod)
      .subscribe({
        next: (trends: AppointmentTrend[]) => {
          this.updateChartData(trends);
          this.loading = false;
        },
        error: (err) => {
          console.error('Error loading trends data:', err);
          this.error = 'Failed to load trends data';
          this.loading = false;
        }
      });
  }

  private updateChartData(trends: AppointmentTrend[]): void {
    const labels = trends.map(t => t.label);
    const completed = trends.map(t => t.completedAppointments);
    const cancelled = trends.map(t => t.cancelledAppointments);
    const noShow = trends.map(t => t.noShowAppointments);
    const scheduled = trends.map(t => t.scheduledAppointments);

    this.lineChartData = {
      labels: labels,
      datasets: [
        {
          data: completed,
          label: 'Completed',
          borderColor: '#28a745',
          backgroundColor: 'rgba(40, 167, 69, 0.1)',
          tension: 0.4,
          fill: true
        },
        {
          data: scheduled,
          label: 'Scheduled',
          borderColor: '#007bff',
          backgroundColor: 'rgba(0, 123, 255, 0.1)',
          tension: 0.4,
          fill: true
        },
        {
          data: cancelled,
          label: 'Cancelled',
          borderColor: '#dc3545',
          backgroundColor: 'rgba(220, 53, 69, 0.1)',
          tension: 0.4,
          fill: true
        },
        {
          data: noShow,
          label: 'No-Show',
          borderColor: '#ffc107',
          backgroundColor: 'rgba(255, 193, 7, 0.1)',
          tension: 0.4,
          fill: true
        }
      ]
    };

    this.chart?.update();
  }
}
