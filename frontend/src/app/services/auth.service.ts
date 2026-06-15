import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, BehaviorSubject, tap, catchError, of, switchMap } from 'rxjs';
import { AuthResponse, LoginDto, RegisterDto, RefreshRequestDto } from '../models/auth.model';
import { Result } from '../models/book.model';
import { environment } from '../../environments/environment';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly apiUrl = `${environment.apiUrl}/api/auth`;
  private loggedIn = new BehaviorSubject<boolean>(this.hasToken());
  private refreshing = false;
  private refreshSubject = new BehaviorSubject<string | null>(null);

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

  refreshToken(): Observable<Result<AuthResponse> | null> {
    const refreshToken = this.getRefreshToken();
    if (!refreshToken) {
      this.logout();
      return of(null);
    }

    if (this.refreshing) {
      return this.refreshSubject.pipe(
        switchMap(token => {
          if (token) {
            const result: Result<AuthResponse> = {
              valid: true,
              data: { accessToken: token, refreshToken: '', email: '', userName: '' },
              messages: [],
              statusCode: 200
            };
            return of(result);
          }
          return of(null);
        })
      );
    }

    this.refreshing = true;
    this.refreshSubject.next(null);

    return this.http.post<Result<AuthResponse>>(`${this.apiUrl}/refresh`, { refreshToken } as RefreshRequestDto).pipe(
      tap(result => {
        if (result.valid && result.data) {
          this.storeToken(result.data);
          this.refreshSubject.next(result.data.accessToken);
        } else {
          this.logout();
          this.refreshSubject.next(null);
        }
        this.refreshing = false;
      }),
      catchError(() => {
        this.logout();
        this.refreshing = false;
        this.refreshSubject.next(null);
        return of(null);
      })
    );
  }

  revokeToken(): Observable<Result<boolean>> {
    const refreshToken = this.getRefreshToken();
    return this.http.post<Result<boolean>>(`${this.apiUrl}/revoke`, { refreshToken } as RefreshRequestDto);
  }

  logout(): void {
    localStorage.removeItem('accessToken');
    localStorage.removeItem('refreshToken');
    localStorage.removeItem('email');
    localStorage.removeItem('userName');
    this.loggedIn.next(false);
  }

  getToken(): string | null {
    return localStorage.getItem('accessToken');
  }

  getRefreshToken(): string | null {
    return localStorage.getItem('refreshToken');
  }

  getUserName(): string | null {
    return localStorage.getItem('userName');
  }

  isLoggedIn(): boolean {
    return this.hasToken();
  }

  private storeToken(data: AuthResponse): void {
    localStorage.setItem('accessToken', data.accessToken);
    localStorage.setItem('refreshToken', data.refreshToken);
    localStorage.setItem('email', data.email);
    localStorage.setItem('userName', data.userName);
  }

  private hasToken(): boolean {
    return !!localStorage.getItem('accessToken');
  }
}
