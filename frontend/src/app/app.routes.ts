import { Routes } from '@angular/router';
import { BookListComponent } from './components/book-list/book-list.component';
import { ReportComponent } from './components/report/report.component';
import { BookDetailComponent } from './components/book-detail/book-detail.component';
import { BookFormComponent } from './components/book-form/book-form.component';
import { LoginComponent } from './components/login/login.component';
import { RegisterComponent } from './components/register/register.component';
import { AuthorListComponent } from './components/author-list/author-list.component';
import { AuthorFormComponent } from './components/author-form/author-form.component';
import { GenreListComponent } from './components/genre-list/genre-list.component';
import { GenreFormComponent } from './components/genre-form/genre-form.component';
import { authGuard } from './guards/auth.guard';

export const routes: Routes = [
  { path: '', redirectTo: '/books', pathMatch: 'full' },
  { path: 'login', component: LoginComponent },
  { path: 'register', component: RegisterComponent },
  { path: 'books', component: BookListComponent, canActivate: [authGuard] },
  { path: 'books/new', component: BookFormComponent, canActivate: [authGuard] },
  { path: 'books/:id', component: BookDetailComponent, canActivate: [authGuard] },
  { path: 'books/:id/edit', component: BookFormComponent, canActivate: [authGuard] },
  { path: 'authors', component: AuthorListComponent, canActivate: [authGuard] },
  { path: 'authors/new', component: AuthorFormComponent, canActivate: [authGuard] },
  { path: 'authors/:id/edit', component: AuthorFormComponent, canActivate: [authGuard] },
  { path: 'genres', component: GenreListComponent, canActivate: [authGuard] },
  { path: 'genres/new', component: GenreFormComponent, canActivate: [authGuard] },
  { path: 'genres/:id/edit', component: GenreFormComponent, canActivate: [authGuard] },
  { path: 'reports', component: ReportComponent, canActivate: [authGuard] }
];
