import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './register.component.html'
})
export class RegisterComponent {
  userName = '';
  email = '';
  password = '';
  confirmPassword = '';
  errorMessage = '';
  loading = false;

  constructor(
    private authService: AuthService,
    private router: Router
  ) {}

  onSubmit(): void {
    this.loading = true;
    this.errorMessage = '';

    if (this.password !== this.confirmPassword) {
      this.errorMessage = 'As senhas não conferem.';
      this.loading = false;
      return;
    }

    this.authService.register({
      email: this.email,
      password: this.password,
      userName: this.userName
    }).subscribe({
      next: (result) => {
        if (result.valid) {
          this.router.navigate(['/books']);
        } else {
          this.errorMessage = result.messages?.join(', ') || 'Erro ao cadastrar.';
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
