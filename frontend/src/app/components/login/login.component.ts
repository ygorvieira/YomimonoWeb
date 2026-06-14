import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './login.component.html'
})
export class LoginComponent {
  email = '';
  password = '';
  errorMessage = '';
  loading = false;

  constructor(
    private authService: AuthService,
    private router: Router
  ) {}

  onSubmit(): void {
    this.loading = true;
    this.errorMessage = '';

    this.authService.login({ email: this.email, password: this.password }).subscribe({
      next: (result) => {
        if (result.valid) {
          this.router.navigate(['/books']);
        } else {
          this.errorMessage = result.messages?.join(', ') || 'Erro ao fazer login.';
        }
        this.loading = false;
      },
      error: () => {
        this.errorMessage = 'Erro de conexão com o servidor.';
        this.loading = false;
      }
    });
  }
}
