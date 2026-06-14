import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { BookService } from './book.service';
import { CreateBookDto, UpdateBookDto, Result, Book } from '../models/book.model';

describe('BookService', () => {
  let service: BookService;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [BookService]
    });
    service = TestBed.inject(BookService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should get all books', () => {
    const mockResult: Result<Book[]> = {
      valid: true,
      data: [{ id: '1', title: 'Dom Casmurro', author: 'Machado', isbn: '123', publicationYear: 1899, publisher: 'Garnier', genre: 'Romance', pageCount: 256, coverUrl: null, description: null, createdAt: '', updatedAt: '' }],
      messages: [],
      statusCode: 200
    };

    service.getAll().subscribe(result => {
      expect(result.valid).toBeTrue();
      expect(result.data.length).toBe(1);
    });

    const req = httpMock.expectOne(`${(service as any).apiUrl}`);
    expect(req.request.method).toBe('GET');
    req.flush(mockResult);
  });

  it('should get book by id', () => {
    const mockResult: Result<Book> = {
      valid: true,
      data: { id: '1', title: 'Dom Casmurro', author: 'Machado', isbn: '123', publicationYear: 1899, publisher: 'Garnier', genre: 'Romance', pageCount: 256, coverUrl: null, description: null, createdAt: '', updatedAt: '' },
      messages: [],
      statusCode: 200
    };

    service.getById('1').subscribe(result => {
      expect(result.valid).toBeTrue();
    });

    const req = httpMock.expectOne(`${(service as any).apiUrl}/1`);
    expect(req.request.method).toBe('GET');
    req.flush(mockResult);
  });

  it('should create a book', () => {
    const dto: CreateBookDto = {
      title: 'New Book', author: 'Author', isbn: '9788535902778',
      publicationYear: 2024, publisher: 'Pub', genre: 'Ficção',
      pageCount: 100, description: null, coverUrl: null
    };

    service.create(dto).subscribe(result => {
      expect(result.valid).toBeTrue();
    });

    const req = httpMock.expectOne(`${(service as any).apiUrl}`);
    expect(req.request.method).toBe('POST');
    req.flush({ valid: true, data: { id: '1', ...dto, createdAt: '', updatedAt: '' }, messages: [], statusCode: 201 });
  });

  it('should update a book', () => {
    const dto: UpdateBookDto = { title: 'Updated' };

    service.update('1', dto).subscribe(result => {
      expect(result.valid).toBeTrue();
    });

    const req = httpMock.expectOne(`${(service as any).apiUrl}/1`);
    expect(req.request.method).toBe('PUT');
    req.flush({ valid: true, data: null, messages: [], statusCode: 200 });
  });

  it('should delete a book', () => {
    service.delete('1').subscribe(result => {
      expect(result.valid).toBeTrue();
    });

    const req = httpMock.expectOne(`${(service as any).apiUrl}/1`);
    expect(req.request.method).toBe('DELETE');
    req.flush({ valid: true, data: true, messages: [], statusCode: 200 });
  });
});
