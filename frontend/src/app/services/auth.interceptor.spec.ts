import { TestBed } from '@angular/core/testing';
import { HttpClient, provideHttpClient, withInterceptors } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { Router } from '@angular/router';
import { AuthService } from './auth.service';
import { authInterceptor } from './auth.interceptor';
import { of, throwError } from 'rxjs';

describe('authInterceptor', () => {
  let httpMock: HttpTestingController;
  let http: HttpClient;
  let authService: jasmine.SpyObj<AuthService>;
  let router: jasmine.SpyObj<Router>;

  beforeEach(() => {
    localStorage.clear();

    const authSpy = jasmine.createSpyObj('AuthService', ['getToken', 'getRefreshToken', 'refreshToken']);
    const routerSpy = jasmine.createSpyObj('Router', ['navigate']);

    TestBed.configureTestingModule({
      providers: [
        provideHttpClient(withInterceptors([authInterceptor])),
        provideHttpClientTesting(),
        { provide: AuthService, useValue: authSpy },
        { provide: Router, useValue: routerSpy }
      ]
    });

    httpMock = TestBed.inject(HttpTestingController);
    http = TestBed.inject(HttpClient);
    authService = TestBed.inject(AuthService) as jasmine.SpyObj<AuthService>;
    router = TestBed.inject(Router) as jasmine.SpyObj<Router>;
  });

  afterEach(() => {
    httpMock.verify();
    localStorage.clear();
  });

  it('should add Authorization header when token exists', () => {
    authService.getToken.and.returnValue('my_token');

    http.get('/api/books').subscribe();

    const req = httpMock.expectOne('/api/books');
    expect(req.request.headers.get('Authorization')).toBe('Bearer my_token');
    req.flush([]);
  });

  it('should not add Authorization header for auth endpoints', () => {
    authService.getToken.and.returnValue('my_token');

    http.post('/api/auth/login', {}).subscribe();

    const req = httpMock.expectOne('/api/auth/login');
    expect(req.request.headers.has('Authorization')).toBeFalse();
    req.flush({});
  });

  it('should attempt refresh on 401 and retry request', () => {
    localStorage.setItem('accessToken', 'expired_token');
    authService.getToken.and.callFake(() => localStorage.getItem('accessToken'));
    authService.getRefreshToken.and.returnValue('refresh_token');
    authService.refreshToken.and.callFake(() => {
      localStorage.setItem('accessToken', 'new_token');
      return of({
        valid: true,
        data: { accessToken: 'new_token', refreshToken: 'new_refresh', email: '', userName: '' },
        messages: [],
        statusCode: 200
      });
    });

    http.get('/api/books').subscribe();

    const req = httpMock.expectOne('/api/books');
    req.flush('Unauthorized', { status: 401, statusText: 'Unauthorized' });

    expect(authService.refreshToken).toHaveBeenCalled();

    const retryReq = httpMock.expectOne('/api/books');
    expect(retryReq.request.headers.get('Authorization')).toBe('Bearer new_token');
    retryReq.flush([]);
  });

  it('should redirect to login when refresh fails', () => {
    authService.getToken.and.returnValue('expired_token');
    authService.getRefreshToken.and.returnValue('refresh_token');
    authService.refreshToken.and.returnValue(of(null));

    http.get('/api/books').subscribe({
      error: () => {}
    });

    const req = httpMock.expectOne('/api/books');
    req.flush('Unauthorized', { status: 401, statusText: 'Unauthorized' });

    expect(authService.refreshToken).toHaveBeenCalled();
    expect(router.navigate).toHaveBeenCalledWith(['/login']);
  });
});
