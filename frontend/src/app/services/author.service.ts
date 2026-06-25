import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Author, CreateAuthorDto, UpdateAuthorDto, Result } from '../models/author.model';
import { environment } from '../../environments/environment';

@Injectable({ providedIn: 'root' })
export class AuthorService {
  private readonly apiUrl = `${environment.apiUrl}/api/authors`;

  constructor(private http: HttpClient) {}

  getAll(searchTerm?: string): Observable<Result<Author[]>> {
    let params = new HttpParams();
    if (searchTerm) params = params.set('searchTerm', searchTerm);
    return this.http.get<Result<Author[]>>(this.apiUrl, { params });
  }

  getById(id: string): Observable<Result<Author>> {
    return this.http.get<Result<Author>>(`${this.apiUrl}/${id}`);
  }

  create(dto: CreateAuthorDto): Observable<Result<Author>> {
    return this.http.post<Result<Author>>(this.apiUrl, dto);
  }

  update(id: string, dto: UpdateAuthorDto): Observable<Result<Author>> {
    return this.http.put<Result<Author>>(`${this.apiUrl}/${id}`, dto);
  }

  delete(id: string): Observable<Result<boolean>> {
    return this.http.delete<Result<boolean>>(`${this.apiUrl}/${id}`);
  }
}
