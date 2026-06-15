import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { BookService } from '../../services/book.service';
import { Book } from '../../models/book.model';

@Component({
  selector: 'app-book-detail',
  standalone: true,
  imports: [CommonModule, RouterModule, FormsModule],
  templateUrl: './book-detail.component.html'
})
export class BookDetailComponent implements OnInit {
  book: Book | null = null;
  loading = false;
  errorMessage = '';

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private bookService: BookService
  ) {}

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) this.loadBook(id);
  }

  loadBook(id: string): void {
    this.loading = true;
    this.errorMessage = '';
    this.bookService.getById(id).subscribe({
      next: (result) => {
        if (result.valid) {
          this.book = result.data;
        } else {
          this.errorMessage = result.messages?.join(', ') || 'Livro não encontrado.';
        }
        this.loading = false;
      },
      error: () => {
        this.errorMessage = 'Erro de conexão com o servidor.';
        this.loading = false;
      }
    });
  }

  toggleLike(): void {
    if (!this.book) return;
    this.bookService.updateStatus(this.book.id, { isLiked: !this.book.isLiked }).subscribe({
      next: (result) => {
        if (result.valid && this.book) this.book.isLiked = result.data.isLiked;
      }
    });
  }

  updateReadingStatus(status: string | null): void {
    if (!this.book) return;
    this.bookService.updateStatus(this.book.id, { readingStatus: status }).subscribe({
      next: (result) => {
        if (result.valid && this.book) this.book.readingStatus = result.data.readingStatus;
      }
    });
  }

  deleteBook(id: string): void {
    if (confirm('Tem certeza que deseja excluir este livro?')) {
      this.bookService.delete(id).subscribe({
        next: (result) => {
          if (result.valid) this.router.navigate(['/books']);
        }
      });
    }
  }
}
