import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { AuthorService } from './author.service';
import { CreateAuthorDto, UpdateAuthorDto, Result, Author } from '../models/author.model';

describe('AuthorService', () => {
  let service: AuthorService;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [AuthorService]
    });
    service = TestBed.inject(AuthorService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => httpMock.verify());

  it('should get all authors', () => {
    const mockResult: Result<Author[]> = {
      valid: true,
      data: [{ id: '1', name: 'Machado de Assis', createdAt: '', updatedAt: '' }],
      messages: [], statusCode: 200
    };

    service.getAll().subscribe(r => expect(r.valid).toBeTrue());
    const req = httpMock.expectOne(`${(service as any).apiUrl}`);
    expect(req.request.method).toBe('GET');
    req.flush(mockResult);
  });

  it('should get author by id', () => {
    service.getById('1').subscribe(r => expect(r.valid).toBeTrue());
    const req = httpMock.expectOne(`${(service as any).apiUrl}/1`);
    expect(req.request.method).toBe('GET');
    req.flush({ valid: true, data: { id: '1', name: 'Machado', createdAt: '', updatedAt: '' }, messages: [], statusCode: 200 });
  });

  it('should create author', () => {
    const dto: CreateAuthorDto = { name: 'Machado de Assis' };
    service.create(dto).subscribe(r => expect(r.valid).toBeTrue());
    const req = httpMock.expectOne(`${(service as any).apiUrl}`);
    expect(req.request.method).toBe('POST');
    req.flush({ valid: true, data: { id: '1', name: 'Machado', createdAt: '', updatedAt: '' }, messages: [], statusCode: 201 });
  });

  it('should update author', () => {
    const dto: UpdateAuthorDto = { name: 'Updated' };
    service.update('1', dto).subscribe(r => expect(r.valid).toBeTrue());
    const req = httpMock.expectOne(`${(service as any).apiUrl}/1`);
    expect(req.request.method).toBe('PUT');
    req.flush({ valid: true, data: null, messages: [], statusCode: 200 });
  });

  it('should delete author', () => {
    service.delete('1').subscribe(r => expect(r.valid).toBeTrue());
    const req = httpMock.expectOne(`${(service as any).apiUrl}/1`);
    expect(req.request.method).toBe('DELETE');
    req.flush({ valid: true, data: true, messages: [], statusCode: 200 });
  });
});
