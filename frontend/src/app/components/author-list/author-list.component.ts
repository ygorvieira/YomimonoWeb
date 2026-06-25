import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { AuthorService } from '../../services/author.service';
import { Author } from '../../models/author.model';

@Component({
  selector: 'app-author-list',
  standalone: true,
  imports: [CommonModule, RouterModule, FormsModule],
  templateUrl: './author-list.component.html'
})
export class AuthorListComponent implements OnInit {
  authors: Author[] = [];
  loading = false;
  errorMessage = '';
  searchTerm = '';

  constructor(private authorService: AuthorService, private router: Router) {}

  ngOnInit(): void { this.loadAuthors(); }

  loadAuthors(): void {
    this.loading = true;
    this.errorMessage = '';
    this.authorService.getAll(this.searchTerm || undefined).subscribe({
      next: (result) => {
        if (result.valid) this.authors = result.data;
        else this.errorMessage = result.messages?.join(', ') || 'Erro ao carregar autores.';
        this.loading = false;
      },
      error: (err) => { this.errorMessage = err.error?.messages?.join(', ') || 'Erro de conexão.'; this.loading = false; }
    });
  }

  onSearchChange(): void {
    this.loadAuthors();
  }

  deleteAuthor(id: string): void {
    if (confirm('Excluir este autor?')) {
      this.authorService.delete(id).subscribe({
        next: (result) => { if (result.valid) this.authors = this.authors.filter(a => a.id !== id); }
      });
    }
  }
}
