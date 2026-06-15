import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { AuthService } from './auth.service';

describe('AuthService', () => {
  let service: AuthService;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    localStorage.clear();
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [AuthService]
    });
    service = TestBed.inject(AuthService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
    localStorage.clear();
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should login and store tokens', () => {
    service.login({ email: 'test@test.com', password: '123456' }).subscribe(result => {
      expect(result.valid).toBeTrue();
      expect(localStorage.getItem('accessToken')).toBe('jwt_token');
      expect(localStorage.getItem('refreshToken')).toBe('refresh_token');
    });

    const req = httpMock.expectOne(`/api/auth/login`);
    expect(req.request.method).toBe('POST');
    req.flush({
      valid: true,
      data: { accessToken: 'jwt_token', refreshToken: 'refresh_token', email: 'test@test.com', userName: 'test' },
      messages: [],
      statusCode: 200
    });
  });

  it('should logout and clear storage', () => {
    localStorage.setItem('accessToken', 'jwt_token');
    localStorage.setItem('refreshToken', 'refresh_token');
    localStorage.setItem('email', 'test@test.com');
    localStorage.setItem('userName', 'test');

    service.logout();

    expect(localStorage.getItem('accessToken')).toBeNull();
    expect(localStorage.getItem('refreshToken')).toBeNull();
    expect(service.isLoggedIn()).toBeFalse();
  });

  it('should return token from storage', () => {
    localStorage.setItem('accessToken', 'my_token');
    expect(service.getToken()).toBe('my_token');
  });

  it('should return refresh token from storage', () => {
    localStorage.setItem('refreshToken', 'my_refresh_token');
    expect(service.getRefreshToken()).toBe('my_refresh_token');
  });

  it('should refresh token', () => {
    localStorage.setItem('refreshToken', 'valid_refresh_token');

    service.refreshToken().subscribe(result => {
      expect(result).not.toBeNull();
      expect(result!.valid).toBeTrue();
    });

    const req = httpMock.expectOne(`/api/auth/refresh`);
    expect(req.request.method).toBe('POST');
    expect(req.request.body).toEqual({ refreshToken: 'valid_refresh_token' });
    req.flush({
      valid: true,
      data: { accessToken: 'new_token', refreshToken: 'new_refresh', email: 'test@test.com', userName: 'test' },
      messages: [],
      statusCode: 200
    });

    expect(localStorage.getItem('accessToken')).toBe('new_token');
  });

  it('should revoke token', () => {
    localStorage.setItem('refreshToken', 'valid_refresh_token');

    service.revokeToken().subscribe(result => {
      expect(result.valid).toBeTrue();
    });

    const req = httpMock.expectOne(`/api/auth/revoke`);
    expect(req.request.method).toBe('POST');
    req.flush({
      valid: true,
      data: true,
      messages: [],
      statusCode: 200
    });
  });
});
