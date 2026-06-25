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
  filterSearchTerm = '';

  currentPage = 1;
  pageSize = 50;
  totalPages = 0;
  totalCount = 0;

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

  get pageNumbers(): number[] {
    const pages: number[] = [];
    const start = Math.max(1, this.currentPage - 2);
    const end = Math.min(this.totalPages, this.currentPage + 2);
    for (let i = start; i <= end; i++) {
      pages.push(i);
    }
    return pages;
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
      this.filterReadingStatus || undefined,
      this.filterSearchTerm || undefined,
      this.currentPage,
      this.pageSize
    ).subscribe({
      next: (result) => {
        if (result.valid) {
          this.books = result.data.items;
          this.totalCount = result.data.totalCount;
          this.currentPage = result.data.pageNumber;
          this.pageSize = result.data.pageSize;
          this.totalPages = result.data.totalPages;
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
    this.currentPage = 1;
    this.loadBooks();
  }

  goToPage(page: number): void {
    if (page < 1 || page > this.totalPages) return;
    this.currentPage = page;
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

  private readonly statusCycle = [null, 'Lendo', 'Lido', 'Relendo', 'Relido', 'Abandonado'] as const;

  cycleStatus(book: Book): void {
    const currentIdx = this.statusCycle.indexOf(book.readingStatus as typeof this.statusCycle[number]);
    const nextStatus = this.statusCycle[(currentIdx + 1) % this.statusCycle.length];
    this.bookService.updateStatus(book.id, { readingStatus: nextStatus }).subscribe({
      next: (result) => {
        if (result.valid) book.readingStatus = result.data.readingStatus;
      }
    });
  }
}
