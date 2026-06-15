import { ComponentFixture, TestBed } from '@angular/core/testing';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { RouterTestingModule } from '@angular/router/testing';
import { LoginComponent } from './login.component';
import { AuthService } from '../../services/auth.service';
import { of } from 'rxjs';

describe('LoginComponent', () => {
  let component: LoginComponent;
  let fixture: ComponentFixture<LoginComponent>;
  let authService: jasmine.SpyObj<AuthService>;

  beforeEach(async () => {
    const spy = jasmine.createSpyObj('AuthService', ['login']);

    await TestBed.configureTestingModule({
      imports: [LoginComponent, HttpClientTestingModule, RouterTestingModule],
      providers: [{ provide: AuthService, useValue: spy }]
    }).compileComponents();

    authService = TestBed.inject(AuthService) as jasmine.SpyObj<AuthService>;
    fixture = TestBed.createComponent(LoginComponent);
    component = fixture.componentInstance;
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should call login on submit', () => {
    authService.login.and.returnValue(of({
      valid: true,
      data: { accessToken: 'token', refreshToken: 'refresh', email: 'test@test.com', userName: 'test' },
      messages: [],
      statusCode: 200
    }));

    component.email = 'test@test.com';
    component.password = '123456';
    component.onSubmit();

    expect(authService.login).toHaveBeenCalledWith({
      email: 'test@test.com',
      password: '123456'
    });
  });

  it('should show error on failed login', () => {
    authService.login.and.returnValue(of({
      valid: false,
      data: null as any,
      messages: ['Credenciais inválidas.'],
      statusCode: 401
    }));

    component.email = 'wrong@test.com';
    component.password = 'wrong';
    component.onSubmit();

    expect(component.errorMessage).toBe('Credenciais inválidas.');
  });
});
