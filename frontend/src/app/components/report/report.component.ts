import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { ReportService } from '../../services/report.service';
import { ReportData } from '../../models/report.model';

@Component({
  selector: 'app-report',
  standalone: true,
  imports: [CommonModule, RouterModule],
  template: `
    <div class="container">
      <a routerLink="/books" class="btn-back">Voltar</a>
      <h1>Relatórios</h1>

      <div *ngIf="loading" class="loading">Carregando...</div>
      <div *ngIf="errorMessage" class="error">{{ errorMessage }}</div>

      <div *ngIf="data" class="report-grid">
        <div class="report-card total-books">
          <h3>Total de Livros</h3>
          <span class="report-value">{{ data.totalBooks }}</span>
        </div>

        <div class="report-card total-read">
          <h3>Total Lido</h3>
          <span class="report-value">{{ data.totalRead }}</span>
        </div>

        <div class="report-table-section">
          <h3>Livros por Categoria</h3>
          <table *ngIf="data.booksByGenre.length > 0">
            <thead>
              <tr>
                <th>Categoria</th>
                <th>Livros</th>
                <th>Likes</th>
              </tr>
            </thead>
            <tbody>
              <tr *ngFor="let item of data.booksByGenre">
                <td>{{ item.genreName }}</td>
                <td>{{ item.bookCount }}</td>
                <td>{{ item.likeCount }}</td>
              </tr>
            </tbody>
          </table>
          <p *ngIf="data.booksByGenre.length === 0" class="empty">Nenhum dado disponível.</p>
        </div>

        <div class="report-table-section">
          <h3>Categorias com Mais Likes</h3>
          <table *ngIf="data.genresByLikes.length > 0">
            <thead>
              <tr>
                <th>Categoria</th>
                <th>Likes</th>
                <th>Livros</th>
              </tr>
            </thead>
            <tbody>
              <tr *ngFor="let item of data.genresByLikes">
                <td>{{ item.genreName }}</td>
                <td>{{ item.likeCount }}</td>
                <td>{{ item.bookCount }}</td>
              </tr>
            </tbody>
          </table>
          <p *ngIf="data.genresByLikes.length === 0" class="empty">Nenhum dado disponível.</p>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .report-grid {
      display: grid;
      grid-template-columns: 1fr 1fr;
      gap: 16px;
      margin-top: 16px;
    }
    .report-card {
      background: #fff;
      border-radius: 8px;
      padding: 24px;
      text-align: center;
      box-shadow: 0 1px 3px rgba(0,0,0,0.1);
    }
    .report-card h3 {
      margin: 0 0 8px;
      font-size: 14px;
      color: #666;
    }
    .report-value {
      font-size: 36px;
      font-weight: 700;
      color: #2c3e50;
    }
    .report-table-section {
      grid-column: 1 / -1;
      background: #fff;
      border-radius: 8px;
      padding: 16px;
      box-shadow: 0 1px 3px rgba(0,0,0,0.1);
    }
    .report-table-section h3 {
      margin: 0 0 12px;
      font-size: 16px;
    }
    .report-table-section table {
      width: 100%;
      border-collapse: collapse;
    }
    .report-table-section th,
    .report-table-section td {
      text-align: left;
      padding: 8px;
      border-bottom: 1px solid #eee;
    }
    .report-table-section th {
      font-weight: 600;
      color: #666;
      font-size: 13px;
    }
  `]
})
export class ReportComponent implements OnInit {
  data: ReportData | null = null;
  loading = false;
  errorMessage = '';

  constructor(private reportService: ReportService) {}

  ngOnInit(): void {
    this.loadReports();
  }

  loadReports(): void {
    this.loading = true;
    this.errorMessage = '';
    this.reportService.getReports().subscribe({
      next: (result) => {
        if (result.valid) {
          this.data = result.data;
        } else {
          this.errorMessage = result.messages?.join(', ') || 'Erro ao carregar relatórios.';
        }
        this.loading = false;
      },
      error: (err) => {
        this.errorMessage = err.error?.messages?.join(', ') || 'Erro de conexão com o servidor.';
        this.loading = false;
      }
    });
  }
}
