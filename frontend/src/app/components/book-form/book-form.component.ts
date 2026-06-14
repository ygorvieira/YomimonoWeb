import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { BookService } from '../../services/book.service';
import { CreateBookDto, UpdateBookDto } from '../../models/book.model';

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

  model: CreateBookDto = {
    title: '',
    author: '',
    isbn: '',
    publicationYear: new Date().getFullYear(),
    publisher: '',
    genre: '',
    description: null,
    pageCount: 0,
    coverUrl: null
  };

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private bookService: BookService
  ) {}

  ngOnInit(): void {
    this.bookId = this.route.snapshot.paramMap.get('id');
    if (this.bookId) {
      this.isEditMode = true;
      this.loadBook(this.bookId);
    }
  }

  loadBook(id: string): void {
    this.loading = true;
    this.bookService.getById(id).subscribe({
      next: (result) => {
        if (result.valid) {
          const book = result.data;
          this.model = {
            title: book.title,
            author: book.author,
            isbn: book.isbn,
            publicationYear: book.publicationYear,
            publisher: book.publisher,
            genre: book.genre,
            description: book.description,
            pageCount: book.pageCount,
            coverUrl: book.coverUrl
          };
        } else {
          this.errorMessage = 'Livro não encontrado.';
        }
        this.loading = false;
      },
      error: () => {
        this.errorMessage = 'Erro de conexão com o servidor.';
        this.loading = false;
      }
    });
  }

  onSubmit(): void {
    this.submitting = true;
    this.errorMessage = '';
    this.successMessage = '';

    if (this.isEditMode && this.bookId) {
      const dto: UpdateBookDto = { ...this.model };
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
        error: () => {
          this.errorMessage = 'Erro de conexão com o servidor.';
          this.submitting = false;
        }
      });
    } else {
      this.bookService.create(this.model).subscribe({
        next: (result) => {
          if (result.valid) {
            this.successMessage = 'Livro cadastrado com sucesso!';
            setTimeout(() => this.router.navigate(['/books', result.data.id]), 1500);
          } else {
            this.errorMessage = result.messages?.join(', ') || 'Erro ao cadastrar.';
          }
          this.submitting = false;
        },
        error: () => {
          this.errorMessage = 'Erro de conexão com o servidor.';
          this.submitting = false;
        }
      });
    }
  }
}
