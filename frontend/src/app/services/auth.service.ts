import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, BehaviorSubject, tap } from 'rxjs';
import { AuthResponse, LoginDto, RegisterDto } from '../models/auth.model';
import { Result } from '../models/book.model';
import { environment } from '../../environments/environment';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly apiUrl = `${environment.apiUrl}/api/auth`;
  private loggedIn = new BehaviorSubject<boolean>(this.hasToken());

  isLoggedIn$ = this.loggedIn.asObservable();

  constructor(private http: HttpClient) {}

  login(dto: LoginDto): Observable<Result<AuthResponse>> {
    return this.http.post<Result<AuthResponse>>(`${this.apiUrl}/login`, dto).pipe(
      tap(result => {
        if (result.valid) {
          this.storeToken(result.data);
          this.loggedIn.next(true);
        }
      })
    );
  }

  register(dto: RegisterDto): Observable<Result<AuthResponse>> {
    return this.http.post<Result<AuthResponse>>(`${this.apiUrl}/register`, dto).pipe(
      tap(result => {
        if (result.valid) {
          this.storeToken(result.data);
          this.loggedIn.next(true);
        }
      })
    );
  }

  logout(): void {
    localStorage.removeItem('token');
    localStorage.removeItem('email');
    localStorage.removeItem('userName');
    this.loggedIn.next(false);
  }

  getToken(): string | null {
    return localStorage.getItem('token');
  }

  getUserName(): string | null {
    return localStorage.getItem('userName');
  }

  isLoggedIn(): boolean {
    return this.hasToken();
  }

  private storeToken(data: AuthResponse): void {
    localStorage.setItem('token', data.token);
    localStorage.setItem('email', data.email);
    localStorage.setItem('userName', data.userName);
  }

  private hasToken(): boolean {
    return !!localStorage.getItem('token');
  }
}
