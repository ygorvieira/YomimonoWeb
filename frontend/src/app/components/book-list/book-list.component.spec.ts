import { ComponentFixture, TestBed } from '@angular/core/testing';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { RouterTestingModule } from '@angular/router/testing';
import { BookListComponent } from './book-list.component';
import { BookService } from '../../services/book.service';
import { of } from 'rxjs';
import { Book } from '../../models/book.model';

describe('BookListComponent', () => {
  let component: BookListComponent;
  let fixture: ComponentFixture<BookListComponent>;
  let bookService: jasmine.SpyObj<BookService>;

  const mockBooks: Book[] = [
    { id: '1', title: 'Dom Casmurro', author: 'Machado de Assis', isbn: '9788535902778', publicationYear: 1899, publisher: 'Garnier', genre: 'Romance', pageCount: 256, description: null, coverUrl: null, createdAt: '2024-01-01', updatedAt: '2024-01-01' }
  ];

  beforeEach(async () => {
    const spy = jasmine.createSpyObj('BookService', ['getAll', 'delete']);

    await TestBed.configureTestingModule({
      imports: [BookListComponent, HttpClientTestingModule, RouterTestingModule],
      providers: [{ provide: BookService, useValue: spy }]
    }).compileComponents();

    bookService = TestBed.inject(BookService) as jasmine.SpyObj<BookService>;
    fixture = TestBed.createComponent(BookListComponent);
    component = fixture.componentInstance;
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should load books on init', () => {
    bookService.getAll.and.returnValue(of({ valid: true, data: mockBooks, messages: [], statusCode: 200 }));

    fixture.detectChanges();

    expect(component.books.length).toBe(1);
    expect(component.books[0].title).toBe('Dom Casmurro');
  });

  it('should display error message when API returns invalid', () => {
    bookService.getAll.and.returnValue(of({ valid: false, data: [], messages: ['Erro ao carregar'], statusCode: 400 }));

    fixture.detectChanges();

    expect(component.errorMessage).toBe('Erro ao carregar');
  });
});
