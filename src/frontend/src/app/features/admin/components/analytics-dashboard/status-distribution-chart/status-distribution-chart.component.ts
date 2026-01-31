import { Component, OnInit, ViewChild } from '@angular/core';
import { AnalyticsService, AppointmentStatusDistribution } from '../../../../../core/services/analytics.service';
import { ChartConfiguration, ChartType } from 'chart.js';
import { BaseChartDirective } from 'ng2-charts';

@Component({
  selector: 'app-status-distribution-chart',
  templateUrl: './status-distribution-chart.component.html',
  styleUrls: ['./status-distribution-chart.component.scss']
})
export class StatusDistributionChartComponent implements OnInit {
  @ViewChild(BaseChartDirective) chart?: BaseChartDirective;

  public pieChartData: ChartConfiguration['data'] = {
    datasets: [{
      data: []
    }],
    labels: []
  };

  public pieChartOptions: ChartConfiguration['options'] = {
    responsive: true,
    maintainAspectRatio: false,
    plugins: {
      legend: {
        display: true,
        position: 'bottom'
      },
      title: {
        display: true,
        text: 'Appointment Status Distribution',
        font: {
          size: 18
        }
      },
      tooltip: {
        callbacks: {
          label: (context) => {
            const label = context.label || '';
            const value = context.parsed || 0;
            const dataset = context.dataset.data as number[];
            const total = dataset.reduce((acc: number, val: number) => acc + val, 0);
            const percentage = total > 0 ? ((value / total) * 100).toFixed(1) : '0';
            return `${label}: ${value} (${percentage}%)`;
          }
        }
      }
    }
  };

  public pieChartType: ChartType = 'pie';

  loading = false;
  error: string | null = null;

  // Status colors
  private statusColors: { [key: string]: string } = {
    'Scheduled': '#007bff',
    'Confirmed': '#17a2b8',
    'InProgress': '#ffc107',
    'Completed': '#28a745',
    'Cancelled': '#dc3545',
    'NoShow': '#fd7e14'
  };

  constructor(private analyticsService: AnalyticsService) { }

  ngOnInit(): void {
    this.loadStatusData();
  }

  loadStatusData(): void {
    this.loading = true;
    this.error = null;

    this.analyticsService.getAppointmentsByStatus()
      .subscribe({
        next: (statusData: AppointmentStatusDistribution[]) => {
          this.updateChartData(statusData);
          this.loading = false;
        },
        error: (err) => {
          console.error('Error loading status data:', err);
          this.error = 'Failed to load status data';
          this.loading = false;
        }
      });
  }

  private updateChartData(statusData: AppointmentStatusDistribution[]): void {
    const labels = statusData.map(s => s.status);
    const data = statusData.map(s => s.count);
    const colors = statusData.map(s => this.statusColors[s.status] || '#6c757d');

    this.pieChartData = {
      labels: labels,
      datasets: [{
        data: data,
        backgroundColor: colors,
        borderWidth: 2,
        borderColor: '#fff'
      }]
    };

    this.chart?.update();
  }
}
