import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { AuthorService } from '../../services/author.service';
import { CreateAuthorDto } from '../../models/author.model';

@Component({
  selector: 'app-author-form',
  standalone: true,
  imports: [CommonModule, RouterModule, FormsModule],
  templateUrl: './author-form.component.html'
})
export class AuthorFormComponent implements OnInit {
  isEditMode = false;
  authorId: string | null = null;
  submitting = false;
  errorMessage = '';
  successMessage = '';
  name = '';

  constructor(
    private route: ActivatedRoute, private router: Router,
    private authorService: AuthorService
  ) {}

  ngOnInit(): void {
    this.authorId = this.route.snapshot.paramMap.get('id');
    if (this.authorId) {
      this.isEditMode = true;
      this.authorService.getById(this.authorId).subscribe({
        next: (r) => { if (r.valid) this.name = r.data.name; }
      });
    }
  }

  onSubmit(): void {
    this.submitting = true;
    this.errorMessage = '';
    this.successMessage = '';

    const request = this.isEditMode
      ? this.authorService.update(this.authorId!, { name: this.name })
      : this.authorService.create({ name: this.name });

    request.subscribe({
      next: (result) => {
        if (result.valid) {
          this.successMessage = this.isEditMode ? 'Autor atualizado!' : 'Autor cadastrado!';
          setTimeout(() => this.router.navigate(['/authors']), 1500);
        } else {
          this.errorMessage = result.messages?.join(', ') || 'Erro ao salvar.';
        }
        this.submitting = false;
      },
      error: () => { this.errorMessage = 'Erro de conexão.'; this.submitting = false; }
    });
  }
}
