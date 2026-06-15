import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { GenreService } from '../../services/genre.service';
import { Genre } from '../../models/genre.model';

@Component({
  selector: 'app-genre-list',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './genre-list.component.html'
})
export class GenreListComponent implements OnInit {
  genres: Genre[] = [];
  loading = false;
  errorMessage = '';

  constructor(private genreService: GenreService) {}

  ngOnInit(): void { this.loadGenres(); }

  loadGenres(): void {
    this.loading = true;
    this.errorMessage = '';
    this.genreService.getAll().subscribe({
      next: (result) => {
        if (result.valid) this.genres = result.data;
        else this.errorMessage = result.messages?.join(', ') || 'Erro ao carregar gêneros.';
        this.loading = false;
      },
      error: () => { this.errorMessage = 'Erro de conexão.'; this.loading = false; }
    });
  }

  deleteGenre(id: string): void {
    if (confirm('Excluir este gênero?')) {
      this.genreService.delete(id).subscribe({
        next: (result) => { if (result.valid) this.genres = this.genres.filter(g => g.id !== id); }
      });
    }
  }
}
