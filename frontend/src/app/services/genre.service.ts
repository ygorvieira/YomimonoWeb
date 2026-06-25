import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Genre, CreateGenreDto, UpdateGenreDto, Result } from '../models/genre.model';
import { environment } from '../../environments/environment';

@Injectable({ providedIn: 'root' })
export class GenreService {
  private readonly apiUrl = `${environment.apiUrl}/api/genres`;

  constructor(private http: HttpClient) {}

  getAll(searchTerm?: string): Observable<Result<Genre[]>> {
    let params = new HttpParams();
    if (searchTerm) params = params.set('searchTerm', searchTerm);
    return this.http.get<Result<Genre[]>>(this.apiUrl, { params });
  }

  getById(id: string): Observable<Result<Genre>> {
    return this.http.get<Result<Genre>>(`${this.apiUrl}/${id}`);
  }

  create(dto: CreateGenreDto): Observable<Result<Genre>> {
    return this.http.post<Result<Genre>>(this.apiUrl, dto);
  }

  update(id: string, dto: UpdateGenreDto): Observable<Result<Genre>> {
    return this.http.put<Result<Genre>>(`${this.apiUrl}/${id}`, dto);
  }

  delete(id: string): Observable<Result<boolean>> {
    return this.http.delete<Result<boolean>>(`${this.apiUrl}/${id}`);
  }
}
