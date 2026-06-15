import { Component, OnInit, HostListener } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { BookService } from '../../services/book.service';
import { AuthorService } from '../../services/author.service';
import { GenreService } from '../../services/genre.service';
import { CreateBookDto, UpdateBookDto } from '../../models/book.model';
import { Author } from '../../models/author.model';
import { Genre } from '../../models/genre.model';

@Component({
  selector: 'app-book-form',
  standalone: true,
  imports: [CommonModule, RouterModule, FormsModule],
  templateUrl: './book-form.component.html'
})
export class BookFormComponent implements OnInit {
  isEditMode = false;
  bookId: string | null = null;
  loading = false;
  submitting = false;
  errorMessage = '';
  successMessage = '';
  authors: Author[] = [];
  genres: Genre[] = [];

  model: CreateBookDto = {
    title: '',
    authorIds: [],
    isbn: '',
    publicationYear: new Date().getFullYear(),
    publisher: '',
    genreId: '',
    description: null,
    pageCount: 0,
    coverUrl: null,
    readingStatus: null,
    isLiked: false
  };

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private bookService: BookService,
    private authorService: AuthorService,
    private genreService: GenreService
  ) {}

  ngOnInit(): void {
    this.loadAuthors();
    this.loadGenres();
    this.bookId = this.route.snapshot.paramMap.get('id');
    if (this.bookId) {
      this.isEditMode = true;
      this.loadBook(this.bookId);
    }
  }

  loadAuthors(): void {
    this.authorService.getAll().subscribe({
      next: (result) => { if (result.valid) this.authors = result.data; },
      error: (err) => this.errorMessage = err.error?.messages?.join(', ') || 'Erro ao carregar autores.'
    });
  }

  loadGenres(): void {
    this.genreService.getAll().subscribe({
      next: (result) => { if (result.valid) this.genres = result.data; },
      error: (err) => this.errorMessage = err.error?.messages?.join(', ') || 'Erro ao carregar gêneros.'
    });
  }

  loadBook(id: string): void {
    this.loading = true;
    this.bookService.getById(id).subscribe({
      next: (result) => {
        if (result.valid) {
          const book = result.data;
          this.model = {
            title: book.title,
            authorIds: book.authorIds,
            isbn: book.isbn,
            publicationYear: book.publicationYear,
            publisher: book.publisher,
            genreId: book.genreId,
            description: book.description,
            pageCount: book.pageCount,
            coverUrl: book.coverUrl,
            readingStatus: book.readingStatus,
            isLiked: book.isLiked
          };
        } else {
          this.errorMessage = result.messages?.join(', ') || 'Livro não encontrado.';
        }
        this.loading = false;
      },
      error: (err) => {
        this.errorMessage = err.error?.messages?.join(', ') || 'Erro de conexão com o servidor.';
        this.loading = false;
      }
    });
  }

  authorDropdownOpen = false;
  authorSearchTerm = '';
  newAuthorName = '';

  get filteredAuthors(): Author[] {
    if (!this.authorSearchTerm.trim()) return this.authors;
    const term = this.authorSearchTerm.toLowerCase();
    return this.authors.filter(a => a.name.toLowerCase().includes(term));
  }

  get selectedAuthors(): Author[] {
    return this.authors.filter(a => this.model.authorIds.includes(a.id));
  }

  @HostListener('document:click', ['$event'])
  closeDropdown(event: Event): void {
    const target = event.target as HTMLElement;
    if (this.authorDropdownOpen && !target.closest('.author-select')) {
      this.authorDropdownOpen = false;
    }
  }

  toggleAuthor(authorId: string): void {
    const idx = this.model.authorIds.indexOf(authorId);
    if (idx >= 0) {
      this.model.authorIds.splice(idx, 1);
    } else {
      this.model.authorIds.push(authorId);
    }
  }

  removeAuthor(authorId: string): void {
    this.model.authorIds = this.model.authorIds.filter(id => id !== authorId);
  }

  selectedAuthorNames(): string {
    const names = this.authors.filter(a => this.model.authorIds.includes(a.id)).map(a => a.name);
    return names.length > 0 ? names.join(', ') : 'Selecionar autores';
  }

  addNewAuthor(): void {
    const name = this.newAuthorName.trim();
    if (!name) return;
    this.authorService.create({ name }).subscribe({
      next: (result) => {
        if (result.valid) {
          this.authors.push(result.data);
          this.model.authorIds.push(result.data.id);
          this.newAuthorName = '';
        } else {
          this.errorMessage = result.messages?.join(', ') || 'Erro ao adicionar autor.';
        }
      },
      error: (err) => {
        this.errorMessage = err.error?.messages?.join(', ') || 'Erro de conexão.';
      }
    });
  }

  onSubmit(): void {
    this.submitting = true;
    this.errorMessage = '';
    this.successMessage = '';

    const payload = { ...this.model };
    if (!payload.isbn) payload.isbn = null;
    if (!payload.pageCount) payload.pageCount = null;

    if (this.isEditMode && this.bookId) {
      const dto: UpdateBookDto = payload;
      this.bookService.update(this.bookId, dto).subscribe({
        next: (result) => {
          if (result.valid) {
            this.successMessage = 'Livro atualizado com sucesso!';
            setTimeout(() => this.router.navigate(['/books', this.bookId]), 1500);
          } else {
            this.errorMessage = result.messages?.join(', ') || 'Erro ao atualizar.';
          }
          this.submitting = false;
        },
        error: (err) => {
          this.errorMessage = err.error?.messages?.join(', ') || 'Erro de conexão com o servidor.';
          this.submitting = false;
        }
      });
    } else {
      this.bookService.create(payload).subscribe({
        next: (result) => {
          if (result.valid) {
            this.successMessage = 'Livro cadastrado com sucesso!';
            setTimeout(() => this.router.navigate(['/books', result.data.id]), 1500);
          } else {
            this.errorMessage = result.messages?.join(', ') || 'Erro ao cadastrar.';
          }
          this.submitting = false;
        },
        error: (err) => {
          this.errorMessage = err.error?.messages?.join(', ') || 'Erro de conexão com o servidor.';
          this.submitting = false;
        }
      });
    }
  }
}
