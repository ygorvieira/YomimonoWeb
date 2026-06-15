import { ComponentFixture, TestBed } from '@angular/core/testing';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { RouterTestingModule } from '@angular/router/testing';
import { RegisterComponent } from './register.component';
import { AuthService } from '../../services/auth.service';
import { of } from 'rxjs';

describe('RegisterComponent', () => {
  let component: RegisterComponent;
  let fixture: ComponentFixture<RegisterComponent>;
  let authService: jasmine.SpyObj<AuthService>;

  beforeEach(async () => {
    const spy = jasmine.createSpyObj('AuthService', ['register']);

    await TestBed.configureTestingModule({
      imports: [RegisterComponent, HttpClientTestingModule, RouterTestingModule],
      providers: [{ provide: AuthService, useValue: spy }]
    }).compileComponents();

    authService = TestBed.inject(AuthService) as jasmine.SpyObj<AuthService>;
    fixture = TestBed.createComponent(RegisterComponent);
    component = fixture.componentInstance;
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should call register on submit', () => {
    authService.register.and.returnValue(of({
      valid: true,
      data: { accessToken: 'token', refreshToken: 'refresh', email: 'test@test.com', userName: 'test' },
      messages: [],
      statusCode: 201
    }));

    component.userName = 'test';
    component.email = 'test@test.com';
    component.password = '123456';
    component.confirmPassword = '123456';
    component.onSubmit();

    expect(authService.register).toHaveBeenCalledWith({
      email: 'test@test.com',
      password: '123456',
      userName: 'test'
    });
  });

  it('should show error when passwords do not match', () => {
    component.password = '123456';
    component.confirmPassword = '654321';
    component.onSubmit();

    expect(component.errorMessage).toBe('As senhas não conferem.');
    expect(authService.register).not.toHaveBeenCalled();
  });

  it('should show error on failed register', () => {
    authService.register.and.returnValue(of({
      valid: false,
      data: null as any,
      messages: ['Email já cadastrado.'],
      statusCode: 400
    }));

    component.userName = 'test';
    component.email = 'existing@test.com';
    component.password = '123456';
    component.confirmPassword = '123456';
    component.onSubmit();

    expect(component.errorMessage).toBe('Email já cadastrado.');
  });
});
