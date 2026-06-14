import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Book, CreateBookDto, UpdateBookDto, Result } from '../models/book.model';
import { environment } from '../../environments/environment';

@Injectable({ providedIn: 'root' })
export class BookService {
  private readonly apiUrl = `${environment.apiUrl}/api/books`;

  constructor(private http: HttpClient) {}

  getAll(): Observable<Result<Book[]>> {
    return this.http.get<Result<Book[]>>(this.apiUrl);
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

  delete(id: string): Observable<Result<boolean>> {
    return this.http.delete<Result<boolean>>(`${this.apiUrl}/${id}`);
  }
}
