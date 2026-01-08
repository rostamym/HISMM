import { Injectable } from '@angular/core';
import {
  HttpInterceptor,
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpResponse,
  HttpErrorResponse
} from '@angular/common/http';
import { Observable, tap } from 'rxjs';

@Injectable()
export class LoggingInterceptor implements HttpInterceptor {
  intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    const startTime = Date.now();

    // Log the outgoing request
    console.group(`🚀 HTTP Request: ${request.method} ${request.url}`);
    console.log('Headers:', request.headers.keys().map(key => `${key}: ${request.headers.get(key)}`));
    console.log('Body:', request.body);
    console.groupEnd();

    return next.handle(request).pipe(
      tap({
        next: (event) => {
          if (event instanceof HttpResponse) {
            const elapsed = Date.now() - startTime;

            // Log successful response
            console.group(`✅ HTTP Response: ${request.method} ${request.url} (${elapsed}ms)`);
            console.log('Status:', event.status, event.statusText);
            console.log('Body:', event.body);
            console.groupEnd();
          }
        },
        error: (error: HttpErrorResponse) => {
          const elapsed = Date.now() - startTime;

          // Log error response with detailed information
          console.group(`❌ HTTP Error: ${request.method} ${request.url} (${elapsed}ms)`);
          console.error('Status:', error.status, error.statusText);
          console.error('Error:', error.error);
          console.error('Message:', error.message);
          console.error('Full Error Object:', error);

          // Log specific error details
          if (error.status === 0) {
            console.error('⚠️ Status 0 Error - This usually means:');
            console.error('  - Network connection failed');
            console.error('  - CORS issue');
            console.error('  - Backend server is not running');
            console.error('  - Request was blocked by browser');
            console.error('Check:');
            console.error(`  1. Is backend running at ${request.url}?`);
            console.error('  2. Are CORS headers configured correctly?');
            console.error('  3. Check browser Network tab for more details');
          } else if (error.status === 400) {
            console.error('⚠️ Bad Request - Validation failed');
            console.error('Validation Errors:', error.error?.errors || error.error);
          } else if (error.status === 401) {
            console.error('⚠️ Unauthorized - Authentication required');
          } else if (error.status === 404) {
            console.error('⚠️ Not Found - Endpoint does not exist');
          } else if (error.status === 500) {
            console.error('⚠️ Server Error - Check backend logs');
          }

          console.groupEnd();
        }
      })
    );
  }
}
