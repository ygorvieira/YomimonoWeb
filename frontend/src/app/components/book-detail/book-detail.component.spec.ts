import { ComponentFixture, TestBed } from '@angular/core/testing';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { RouterTestingModule } from '@angular/router/testing';
import { BookDetailComponent } from './book-detail.component';
import { BookService } from '../../services/book.service';
import { of } from 'rxjs';
import { ActivatedRoute } from '@angular/router';

describe('BookDetailComponent', () => {
  let component: BookDetailComponent;
  let fixture: ComponentFixture<BookDetailComponent>;
  let bookService: jasmine.SpyObj<BookService>;

  beforeEach(async () => {
    const spy = jasmine.createSpyObj('BookService', ['getById']);

    await TestBed.configureTestingModule({
      imports: [BookDetailComponent, HttpClientTestingModule, RouterTestingModule],
      providers: [
        { provide: BookService, useValue: spy },
        { provide: ActivatedRoute, useValue: { snapshot: { paramMap: { get: () => '1' } } } }
      ]
    }).compileComponents();

    bookService = TestBed.inject(BookService) as jasmine.SpyObj<BookService>;
    fixture = TestBed.createComponent(BookDetailComponent);
    component = fixture.componentInstance;
  });

  it('should create', () => expect(component).toBeTruthy());

  it('should load book on init', () => {
    bookService.getById.and.returnValue(of({
      valid: true,
      data: {
        id: '1', title: 'Dom Casmurro', authorIds: ['a1'], authorNames: ['Machado de Assis'],
        organizerIds: [], organizerNames: [],
        publicationYear: 1899, publisher: 'Garnier',
        genreIds: ['g1'], genreNames: ['Romance'], pageCount: 256,
        description: null, coverUrl: null, readingStatus: null, isLiked: false,
        isTradePaperback: false, isDigital: false, reReadCount: 0, createdAt: '', updatedAt: ''
      },
      messages: [],
      statusCode: 200
    }));

    fixture.detectChanges();
    expect(component.book).toBeTruthy();
    expect(component.book!.title).toBe('Dom Casmurro');
  });

  it('should show error when book not found', () => {
    bookService.getById.and.returnValue(of({
      valid: false,
      data: null as any,
      messages: ['Livro não encontrado.'],
      statusCode: 404
    }));

    fixture.detectChanges();
    expect(component.errorMessage).toBe('Livro não encontrado.');
  });
});
