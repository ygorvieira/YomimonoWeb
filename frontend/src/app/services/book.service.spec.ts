import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { BookService } from './book.service';
import { CreateBookDto, UpdateBookDto, UpdateBookStatusDto, Result, Book } from '../models/book.model';

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

  afterEach(() => httpMock.verify());

  const mockBook: Book = {
    id: '1', title: 'Dom Casmurro', authorIds: ['a1'], authorNames: ['Machado'],
    organizerIds: [], organizerNames: [],
    isbn: '123', publicationYear: 1899, publisher: 'Garnier',
    genreIds: ['g1'], genreNames: ['Romance'], pageCount: 256,
    coverUrl: null, description: null, readingStatus: null, isLiked: false,
    reReadCount: 0, createdAt: '', updatedAt: ''
  };

  it('should get all books', () => {
    const mockResult: Result<Book[]> = { valid: true, data: [mockBook], messages: [], statusCode: 200 };

    service.getAll().subscribe(result => {
      expect(result.valid).toBeTrue();
      expect(result.data.length).toBe(1);
    });

    const req = httpMock.expectOne(`${(service as any).apiUrl}`);
    expect(req.request.method).toBe('GET');
    req.flush(mockResult);
  });

  it('should get books with filters', () => {
    service.getAll('g1', 'a1', 'Lendo').subscribe();
    const req = httpMock.expectOne(r => r.url === `${(service as any).apiUrl}`);
    expect(req.request.method).toBe('GET');
    expect(req.request.params.get('genreId')).toBe('g1');
    expect(req.request.params.get('authorId')).toBe('a1');
    expect(req.request.params.get('readingStatus')).toBe('Lendo');
    req.flush({ valid: true, data: [], messages: [], statusCode: 200 });
  });

  it('should get book by id', () => {
    service.getById('1').subscribe(result => expect(result.valid).toBeTrue());
    const req = httpMock.expectOne(`${(service as any).apiUrl}/1`);
    expect(req.request.method).toBe('GET');
    req.flush({ valid: true, data: mockBook, messages: [], statusCode: 200 });
  });

  it('should create a book', () => {
    const dto: CreateBookDto = {
      title: 'New Book', authorIds: ['a1'], isbn: '9788535902778',
      publicationYear: 2024, publisher: 'Pub', genreIds: ['g1'],
      pageCount: 100, description: null, coverUrl: null,
      readingStatus: null, isLiked: false, organizerIds: null
    };

    service.create(dto).subscribe(result => expect(result.valid).toBeTrue());
    const req = httpMock.expectOne(`${(service as any).apiUrl}`);
    expect(req.request.method).toBe('POST');
    req.flush({ valid: true, data: { ...mockBook, id: '2' }, messages: [], statusCode: 201 });
  });

  it('should update a book', () => {
    const dto: UpdateBookDto = { title: 'Updated' };
    service.update('1', dto).subscribe(result => expect(result.valid).toBeTrue());
    const req = httpMock.expectOne(`${(service as any).apiUrl}/1`);
    expect(req.request.method).toBe('PUT');
    req.flush({ valid: true, data: null, messages: [], statusCode: 200 });
  });

  it('should update book status', () => {
    const dto: UpdateBookStatusDto = { readingStatus: 'Lendo', isLiked: true };
    service.updateStatus('1', dto).subscribe(result => expect(result.valid).toBeTrue());
    const req = httpMock.expectOne(`${(service as any).apiUrl}/1/status`);
    expect(req.request.method).toBe('PATCH');
    req.flush({ valid: true, data: mockBook, messages: [], statusCode: 200 });
  });

  it('should delete a book', () => {
    service.delete('1').subscribe(result => expect(result.valid).toBeTrue());
    const req = httpMock.expectOne(`${(service as any).apiUrl}/1`);
    expect(req.request.method).toBe('DELETE');
    req.flush({ valid: true, data: true, messages: [], statusCode: 200 });
  });
});
