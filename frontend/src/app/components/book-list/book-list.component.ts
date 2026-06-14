import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { BookService } from '../../services/book.service';
import { Book } from '../../models/book.model';

@Component({
  selector: 'app-book-list',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './book-list.component.html'
})
export class BookListComponent implements OnInit {
  books: Book[] = [];
  loading = false;
  errorMessage = '';

  constructor(private bookService: BookService) {}

  ngOnInit(): void {
    this.loadBooks();
  }

  loadBooks(): void {
    this.loading = true;
    this.errorMessage = '';
    this.bookService.getAll().subscribe({
      next: (result) => {
        if (result.valid) {
          this.books = result.data;
        } else {
          this.errorMessage = result.messages?.join(', ') || 'Erro ao carregar livros.';
        }
        this.loading = false;
      },
      error: () => {
        this.errorMessage = 'Erro de conexão com o servidor.';
        this.loading = false;
      }
    });
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
}
