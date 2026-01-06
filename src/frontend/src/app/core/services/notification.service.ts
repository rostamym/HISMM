import { Injectable } from '@angular/core';
import { MatSnackBar, MatSnackBarConfig } from '@angular/material/snack-bar';

@Injectable({
  providedIn: 'root'
})
export class NotificationService {
  private defaultConfig: MatSnackBarConfig = {
    duration: 3000,
    horizontalPosition: 'right',
    verticalPosition: 'top'
  };

  constructor(private snackBar: MatSnackBar) {}

  success(message: string, duration?: number): void {
    this.snackBar.open(message, 'Close', {
      ...this.defaultConfig,
      duration: duration || this.defaultConfig.duration,
      panelClass: ['snackbar-success']
    });
  }

  error(message: string, duration?: number): void {
    this.snackBar.open(message, 'Close', {
      ...this.defaultConfig,
      duration: duration || this.defaultConfig.duration,
      panelClass: ['snackbar-error']
    });
  }

  warning(message: string, duration?: number): void {
    this.snackBar.open(message, 'Close', {
      ...this.defaultConfig,
      duration: duration || this.defaultConfig.duration,
      panelClass: ['snackbar-warning']
    });
  }

  info(message: string, duration?: number): void {
    this.snackBar.open(message, 'Close', {
      ...this.defaultConfig,
      duration: duration || this.defaultConfig.duration,
      panelClass: ['snackbar-info']
    });
  }
}
