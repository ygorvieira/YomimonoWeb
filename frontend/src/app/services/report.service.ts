import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ReportData } from '../models/report.model';
import { Result } from '../models/book.model';
import { environment } from '../../environments/environment';

@Injectable({ providedIn: 'root' })
export class ReportService {
  private readonly apiUrl = `${environment.apiUrl}/api/reports`;

  constructor(private http: HttpClient) {}

  getReports(): Observable<Result<ReportData>> {
    return this.http.get<Result<ReportData>>(this.apiUrl);
  }
}
