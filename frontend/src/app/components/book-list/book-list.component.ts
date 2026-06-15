import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { BookService } from '../../services/book.service';
import { AuthorService } from '../../services/author.service';
import { GenreService } from '../../services/genre.service';
import { Book } from '../../models/book.model';
import { Author } from '../../models/author.model';
import { Genre } from '../../models/genre.model';

@Component({
  selector: 'app-book-list',
  standalone: true,
  imports: [CommonModule, RouterModule, FormsModule],
  templateUrl: './book-list.component.html'
})
export class BookListComponent implements OnInit {
  books: Book[] = [];
  loading = false;
  errorMessage = '';
  authors: Author[] = [];
  genres: Genre[] = [];

  filterGenreId = '';
  filterAuthorId = '';
  filterReadingStatus = '';

  constructor(
    private bookService: BookService,
    private authorService: AuthorService,
    private genreService: GenreService
  ) {}

  ngOnInit(): void {
    this.loadAuthors();
    this.loadGenres();
    this.loadBooks();
  }

  loadAuthors(): void {
    this.authorService.getAll().subscribe({
      next: (result) => { if (result.valid) this.authors = result.data; }
    });
  }

  loadGenres(): void {
    this.genreService.getAll().subscribe({
      next: (result) => { if (result.valid) this.genres = result.data; }
    });
  }

  loadBooks(): void {
    this.loading = true;
    this.errorMessage = '';
    this.bookService.getAll(
      this.filterGenreId || undefined,
      this.filterAuthorId || undefined,
      this.filterReadingStatus || undefined
    ).subscribe({
      next: (result) => {
        if (result.valid) {
          this.books = result.data;
        } else {
          this.errorMessage = result.messages?.join(', ') || 'Erro ao carregar livros.';
        }
        this.loading = false;
      },
      error: (err) => {
        this.errorMessage = err.error?.messages?.join(', ') || 'Erro de conexão com o servidor.';
        this.loading = false;
      }
    });
  }

  onFilterChange(): void {
    this.loadBooks();
  }

  deleteBook(id: string): void {
    if (confirm('Tem certeza que deseja excluir este livro?')) {
      this.bookService.delete(id).subscribe({
        next: (result) => {
          if (result.valid) {
            this.books = this.books.filter(b => b.id !== id);
          }
        }
      });
    }
  }

  toggleLike(book: Book): void {
    this.bookService.updateStatus(book.id, { isLiked: !book.isLiked }).subscribe({
      next: (result) => {
        if (result.valid) book.isLiked = result.data.isLiked;
      }
    });
  }
}
