import { ComponentFixture, TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { RouterTestingModule } from '@angular/router/testing';
import { BookListComponent } from './book-list.component';
import { BookService } from '../../services/book.service';
import { Result, Book } from '../../models/book.model';

describe('BookListComponent', () => {
  let component: BookListComponent;
  let fixture: ComponentFixture<BookListComponent>;
  let httpMock: HttpTestingController;
  let service: BookService;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [BookListComponent, HttpClientTestingModule, RouterTestingModule]
    }).compileComponents();

    fixture = TestBed.createComponent(BookListComponent);
    component = fixture.componentInstance;
    httpMock = TestBed.inject(HttpTestingController);
    service = TestBed.inject(BookService);
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should load books on init', () => {
    fixture.detectChanges();

    const req = httpMock.expectOne(`${(service as any).apiUrl}`);
    expect(req.request.method).toBe('GET');

    const mockResult: Result<Book[]> = {
      valid: true,
      data: [{
        id: '1', title: 'Dom Casmurro', authorIds: ['a1'], authorNames: ['Machado de Assis'],
        isbn: '9788535902778', publicationYear: 1899, publisher: 'Garnier',
        genreId: 'g1', genreName: 'Romance', pageCount: 256,
        description: null, coverUrl: null, readingStatus: null, isLiked: false,
        createdAt: '2024-01-01', updatedAt: '2024-01-01'
      }],
      messages: [],
      statusCode: 200
    };

    req.flush(mockResult);
    expect(component.books.length).toBe(1);
  });
});
