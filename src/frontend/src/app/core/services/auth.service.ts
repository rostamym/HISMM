import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable, tap } from 'rxjs';
import { Router } from '@angular/router';

import { API_ENDPOINTS } from '../constants/api-endpoints';
import { TokenService } from './token.service';
import {
  User,
  AuthResponse,
  LoginRequest,
  RegisterRequest,
  UserRole
} from '../models/user.model';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private currentUserSubject: BehaviorSubject<User | null>;
  public currentUser$: Observable<User | null>;

  constructor(
    private http: HttpClient,
    private tokenService: TokenService,
    private router: Router
  ) {
    this.currentUserSubject = new BehaviorSubject<User | null>(
      this.tokenService.getUser()
    );
    this.currentUser$ = this.currentUserSubject.asObservable();
  }

  public get currentUserValue(): User | null {
    return this.currentUserSubject.value;
  }

  public get isAuthenticated(): boolean {
    return !!this.tokenService.getToken();
  }

  public get currentUserRole(): UserRole | null {
    return this.currentUserValue?.role || null;
  }

  login(credentials: LoginRequest): Observable<AuthResponse> {
    return this.http
      .post<AuthResponse>(API_ENDPOINTS.AUTH.LOGIN, credentials)
      .pipe(
        tap(response => {
          this.tokenService.saveToken(response.token);
          this.tokenService.saveRefreshToken(response.refreshToken);
          this.tokenService.saveUser(response.user);
          this.currentUserSubject.next(response.user);
        })
      );
  }

  register(request: RegisterRequest): Observable<AuthResponse> {
    return this.http
      .post<AuthResponse>(API_ENDPOINTS.AUTH.REGISTER, request)
      .pipe(
        tap(response => {
          this.tokenService.saveToken(response.token);
          this.tokenService.saveRefreshToken(response.refreshToken);
          this.tokenService.saveUser(response.user);
          this.currentUserSubject.next(response.user);
        })
      );
  }

  refreshToken(): Observable<AuthResponse> {
    const refreshToken = this.tokenService.getRefreshToken();
    return this.http
      .post<AuthResponse>(API_ENDPOINTS.AUTH.REFRESH, { refreshToken })
      .pipe(
        tap(response => {
          this.tokenService.saveToken(response.token);
          this.tokenService.saveRefreshToken(response.refreshToken);
        })
      );
  }

  logout(): void {
    this.tokenService.removeToken();
    this.tokenService.removeRefreshToken();
    this.tokenService.removeUser();
    this.currentUserSubject.next(null);
    this.router.navigate(['/auth/login']);
  }

  forgotPassword(email: string): Observable<any> {
    return this.http.post(API_ENDPOINTS.AUTH.FORGOT_PASSWORD, { email });
  }

  resetPassword(token: string, newPassword: string): Observable<any> {
    return this.http.post(API_ENDPOINTS.AUTH.RESET_PASSWORD, {
      token,
      newPassword
    });
  }
}
