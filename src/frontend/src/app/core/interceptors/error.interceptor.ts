import { Injectable } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor,
  HttpErrorResponse
} from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { Router } from '@angular/router';
import { NotificationService } from '../services/notification.service';
import { AuthService } from '../services/auth.service';

@Injectable()
export class ErrorInterceptor implements HttpInterceptor {
  constructor(
    private router: Router,
    private notificationService: NotificationService,
    private authService: AuthService
  ) {}

  intercept(
    request: HttpRequest<unknown>,
    next: HttpHandler
  ): Observable<HttpEvent<unknown>> {
    return next.handle(request).pipe(
      catchError((error: HttpErrorResponse) => {
        let errorMessage = 'An error occurred';

        if (error.error instanceof ErrorEvent) {
          // Client-side error
          errorMessage = `Error: ${error.error.message}`;
        } else {
          // Server-side error
          switch (error.status) {
            case 400:
              errorMessage =
                error.error?.message || 'Bad request. Please check your input.';
              break;
            case 401:
              errorMessage = 'Unauthorized. Please login again.';
              this.authService.logout();
              break;
            case 403:
              errorMessage = 'Forbidden. You do not have permission.';
              this.router.navigate(['/unauthorized']);
              break;
            case 404:
              errorMessage = error.error?.message || 'Resource not found.';
              break;
            case 500:
              errorMessage =
                'Internal server error. Please try again later.';
              break;
            default:
              errorMessage =
                error.error?.message ||
                `Error ${error.status}: ${error.statusText}`;
          }
        }

        this.notificationService.error(errorMessage);
        return throwError(() => error);
      })
    );
  }
}
