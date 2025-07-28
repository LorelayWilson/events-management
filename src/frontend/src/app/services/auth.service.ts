import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { BehaviorSubject, Observable } from 'rxjs';
import { tap } from 'rxjs/operators';
import { EncryptionService } from './encryption.service';

export interface LoginDto {
  email: string;
  password: string;
}

export interface RegisterDto {
  email: string;
  password: string;
  firstName: string;
  lastName: string;
}

export interface LoginResponseDto {
  userId: string;
  email: string;
  firstName: string;
  lastName: string;
  token: string;
}

export interface UserDto {
  userId: string;
  email: string;
  firstName: string;
  lastName: string;
}

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private readonly API_URL = 'http://localhost:5000/api';
  private currentUserSubject = new BehaviorSubject<LoginResponseDto | null>(null);
  public currentUser$ = this.currentUserSubject.asObservable();

  constructor(
    private http: HttpClient,
    private router: Router,
    private encryptionService: EncryptionService
  ) {}

  login(loginData: LoginDto): Observable<LoginResponseDto> {
    return this.http.post<LoginResponseDto>(`${this.API_URL}/auth/login`, loginData)
      .pipe(
        tap(response => {
          this.encryptionService.setEncryptedItem('currentUser', JSON.stringify(response));
          this.encryptionService.setEncryptedItem('token', response.token);
          this.currentUserSubject.next(response);
        })
      );
  }

  register(registerData: RegisterDto): Observable<LoginResponseDto> {
    return this.http.post<LoginResponseDto>(`${this.API_URL}/auth/register`, registerData)
      .pipe(
        tap(response => {
          this.encryptionService.setEncryptedItem('currentUser', JSON.stringify(response));
          this.encryptionService.setEncryptedItem('token', response.token);
          this.currentUserSubject.next(response);
        })
      );
  }

  logout(): void {
    this.encryptionService.removeEncryptedItem('currentUser');
    this.encryptionService.removeEncryptedItem('token');
    this.currentUserSubject.next(null);
    this.router.navigate(['/login']);
  }

  checkAuthStatus(): void {
    const userStr = this.encryptionService.getDecryptedItem('currentUser');
    if (userStr) {
      try {
        const user = JSON.parse(userStr);
        this.currentUserSubject.next(user);
      } catch (error) {
        console.error('Error parsing user:', error);
        // If there's a parsing error, clean corrupted data
        this.logout();
      }
    }
  }

  isLoggedIn(): boolean {
    return this.currentUserSubject.value !== null;
  }

  getCurrentUser(): LoginResponseDto | null {
    return this.currentUserSubject.value;
  }

  getToken(): string | null {
    return this.encryptionService.getDecryptedItem('token');
  }

  /**
   * Get user information by ID
   * Note: This endpoint might not be available in all backend implementations
   * If the API returns 404, the frontend will handle it gracefully
   */
  getUserById(userId: string): Observable<UserDto> {
    return this.http.get<UserDto>(`${this.API_URL}/users/${userId}`, {
      headers: {
        'Authorization': `Bearer ${this.getToken()}`,
        'Content-Type': 'application/json'
      }
    });
  }
}