import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { GenreService } from '../../services/genre.service';

@Component({
  selector: 'app-genre-form',
  standalone: true,
  imports: [CommonModule, RouterModule, FormsModule],
  templateUrl: './genre-form.component.html'
})
export class GenreFormComponent implements OnInit {
  isEditMode = false;
  genreId: string | null = null;
  submitting = false;
  errorMessage = '';
  successMessage = '';
  name = '';

  constructor(
    private route: ActivatedRoute, private router: Router,
    private genreService: GenreService
  ) {}

  ngOnInit(): void {
    this.genreId = this.route.snapshot.paramMap.get('id');
    if (this.genreId) {
      this.isEditMode = true;
      this.genreService.getById(this.genreId).subscribe({
        next: (r) => { if (r.valid) this.name = r.data.name; }
      });
    }
  }

  onSubmit(): void {
    this.submitting = true;
    this.errorMessage = '';
    this.successMessage = '';

    const request = this.isEditMode
      ? this.genreService.update(this.genreId!, { name: this.name })
      : this.genreService.create({ name: this.name });

    request.subscribe({
      next: (result) => {
        if (result.valid) {
          this.successMessage = this.isEditMode ? 'Gênero atualizado!' : 'Gênero cadastrado!';
          setTimeout(() => this.router.navigate(['/genres']), 1500);
        } else {
          this.errorMessage = result.messages?.join(', ') || 'Erro ao salvar.';
        }
        this.submitting = false;
      },
      error: () => { this.errorMessage = 'Erro de conexão.'; this.submitting = false; }
    });
  }
}
