import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatSnackBarModule } from '@angular/material/snack-bar';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';

// TODO: Add shared components, directives, and pipes
// - HeaderComponent
// - FooterComponent
// - LoadingSpinnerComponent
// - ConfirmDialogComponent
// - Custom pipes and directives

@NgModule({
  declarations: [],
  imports: [CommonModule, MatSnackBarModule, MatProgressSpinnerModule],
  exports: [MatSnackBarModule, MatProgressSpinnerModule]
})
export class SharedModule {}
