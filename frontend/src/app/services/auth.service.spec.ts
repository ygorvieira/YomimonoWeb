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

  it('should login and store token', () => {
    service.login({ email: 'test@test.com', password: '123456' }).subscribe(result => {
      expect(result.valid).toBeTrue();
      expect(localStorage.getItem('token')).toBe('jwt_token');
    });

    const req = httpMock.expectOne(`/api/auth/login`);
    expect(req.request.method).toBe('POST');
    req.flush({
      valid: true,
      data: { token: 'jwt_token', email: 'test@test.com', userName: 'test' },
      messages: [],
      statusCode: 200
    });
  });

  it('should logout and clear storage', () => {
    localStorage.setItem('token', 'jwt_token');
    localStorage.setItem('email', 'test@test.com');
    localStorage.setItem('userName', 'test');

    service.logout();

    expect(localStorage.getItem('token')).toBeNull();
    expect(service.isLoggedIn()).toBeFalse();
  });

  it('should return token from storage', () => {
    localStorage.setItem('token', 'my_token');
    expect(service.getToken()).toBe('my_token');
  });
});
