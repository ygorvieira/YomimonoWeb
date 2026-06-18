---
description: "Task list for 003-reports-pagination-tracking feature"
---

# Tasks: Relatórios Avançados e Paginação

**Input**: spec.md in specs/003-reports-pagination-tracking/

**TDD**: Tests MUST be written first and fail before implementation (Constitution II).
All handler tests MUST assert `result.Valid` (Constitution V).

**Path Conventions**:
- Backend: `backend/Yomimono.Domain/`, `backend/Yomimono.Application/`, etc.
- Tests: `backend/Yomimono.Api.Tests/`
- Frontend: `frontend/src/app/`

---

## Technical Plan

### Architecture Overview

```
Novos tipos:
  Application/Common/PagedResult.cs     ← Envelope paginado genérico
  Application/Reports/DTOs/ReportDto.cs  ← Extendido com AuthorReportDto

Mudanças em contratos existentes:
  Application/Books/Queries/GetAllBooksQuery.cs  ← +Page, +PageSize
  Application/Books/Handlers/GetAllBooksQueryHandler.cs  ← retorna PagedResult<BookDto>
  Application/Books/Common/IBookRepository.cs  ← +GetAllPagedAsync
  Infrastructure/Repositories/BookRepository.cs  ← Skip/Take no SQL
  Application/Reports/Queries/GetReportsQueryHandler.cs  ← novos campos
  Api/Controllers/BooksController.cs  ← page/pageSize query params

Frontend:
  models/  ← +PagedResult<T>, ReportData atualizado
  services/  ← bookService.getAll com page/pageSize
  components/book-list/  ← paginator UI
  components/report/  ← novas seções
```

### Data Flow — Pagination

```
Browser → GET /api/books?page=2&pageSize=50&genreId=X
  → BooksController.GetAll(page, pageSize, genreId, ...)
  → MediatR → GetAllBooksQueryHandler
  → BookRepository.GetAllPagedAsync(genreId, authorId, status, page, pageSize)
  → DB: COUNT + SELECT com OFFSET/FETCH NEXT
  → PagedResult<BookDto> → Result<PagedResult<BookDto>>
  → JSON response com items + metadados
```

### Data Flow — Reports

```
Browser → GET /api/reports
  → ReportsController.Get()
  → MediatR → GetReportsQueryHandler
  → BookRepository.GetAllForReportsAsync()  ← agora inclui BookAuthors
  → DB: livros com Genres + BookAuthors + Authors
  → Cálculo: totalPagesRead, booksByAuthor, topAuthorsByLikes
  → ReportDto → Result<ReportDto>
```

---

## Phase 1: Common — PagedResult Type

**Purpose**: Tipo genérico reutilizável para respostas paginadas

- [ ] T001 Create `PagedResult<T>` in `backend/Yomimono.Application/Common/PagedResult.cs`
      ```csharp
      public record PagedResult<T>(
          List<T> Items,
          int TotalCount,
          int PageNumber,
          int PageSize,
          int TotalPages,
          bool HasNextPage,
          bool HasPrevPage
      );
      ```
      Namespace `Yomimono.Application.Common`.

---

## Phase 2: Backend — Repository Updates

**Purpose**: Adicionar suporte a paginação no repositório de books

- [ ] T002 Update `IBookRepository` in `backend/Yomimono.Application/Books/Common/IBookRepository.cs`
      Add method: `Task<PagedResult<Book>> GetAllPagedAsync(Guid? genreId, Guid? authorId, string? readingStatus, int pageNumber, int pageSize, CancellationToken cancellationToken)`
      Import `Yomimono.Application.Common`.

- [ ] T003 Update `BookRepository` in `backend/Yomimono.Infrastructure/Repositories/BookRepository.cs`
      Implement `GetAllPagedAsync`:
      - Mesma lógica de filtros do `GetAllAsync`
      - `var totalCount = await query.CountAsync(cancellationToken)`
      - `var items = await query.OrderBy(b => b.Title).Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync(cancellationToken)`
      - Calcular `totalPages = (int)Math.Ceiling(totalCount / (double)pageSize)`
      - `hasNextPage = pageNumber < totalPages`
      - `hasPrevPage = pageNumber > 1`
      - Retornar `new PagedResult<Book>(items, totalCount, pageNumber, pageSize, totalPages, hasNextPage, hasPrevPage)`

- [ ] T004 Update `BookRepository.GetAllForReportsAsync` to include BookAuthors + Authors
      Add `.Include(b => b.BookAuthors).ThenInclude(ba => ba.Author)` alongside existing `.Include(b => b.Genres).ThenInclude(bg => bg.Genre)`.

---

## Phase 3: Backend — Reports Enhancement (TDD)

**Purpose**: Extender o relatório com páginas lidas, livros por autor, autor com mais curtidas

**Pre-condition**: Write tests FIRST, ensure they FAIL, then implement.

- [ ] T005 Write/update tests for Reports handler
      File `backend/Yomimono.Api.Tests/Handlers/` (check if existing tests exist, create if not):
      - `GetReportsQueryHandlerTests.cs` — add tests for:
        - `totalPagesRead` soma pageCount de livros "Lido" + "Relido", ignora null
        - `booksByAuthor` lista autores ordenados por bookCount desc
        - `topAuthorsByLikes` lista top 10 por likeCount desc
        - `booksByAuthor` considera co-autoria (livro conta para todos)
        - Livros "Relido" contam como lido para totalPagesRead
        - Todos os testes MUST assert `result.Valid`

- [ ] T006 Add `AuthorReportDto` in `backend/Yomimono.Application/Reports/DTOs/ReportDto.cs`
      ```csharp
      public record AuthorReportDto(
          Guid AuthorId,
          string AuthorName,
          int BookCount,
          int TotalPagesRead,
          int LikeCount
      );
      ```

- [ ] T007 Update `ReportDto` in `backend/Yomimono.Application/Reports/DTOs/ReportDto.cs`
      Add new fields:
      ```csharp
      int TotalPagesRead,
      List<AuthorReportDto> BooksByAuthor,
      List<AuthorReportDto> TopAuthorsByLikes
      ```

- [ ] T008 Update `GetReportsQueryHandler` in `backend/Yomimono.Application/Reports/Queries/GetReportsQueryHandler.cs`
      After loading books, compute:
      - `totalPagesRead = bookList.Where(b => b.ReadingStatus is "Lido" or "Relido").Sum(b => b.PageCount ?? 0)`
      - `booksByAuthor`: group by `BookAuthors` (role "Author"), for each author compute:
        - `bookCount = count of distinct books`
        - `totalPagesRead = sum of PageCount for "Lido"/"Relido" books`
        - `likeCount = count of books where IsLiked`
        - Order by bookCount desc
      - `topAuthorsByLikes = booksByAuthor.OrderByDescending(a => a.LikeCount).Take(10).ToList()`

---

## Phase 4: Backend — Book Query Pagination (TDD)

**Purpose**: Atualizar GetAllBooksQuery para suportar paginação

**Pre-condition**: Write tests FIRST.

- [ ] T009 Write/update tests for GetAllBooksQueryHandler
      File `backend/Yomimono.Api.Tests/Handlers/GetAllBooksQueryHandlerTests.cs`:
      - Add test: `GetAllBooks_WithPagination_ReturnsCorrectPage` — cria 3 livros, busca page=1 pageSize=2, verifica 2 items, totalCount=3, totalPages=2, hasNextPage=true
      - Add test: `GetAllBooks_WithPagination_LastPage` — page=2 pageSize=2, verifica 1 item, hasNextPage=false
      - Add test: `GetAllBooks_WithPagination_AndFilters` — page=1 pageSize=10 com filtro genreId, verifica paginação correta
      - Add test: `GetAllBooks_WithPagination_EmptyPage` — page=99 pageSize=10, verifica items vazio, totalCount real
      - Todos os testes MUST assert `result.Valid`

- [ ] T010 Update `GetAllBooksQuery` in `backend/Yomimono.Application/Books/Queries/GetAllBooksQuery.cs`
      Add params:
      ```csharp
      int PageNumber = 1,
      int PageSize = 50
      ```
      Change return type from `Result<IEnumerable<BookDto>>` to `Result<PagedResult<BookDto>>`.

- [ ] T011 Update `GetAllBooksQueryHandler` in `backend/Yomimono.Application/Books/Handlers/GetAllBooksQueryHandler.cs`
      - Change return type to `Result<PagedResult<BookDto>>`
      - Call `repository.GetAllPagedAsync(...)` instead of `GetAllAsync(...)`
      - Map items to BookDto list
      - Return `Result<PagedResult<BookDto>>.Success(pagedResult)`
      - Keep private `MapToDto` method (unchanged)

- [ ] T012 Update `GetAllBooksQueryHandler` test file if needed (already covered in T009)

- [ ] T013 Update `IBookRepository` — already done in T002, verify import for PagedResult

---

## Phase 5: Backend — Controller Updates

- [ ] T014 Update `BooksController` in `backend/Yomimono.Api/Controllers/BooksController.cs`
      Update `GetAll` action:
      - Add `[FromQuery] int page = 1` and `[FromQuery] int pageSize = 50`
      - Validate pageSize max 100 (return `Result.Failure` if > 100)
      - Pass params to `GetAllBooksQuery`

---

## Phase 6: Backend — Integration Tests

- [ ] T015 Write integration tests for paginated books endpoint
      File `backend/Yomimono.Api.Tests/Integration/`:
      - Add test: `GET /api/books?page=1&pageSize=10` retorna paginação correta
      - Add test: `GET /api/books?pageSize=200` retorna erro (max 100)
      - Add test: `GET /api/books?page=1&pageSize=50&genreId=X` com paginação + filtro
      - Follow existing integration test patterns (CustomWebApplicationFactory, auth header)

- [ ] T016 Write integration test for updated reports endpoint
      File `backend/Yomimono.Api.Tests/Integration/`:
      - Add test: `GET /api/reports` verifica novos campos (totalPagesRead, booksByAuthor, topAuthorsByLikes)

---

## Phase 7: Backend — Build Verification

- [ ] T017 Run `dotnet build` in `backend/` — verify no compilation errors
- [ ] T018 Run `dotnet test` in `backend/` — verify all tests pass

---

## Phase 8: Frontend — Models

- [ ] T019 Create `PagedResult` model in `frontend/src/app/models/paged-result.model.ts`
      ```typescript
      export interface PagedResult<T> {
        items: T[];
        totalCount: number;
        pageNumber: number;
        pageSize: number;
        totalPages: number;
        hasNextPage: boolean;
        hasPrevPage: boolean;
      }
      ```

- [ ] T020 Update `ReportData` model in `frontend/src/app/models/report.model.ts`
      Add new fields:
      ```typescript
      export interface AuthorReportItem {
        authorId: string;
        authorName: string;
        bookCount: number;
        totalPagesRead: number;
        likeCount: number;
      }

      export interface ReportData {
        totalBooks: number;
        totalRead: number;
        totalPagesRead: number;
        booksByGenre: GenreReportItem[];
        genresByLikes: GenreReportItem[];
        booksByAuthor: AuthorReportItem[];
        topAuthorsByLikes: AuthorReportItem[];
      }
      ```

---

## Phase 9: Frontend — Services

- [ ] T021 Update `BookService.getAll` in `frontend/src/app/services/book.service.ts`
      - Add `page?: number` and `pageSize?: number` params
      - Pass `page` and `pageSize` as query params
      - Change return type to `Observable<Result<PagedResult<Book>>>`
      - Import `PagedResult` from models

- [ ] T022 Update BookService tests in `frontend/src/app/services/book.service.spec.ts`
      - Add tests: `getAll` with page/pageSize params, verifica query string

- [ ] T023 Update `ReportService` if needed (return type unchanged, but data shape changed)
      - No change needed — `Result<ReportData>` já retorna os dados

---

## Phase 10: Frontend — Components

- [ ] T024 Update `BookListComponent` type annotations in `frontend/src/app/components/book-list/book-list.component.ts`
      - Change `books: Book[] = []` to use `pagedResult` approach
      - Add state: `currentPage = 1`, `pageSize = 50`, `totalPages = 0`, `totalCount = 0`
      - Update `loadBooks()` to pass page/pageSize and handle `PagedResult<Book>` response
      - Add `goToPage(page: number)` method
      - Add `get pageNumbers()` getter for pagination display
      - Reset `currentPage = 1` in `onFilterChange()`

- [ ] T025 Update `BookListComponent` template in `frontend/src/app/components/book-list/book-list.component.html`
      - Iterate over `pagedResult.items` instead of `books`
      - Add pagination controls after table (only if totalPages > 1):
        ```html
        <div class="pagination" *ngIf="totalPages > 1">
          <button (click)="goToPage(currentPage - 1)" [disabled]="currentPage <= 1">Anterior</button>
          <button *ngFor="let p of pageNumbers" (click)="goToPage(p)" [class.active]="p === currentPage">{{ p }}</button>
          <button (click)="goToPage(currentPage + 1)" [disabled]="currentPage >= totalPages">Próximo</button>
        </div>
        <div class="pagination-info">Página {{ currentPage }} de {{ totalPages }} ({{ totalCount }} livros)</div>
        ```

- [ ] T026 Add pagination styles in book list component (inline or global)
      - Pagination bar: horizontal, centered, buttons with hover/active states
      - Active page highlighted
      - Disabled buttons (Anterior/Próximo) visualmente desabilitados

- [ ] T027 Update `ReportComponent` in `frontend/src/app/components/report/report.component.ts`
      Add new report sections to template:
      - "Total de Páginas Lidas" card (between existing cards)
      - "Livros por Autor" table (AuthorReportItem: Autor, Livros, Páginas Lidas, Likes)
      - "Autores com Mais Curtidas" table (AuthorReportItem: Autor, Likes, Livros)
      - Follow existing style pattern (cards + tables)

- [ ] T028 Update BookList component tests
      File `frontend/src/app/components/book-list/book-list.component.spec.ts`:
      - Update existing tests for new pagination state
      - Add test: pagination controls appear when totalPages > 1
      - Add test: goToPage updates currentPage and reloads books
      - Add test: filter change resets to page 1

- [ ] T029 Update Report component tests if they exist
      File `frontend/src/app/components/report/`:
      - Verify new report sections render with data

---

## Phase 11: Frontend — Build Verification

- [ ] T030 Run `npx ng build` in `frontend/` — verify no compilation errors
- [ ] T031 Run `npx ng test --watch=false --browsers=ChromeHeadlessNoSandbox` in `frontend/` — verify tests pass

---

## Dependencies & Execution Order

### Phase Dependencies
- Phase 1 (PagedResult): NO dependencies — can start immediately
- Phase 2 (Repository): depends on Phase 1
- Phase 3 (Reports): depends on Phase 2 (GetAllForReportsAsync needs BookAuthors)
- Phase 4 (Book pagination): depends on Phase 1 + 2
- Phase 5 (Controller): depends on Phase 4
- Phase 6 (Integration tests): depends on Phase 3 + 5
- Phase 7 (Backend build): depends on Phase 6
- Phase 8 (Frontend models): NO backend dependencies
- Phase 9 (Frontend services): depends on Phase 8
- Phase 10 (Frontend components): depends on Phase 8 + 9
- Phase 11 (Frontend build): depends on Phase 10

### Parallel Opportunities
- Phase 1 + Phase 8 (PagedResult + frontend models) — parallel
- Phase 3 (Reports) + Phase 4 (Book pagination) — parallel after Phase 2
- Implementation + Tests within each phase can be done sequentially (TDD: test first)

### File Change Summary

| File | Change |
|------|--------|
| `Yomimono.Application/Common/PagedResult.cs` | **NOVO** |
| `Yomimono.Application/Books/Common/IBookRepository.cs` | +GetAllPagedAsync |
| `Yomimono.Infrastructure/Repositories/BookRepository.cs` | Implementar GetAllPagedAsync, atualizar GetAllForReportsAsync |
| `Yomimono.Application/Reports/DTOs/ReportDto.cs` | +AuthorReportDto, +TotalPagesRead, +BooksByAuthor, +TopAuthorsByLikes |
| `Yomimono.Application/Reports/Queries/GetReportsQueryHandler.cs` | Calcular novos campos |
| `Yomimono.Application/Books/Queries/GetAllBooksQuery.cs` | +PageNumber, +PageSize, mudar return type |
| `Yomimono.Application/Books/Handlers/GetAllBooksQueryHandler.cs` | Usar GetAllPagedAsync, retornar PagedResult |
| `Yomimono.Api/Controllers/BooksController.cs` | +page, +pageSize params |
| `Yomimono.Api.Tests/Handlers/GetReportsQueryHandlerTests.cs` | **NOVO** ou atualizado |
| `Yomimono.Api.Tests/Handlers/GetAllBooksQueryHandlerTests.cs` | Atualizado com testes de paginação |
| `Yomimono.Api.Tests/Integration/` | +testes de integração |
| `frontend/src/app/models/paged-result.model.ts` | **NOVO** |
| `frontend/src/app/models/report.model.ts` | +AuthorReportItem, +novos campos |
| `frontend/src/app/services/book.service.ts` | +page/pageSize params, PagedResult return |
| `frontend/src/app/services/book.service.spec.ts` | Atualizado |
| `frontend/src/app/components/book-list/book-list.component.ts` | Paginação state + métodos |
| `frontend/src/app/components/book-list/book-list.component.html` | +Paginator UI |
| `frontend/src/app/components/book-list/book-list.component.spec.ts` | Atualizado |
| `frontend/src/app/components/report/report.component.ts` | Novas seções de relatório |
