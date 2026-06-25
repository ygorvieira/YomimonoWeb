import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { GenreService } from '../../services/genre.service';
import { Genre } from '../../models/genre.model';

@Component({
  selector: 'app-genre-list',
  standalone: true,
  imports: [CommonModule, RouterModule, FormsModule],
  templateUrl: './genre-list.component.html'
})
export class GenreListComponent implements OnInit {
  genres: Genre[] = [];
  loading = false;
  errorMessage = '';
  searchTerm = '';

  constructor(private genreService: GenreService) {}

  ngOnInit(): void { this.loadGenres(); }

  loadGenres(): void {
    this.loading = true;
    this.errorMessage = '';
    this.genreService.getAll(this.searchTerm || undefined).subscribe({
      next: (result) => {
        if (result.valid) this.genres = result.data;
        else this.errorMessage = result.messages?.join(', ') || 'Erro ao carregar gêneros.';
        this.loading = false;
      },
      error: (err) => { this.errorMessage = err.error?.messages?.join(', ') || 'Erro de conexão.'; this.loading = false; }
    });
  }

  onSearchChange(): void {
    this.loadGenres();
  }

  deleteGenre(id: string): void {
    if (confirm('Excluir este gênero?')) {
      this.genreService.delete(id).subscribe({
        next: (result) => { if (result.valid) this.genres = this.genres.filter(g => g.id !== id); }
      });
    }
  }
}
