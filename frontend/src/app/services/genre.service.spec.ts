import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { GenreService } from './genre.service';
import { CreateGenreDto, UpdateGenreDto, Result, Genre } from '../models/genre.model';

describe('GenreService', () => {
  let service: GenreService;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [GenreService]
    });
    service = TestBed.inject(GenreService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => httpMock.verify());

  it('should get all genres', () => {
    const mockResult: Result<Genre[]> = {
      valid: true,
      data: [{ id: '1', name: 'Romance', createdAt: '', updatedAt: '' }],
      messages: [], statusCode: 200
    };

    service.getAll().subscribe(r => expect(r.valid).toBeTrue());
    const req = httpMock.expectOne(`${(service as any).apiUrl}`);
    expect(req.request.method).toBe('GET');
    req.flush(mockResult);
  });

  it('should get genre by id', () => {
    service.getById('1').subscribe(r => expect(r.valid).toBeTrue());
    const req = httpMock.expectOne(`${(service as any).apiUrl}/1`);
    expect(req.request.method).toBe('GET');
    req.flush({ valid: true, data: { id: '1', name: 'Romance', createdAt: '', updatedAt: '' }, messages: [], statusCode: 200 });
  });

  it('should create genre', () => {
    const dto: CreateGenreDto = { name: 'Romance' };
    service.create(dto).subscribe(r => expect(r.valid).toBeTrue());
    const req = httpMock.expectOne(`${(service as any).apiUrl}`);
    expect(req.request.method).toBe('POST');
    req.flush({ valid: true, data: { id: '1', name: 'Romance', createdAt: '', updatedAt: '' }, messages: [], statusCode: 201 });
  });

  it('should update genre', () => {
    const dto: UpdateGenreDto = { name: 'Updated' };
    service.update('1', dto).subscribe(r => expect(r.valid).toBeTrue());
    const req = httpMock.expectOne(`${(service as any).apiUrl}/1`);
    expect(req.request.method).toBe('PUT');
    req.flush({ valid: true, data: null, messages: [], statusCode: 200 });
  });

  it('should delete genre', () => {
    service.delete('1').subscribe(r => expect(r.valid).toBeTrue());
    const req = httpMock.expectOne(`${(service as any).apiUrl}/1`);
    expect(req.request.method).toBe('DELETE');
    req.flush({ valid: true, data: true, messages: [], statusCode: 200 });
  });
});
