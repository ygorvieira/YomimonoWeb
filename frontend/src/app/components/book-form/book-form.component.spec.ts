import { ComponentFixture, TestBed } from '@angular/core/testing';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { RouterTestingModule } from '@angular/router/testing';
import { BookFormComponent } from './book-form.component';
import { BookService } from '../../services/book.service';
import { AuthorService } from '../../services/author.service';
import { GenreService } from '../../services/genre.service';
import { of } from 'rxjs';
import { ActivatedRoute } from '@angular/router';

describe('BookFormComponent', () => {
  let component: BookFormComponent;
  let fixture: ComponentFixture<BookFormComponent>;
  let bookService: jasmine.SpyObj<BookService>;

  beforeEach(async () => {
    const bookSpy = jasmine.createSpyObj('BookService', ['create', 'getById']);
    const authorSpy = jasmine.createSpyObj('AuthorService', ['getAll', 'create']);
    const genreSpy = jasmine.createSpyObj('GenreService', ['getAll']);

    authorSpy.getAll.and.returnValue(of({ valid: true, data: [], messages: [], statusCode: 200 }));
    genreSpy.getAll.and.returnValue(of({ valid: true, data: [], messages: [], statusCode: 200 }));

    await TestBed.configureTestingModule({
      imports: [BookFormComponent, HttpClientTestingModule, RouterTestingModule],
      providers: [
        { provide: BookService, useValue: bookSpy },
        { provide: AuthorService, useValue: authorSpy },
        { provide: GenreService, useValue: genreSpy },
        { provide: ActivatedRoute, useValue: { snapshot: { paramMap: { get: () => null } } } }
      ]
    }).compileComponents();

    bookService = TestBed.inject(BookService) as jasmine.SpyObj<BookService>;
    fixture = TestBed.createComponent(BookFormComponent);
    component = fixture.componentInstance;
  });

  it('should create', () => expect(component).toBeTruthy());

  it('should be in create mode when no id', () => expect(component.isEditMode).toBeFalse());

  it('should call create on submit when in create mode', () => {
    bookService.create.and.returnValue(of({
      valid: true,
      data: {
        id: '1', title: 'Test', authorIds: ['a1'], authorNames: ['Author'],
        organizerIds: [], organizerNames: [],
        isbn: '123', publicationYear: 2024, publisher: 'Pub',
        genreIds: ['g1'], genreNames: ['Ficção'], pageCount: 100,
        description: null, coverUrl: null, readingStatus: null, isLiked: false,
        reReadCount: 0, createdAt: '', updatedAt: ''
      },
      messages: ['Livro cadastrado com sucesso.'],
      statusCode: 201
    }));

    component.onSubmit();
    expect(bookService.create).toHaveBeenCalled();
  });

  it('should toggle author selection', () => {
    component.toggleAuthor('a1');
    expect(component.model.authorIds).toContain('a1');
    component.toggleAuthor('a1');
    expect(component.model.authorIds).not.toContain('a1');
  });

  it('should remove author', () => {
    component.model.authorIds = ['a1', 'a2'];
    component.removeAuthor('a1');
    expect(component.model.authorIds).toEqual(['a2']);
  });

  it('should toggle organizer selection', () => {
    component.toggleOrganizer('o1');
    expect(component.model.organizerIds).toContain('o1');
    component.toggleOrganizer('o1');
    expect(component.model.organizerIds).not.toContain('o1');
  });

  it('should add new author and select it', () => {
    const authorSpy = TestBed.inject(AuthorService) as jasmine.SpyObj<AuthorService>;
    authorSpy.create.and.returnValue(of({
      valid: true,
      data: { id: 'new1', name: 'Novo Autor', createdAt: '', updatedAt: '' },
      messages: [],
      statusCode: 201
    }));

    component.newAuthorName = 'Novo Autor';
    component.addNewAuthor();

    expect(authorSpy.create).toHaveBeenCalledWith({ name: 'Novo Autor' });
    expect(component.authors.some(a => a.id === 'new1')).toBeTrue();
    expect(component.model.authorIds).toContain('new1');
    expect(component.newAuthorName).toBe('');
  });

  it('should not add empty author', () => {
    component.newAuthorName = '  ';
    component.addNewAuthor();
    const authorSpy = TestBed.inject(AuthorService) as jasmine.SpyObj<AuthorService>;
    expect(authorSpy.create).not.toHaveBeenCalled();
  });
});
