import { ComponentFixture, TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { RouterTestingModule } from '@angular/router/testing';
import { BookListComponent } from './book-list.component';
import { BookService } from '../../services/book.service';
import { Result, Book } from '../../models/book.model';
import { of } from 'rxjs';

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
        genreIds: ['g1'], genreNames: ['Romance'], pageCount: 256,
        description: null, coverUrl: null, readingStatus: null, isLiked: false,
        reReadCount: 0, createdAt: '2024-01-01', updatedAt: '2024-01-01'
      }],
      messages: [],
      statusCode: 200
    };

    req.flush(mockResult);
    expect(component.books.length).toBe(1);
  });

  it('should cycle reading status', () => {
    const book: Book = {
      id: '1', title: 'Test', authorIds: ['a1'], authorNames: ['Author'],
      isbn: '123', publicationYear: 2024, publisher: 'Pub',
      genreIds: ['g1'], genreNames: ['Genre'], pageCount: 100,
      description: null, coverUrl: null, readingStatus: null, isLiked: false,
      reReadCount: 0, createdAt: '', updatedAt: ''
    };

    fixture.detectChanges();
    const req0 = httpMock.expectOne(`${(service as any).apiUrl}`);
    req0.flush({ valid: true, data: [book], messages: [], statusCode: 200 });

    component.cycleStatus(book);
    const req1 = httpMock.expectOne(`${(service as any).apiUrl}/1/status`);
    expect(req1.request.method).toBe('PATCH');
    expect(req1.request.body).toEqual({ readingStatus: 'Lendo' });
    req1.flush({ valid: true, data: { ...book, readingStatus: 'Lendo' }, messages: [], statusCode: 200 });

    expect(book.readingStatus).toBe('Lendo');
  });
});
