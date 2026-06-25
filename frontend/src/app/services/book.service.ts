import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Book, CreateBookDto, UpdateBookDto, UpdateBookStatusDto, Result } from '../models/book.model';
import { PagedResult } from '../models/paged-result.model';
import { environment } from '../../environments/environment';

@Injectable({ providedIn: 'root' })
export class BookService {
  private readonly apiUrl = `${environment.apiUrl}/api/books`;

  constructor(private http: HttpClient) {}

  getAll(genreId?: string, authorId?: string, readingStatus?: string, searchTerm?: string, page?: number, pageSize?: number): Observable<Result<PagedResult<Book>>> {
    let params = new HttpParams();
    if (genreId) params = params.set('genreId', genreId);
    if (authorId) params = params.set('authorId', authorId);
    if (readingStatus) params = params.set('readingStatus', readingStatus);
    if (searchTerm) params = params.set('searchTerm', searchTerm);
    if (page) params = params.set('page', page);
    if (pageSize) params = params.set('pageSize', pageSize);
    return this.http.get<Result<PagedResult<Book>>>(this.apiUrl, { params });
  }

  getById(id: string): Observable<Result<Book>> {
    return this.http.get<Result<Book>>(`${this.apiUrl}/${id}`);
  }

  create(dto: CreateBookDto): Observable<Result<Book>> {
    return this.http.post<Result<Book>>(this.apiUrl, dto);
  }

  update(id: string, dto: UpdateBookDto): Observable<Result<Book>> {
    return this.http.put<Result<Book>>(`${this.apiUrl}/${id}`, dto);
  }

  updateStatus(id: string, dto: UpdateBookStatusDto): Observable<Result<Book>> {
    return this.http.patch<Result<Book>>(`${this.apiUrl}/${id}/status`, dto);
  }

  delete(id: string): Observable<Result<boolean>> {
    return this.http.delete<Result<boolean>>(`${this.apiUrl}/${id}`);
  }
}
