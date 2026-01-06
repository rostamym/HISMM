import { Injectable } from '@angular/core';
import { jwtDecode } from 'jwt-decode';
import { environment } from '@environments/environment';
import { User } from '../models/user.model';

interface JwtPayload {
  sub: string;
  email: string;
  role: string;
  exp: number;
  iat: number;
}

@Injectable({
  providedIn: 'root'
})
export class TokenService {
  private readonly TOKEN_KEY = environment.tokenKey;
  private readonly REFRESH_TOKEN_KEY = environment.refreshTokenKey;
  private readonly USER_KEY = 'hospital_user';

  saveToken(token: string): void {
    localStorage.setItem(this.TOKEN_KEY, token);
  }

  getToken(): string | null {
    return localStorage.getItem(this.TOKEN_KEY);
  }

  removeToken(): void {
    localStorage.removeItem(this.TOKEN_KEY);
  }

  saveRefreshToken(refreshToken: string): void {
    localStorage.setItem(this.REFRESH_TOKEN_KEY, refreshToken);
  }

  getRefreshToken(): string | null {
    return localStorage.getItem(this.REFRESH_TOKEN_KEY);
  }

  removeRefreshToken(): void {
    localStorage.removeItem(this.REFRESH_TOKEN_KEY);
  }

  saveUser(user: User): void {
    localStorage.setItem(this.USER_KEY, JSON.stringify(user));
  }

  getUser(): User | null {
    const userStr = localStorage.getItem(this.USER_KEY);
    if (userStr) {
      try {
        return JSON.parse(userStr) as User;
      } catch {
        return null;
      }
    }
    return null;
  }

  removeUser(): void {
    localStorage.removeItem(this.USER_KEY);
  }

  decodeToken(token: string): JwtPayload | null {
    try {
      return jwtDecode<JwtPayload>(token);
    } catch {
      return null;
    }
  }

  isTokenExpired(token?: string): boolean {
    const tokenToCheck = token || this.getToken();
    if (!tokenToCheck) {
      return true;
    }

    const decoded = this.decodeToken(tokenToCheck);
    if (!decoded) {
      return true;
    }

    const expirationDate = new Date(decoded.exp * 1000);
    return expirationDate < new Date();
  }

  getTokenExpirationDate(token?: string): Date | null {
    const tokenToCheck = token || this.getToken();
    if (!tokenToCheck) {
      return null;
    }

    const decoded = this.decodeToken(tokenToCheck);
    if (!decoded) {
      return null;
    }

    return new Date(decoded.exp * 1000);
  }
}
