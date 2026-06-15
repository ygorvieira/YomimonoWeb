---
description: "Task list for 002-author-status-filters feature"
---

# Tasks: Author como Entidade + Status de Leitura + Like + Filtros

**Input**: spec.md in specs/002-author-status-filters/

**TDD**: Tests MUST be written first and fail before implementation (Constitution II).
All handler tests MUST assert `result.Valid` (Constitution V).

**Path Conventions**:
- Backend: `backend/Yomimono.Domain/`, `backend/Yomimono.Application/`, etc.
- Tests: `backend/Yomimono.Api.Tests/`
- Frontend: `frontend/src/app/`

## Phase 1: Domain Layer — Entities

**Purpose**: New entities and Book modifications at domain level

- [ ] T001 [P] Create `ReadingStatus` enum in `backend/Yomimono.Domain/Common/ReadingStatus.cs`
      Values: `Lendo`, `Lido`, `Abandonado`. Namespace `Yomimono.Domain.Common`.

- [ ] T002 [P] Create `Author` entity in `backend/Yomimono.Domain/Entities/Author.cs`
      Inherits `BaseEntity`. Property `Name` (string, private set). Factory `Create(string name)`.
      Validation: name required, max 150 chars.

- [ ] T003 Create `BookAuthor` join entity in `backend/Yomimono.Domain/Entities/BookAuthor.cs`
      Properties: `BookId` (Guid), `AuthorId` (Guid). Navigation: `Book`, `Author`.
      No `BaseEntity` inheritance. Namespace `Yomimono.Domain.Entities`.

- [ ] T004 Update `Book` entity in `backend/Yomimono.Domain/Entities/Book.cs`
      - Remove `Author` (string) and `Genre` (string) properties
      - Add `GenreId` (Guid), `BookAuthors` (ICollection<BookAuthor>)
      - Add `ReadingStatus` (string?, nullable)
      - Add `IsLiked` (bool, default false)
      - Update `Create(...)` method: accept `Guid genreId`, `IEnumerable<Guid> authorIds`,
        `string? readingStatus`, `bool isLiked` parameters
      - Update `UpdateDetails(...)`: accept nullable versions of new fields
      - Keep all existing validations for Title, Isbn, etc.

---

## Phase 2: Infrastructure — EF Core Configuration

**Purpose**: Database mappings, DbContext updates, seed data, migration

- [ ] T005 Create `AuthorConfiguration` in `backend/Yomimono.Infrastructure/Data/Configurations/AuthorConfiguration.cs`
      Table "Authors". Name required, max 150, unique index. Query filter: `DeletedAt == null`.

- [ ] T006 Create `BookAuthorConfiguration` in `backend/Yomimono.Infrastructure/Data/Configurations/BookAuthorConfiguration.cs`
      Table "BookAuthors". Composite PK (`BookId`, `AuthorId`). FKs with cascade delete.

- [ ] T007 Update `BookConfiguration` in `backend/Yomimono.Infrastructure/Data/Configurations/BookConfiguration.cs`
      - Remove `Author` and `Genre` string property configurations
      - Add `GenreId` FK configuration (required, FK → Genres)
      - Add `ReadingStatus` property: string, max 20, nullable
      - Add `IsLiked` property: bool, default `false`
      - Add `BookAuthors` navigation (ICollection)

- [ ] T008 Update `AppDbContext` in `backend/Yomimono.Infrastructure/Data/AppDbContext.cs`
      Add `DbSet<Author> Authors` property.

- [ ] T009 Update `SeedData` in `backend/Yomimono.Infrastructure/Data/SeedData.cs`
      Add `SeedAuthorsAsync` with default authors: "Machado de Assis", "Clarice Lispector",
      "Jorge Amado", "José Saramago", "Guimarães Rosa", "Cecília Meireles",
      "Carlos Drummond de Andrade", "Fernando Pessoa", "Eça de Queirós", "Mário de Andrade".

- [ ] T010 Update `Program.cs` in `backend/Yomimono.Api/Program.cs`
      Call `SeedData.SeedAuthorsAsync(db)` after migration and genre seeding.

- [ ] T011 [P] Create migration with data migration in `backend/Yomimono.Infrastructure/Data/Migrations/`
      ```bash
      dotnet ef migrations add AddAuthorEntityAndBookStatus -p backend/Yomimono.Infrastructure -s backend/Yomimono.Api
      ```
      Then edit the generated migration to add data migration:
      1. Add `GenreId` nullable, `ReadingStatus`, `IsLiked` columns
      2. Loop distinct Book.Author strings → create Author entities
      3. Loop all books → create BookAuthor links
      4. Drop `Author` and `Genre` string columns
      5. Make `GenreId` NOT NULL (requires existing books to have a genre assigned)

- [ ] T012 Update `DependencyInjection` in `backend/Yomimono.Infrastructure/DependencyInjection.cs`
      Register `IAuthorRepository`, `IGenreRepository` as scoped services.

---

## Phase 3: Application — Author CRUD (TDD)

**Purpose**: Full CQRS for Author entity

**Pre-condition**: Write tests FIRST, ensure they FAIL, then implement.

- [ ] T013 Write tests for Author handlers
      Files in `backend/Yomimono.Api.Tests/Handlers/`:
      - `CreateAuthorCommandHandlerTests.cs` (3 tests: valid, duplicate name, invalid data)
      - `UpdateAuthorCommandHandlerTests.cs` (2 tests: valid, not found)
      - `DeleteAuthorCommandHandlerTests.cs` (2 tests: valid, not found)
      - `GetAllAuthorsQueryHandlerTests.cs` (1 test: returns list)
      - `GetAuthorByIdQueryHandlerTests.cs` (2 tests: found, not found)
      All tests MUST assert `result.Valid`.

- [ ] T014 Implement Author DTOs in `backend/Yomimono.Application/Authors/DTOs/AuthorDto.cs`
      `AuthorDto(Guid Id, string Name, DateTime CreatedAt, DateTime UpdatedAt)`
      `CreateAuthorDto(string Name)`
      `UpdateAuthorDto(string Name)`
      Namespace `Yomimono.Application.Authors.DTOs`.

- [ ] T015 Create `IAuthorRepository` in `backend/Yomimono.Application/Authors/Common/IAuthorRepository.cs`
      Methods: `GetByIdAsync`, `GetAllAsync`, `AddAsync`, `Update`, `Delete`, `GetByNameAsync`.

- [ ] T016 Implement Author Commands + Queries
      Files in `backend/Yomimono.Application/Authors/`:
      - `Commands/CreateAuthorCommand.cs`
      - `Commands/UpdateAuthorCommand.cs`
      - `Commands/DeleteAuthorCommand.cs`
      - `Queries/GetAllAuthorsQuery.cs`
      - `Queries/GetAuthorByIdQuery.cs`

- [ ] T017 Implement Author Handlers
      Files in `backend/Yomimono.Application/Authors/Handlers/`:
      - `CreateAuthorCommandHandler.cs` — check duplicate name, create, save
      - `UpdateAuthorCommandHandler.cs` — check duplicate name excluding self, update
      - `DeleteAuthorCommandHandler.cs` — soft delete
      - `GetAllAuthorsQueryHandler.cs` — list ordered by name
      - `GetAuthorByIdQueryHandler.cs` — find by ID, NotFound if null

- [ ] T018 Implement `AuthorRepository` in `backend/Yomimono.Infrastructure/Repositories/AuthorRepository.cs`
      Full EF Core implementation of `IAuthorRepository`.

---

## Phase 4: Application — Genre CRUD (TDD)

**Purpose**: Full CQRS for Genre entity (existing entity, new API)

**Pre-condition**: Write tests FIRST.

- [ ] T019 Write tests for Genre handlers
      Files in `backend/Yomimono.Api.Tests/Handlers/`:
      - `CreateGenreCommandHandlerTests.cs` (3 tests)
      - `UpdateGenreCommandHandlerTests.cs` (2 tests)
      - `DeleteGenreCommandHandlerTests.cs` (2 tests)
      - `GetAllGenresQueryHandlerTests.cs` (1 test)
      - `GetGenreByIdQueryHandlerTests.cs` (2 tests)

- [ ] T020 Implement Genre DTOs in `backend/Yomimono.Application/Genres/DTOs/GenreDto.cs`
      `GenreDto(Guid Id, string Name, DateTime CreatedAt, DateTime UpdatedAt)`
      `CreateGenreDto(string Name)`
      `UpdateGenreDto(string Name)`

- [ ] T021 Create `IGenreRepository` in `backend/Yomimono.Application/Genres/Common/IGenreRepository.cs`
      Methods: `GetByIdAsync`, `GetAllAsync`, `AddAsync`, `Update`, `Delete`, `GetByNameAsync`.

- [ ] T022 Implement Genre Commands + Queries
      Files in `backend/Yomimono.Application/Genres/`:
      - `Commands/CreateGenreCommand.cs`
      - `Commands/UpdateGenreCommand.cs`
      - `Commands/DeleteGenreCommand.cs`
      - `Queries/GetAllGenresQuery.cs`
      - `Queries/GetGenreByIdQuery.cs`

- [ ] T023 Implement Genre Handlers
      Files in `backend/Yomimono.Application/Genres/Handlers/`:
      - `CreateGenreCommandHandler.cs`
      - `UpdateGenreCommandHandler.cs`
      - `DeleteGenreCommandHandler.cs`
      - `GetAllGenresQueryHandler.cs`
      - `GetGenreByIdQueryHandler.cs`

- [ ] T024 Implement `GenreRepository` in `backend/Yomimono.Infrastructure/Repositories/GenreRepository.cs`
      Full EF Core implementation of `IGenreRepository`.

---

## Phase 5: Application — Book CRUD Update + Status + Filters (TDD)

**Purpose**: Update Book handlers for new entity relationships, add quick status update, add filters

**Pre-condition**: Write tests FIRST.

- [ ] T025 Write/update tests for Book handlers
      Files in `backend/Yomimono.Api.Tests/Handlers/`:
      - **Update** `CreateBookCommandHandlerTests.cs` — add test for valid authorIds+genreId,
        invalid authorId, invalid genreId
      - **Update** `UpdateBookCommandHandlerTests.cs` — add test for author reconciliation
      - **NEW** `UpdateBookStatusCommandHandlerTests.cs` (3 tests: valid status+like,
        not found, partial update)
      - **Update** `GetAllBooksQueryHandlerTests.cs` — add tests for genre filter,
        author filter, status filter, combined filters
      - **Update** `GetBookByIdQueryHandlerTests.cs` — verify AuthorNames + GenreName in response

- [ ] T026 Update Book DTOs in `backend/Yomimono.Application/Books/DTOs/BookDto.cs`
      `BookDto` changes:
      - Remove: `Author` (string), `Genre` (string)
      - Add: `AuthorIds` (Guid[]), `AuthorNames` (string[]), `GenreId` (Guid),
        `GenreName` (string), `ReadingStatus` (string?), `IsLiked` (bool)
      `CreateBookDto` changes:
      - Remove: `Author`, `Genre`
      - Add: `AuthorIds` (Guid[]), `GenreId` (Guid), `ReadingStatus` (string?), `IsLiked` (bool)
      `UpdateBookDto` similar (all nullable)

- [ ] T027 Create `UpdateBookStatusCommand` in `backend/Yomimono.Application/Books/Commands/UpdateBookStatusCommand.cs`
      `record UpdateBookStatusCommand(Guid Id, UpdateBookStatusDto Status) : IRequest<Result<BookDto>>`
      With `UpdateBookStatusDto(string? ReadingStatus, bool? IsLiked)`.

- [ ] T028 Create `UpdateBookStatusCommandHandler` in `backend/Yomimono.Application/Books/Handlers/`
      - Load book by ID
      - If null, return NotFound
      - If ReadingStatus not null, set book.ReadingStatus
      - If IsLiked not null, set book.IsLiked
      - Save
      - Return Result<BookDto>

- [ ] T029 Update `GetAllBooksQuery` in `backend/Yomimono.Application/Books/Queries/GetAllBooksQuery.cs`
      Add filter params: `Guid? GenreId`, `Guid? AuthorId`, `string? ReadingStatus`.

- [ ] T030 Update `IBookRepository` in `backend/Yomimono.Application/Books/Common/IBookRepository.cs`
      - Update `GetAllAsync` signature: accept filter params
      - Remove `GetByGenreAsync(string genre)` (replaced by filtered GetAll)
      - Add `GetByAuthorAsync(Guid authorId)` if needed

- [ ] T031 Update `CreateBookCommandHandler` in `backend/Yomimono.Application/Books/Handlers/`
      - Receive `authorIds` (Guid[]) and `genreId` (Guid)
      - Validate all AuthorIds exist in DB
      - Validate GenreId exists in DB
      - Call `Book.Create(...)` with new params
      - After Book created, add `BookAuthor` links for each authorId
      - Map to DTO including AuthorNames + GenreName

- [ ] T032 Update `UpdateBookCommandHandler` in `backend/Yomimono.Application/Books/Handlers/`
      - If `AuthorIds` provided: reconcile BookAuthors
        (remove links not in new list, add new links)
      - If `GenreId` provided: validate exists, update
      - If `ReadingStatus` or `IsLiked` provided: update

- [ ] T033 Update `GetAllBooksQueryHandler` in `backend/Yomimono.Application/Books/Handlers/`
      - Pass filter params to repository
      - Include `.Include(b => b.BookAuthors).ThenInclude(ba => ba.Author)`
        and `.Include(b => b.Genre)` for DTO mapping
      - Map `AuthorNames` from BookAuthors collection

- [ ] T034 Update `GetBookByIdQueryHandler` in `backend/Yomimono.Application/Books/Handlers/`
      Same includes as GetAll for DTO mapping.

- [ ] T035 Update `BookRepository` in `backend/Yomimono.Infrastructure/Repositories/BookRepository.cs`
      - `GetAllAsync`: apply `genreId` filter, `authorId` filter (via BookAuthors),
        `readingStatus` filter. Use conditional `.Where()`.
      - `GetByIdAsync`: include `BookAuthors.Author` and `Genre`.

---

## Phase 6: API Controllers

**Purpose**: Expose new endpoints and updated endpoints

- [ ] T036 Create `AuthorsController` in `backend/Yomimono.Api/Controllers/AuthorsController.cs`
      `[Authorize]`, `[Route("api/[controller]")]`
      - GET / → GetAllAuthorsQuery
      - GET /{id:guid} → GetAuthorByIdQuery
      - POST / → CreateAuthorCommand
      - PUT /{id:guid} → UpdateAuthorCommand
      - DELETE /{id:guid} → DeleteAuthorCommand

- [ ] T037 Create `GenresController` in `backend/Yomimono.Api/Controllers/GenresController.cs`
      Same structure as AuthorsController.

- [ ] T038 Update `BooksController` in `backend/Yomimono.Api/Controllers/BooksController.cs`
      - GET /: accept `[FromQuery]` params for filters
      - Add `PATCH /{id:guid}/status`: calls `UpdateBookStatusCommand`

---

## Phase 7: Backend Build Verification

- [ ] T039 Run `dotnet build` in `backend/` — verify no compilation errors
- [ ] T040 Run `dotnet test` in `backend/` — verify all 40+ tests pass

---

## Phase 8: Frontend — Data Layer

**Purpose**: New models and services for Author and Genre; update Book model

- [ ] T041 Create Author model in `frontend/src/app/models/author.model.ts`
      `Author`, `CreateAuthorDto`, `UpdateAuthorDto` interfaces.

- [ ] T042 Create Genre model in `frontend/src/app/models/genre.model.ts`
      `Genre`, `CreateGenreDto`, `UpdateGenreDto` interfaces.

- [ ] T043 Write AuthorService tests in `frontend/src/app/services/author.service.spec.ts`
      5 tests: create, getAll, getById, update, delete.

- [ ] T044 Create AuthorService in `frontend/src/app/services/author.service.ts`
      Full CRUD calling `/api/authors`.

- [ ] T045 Write GenreService tests in `frontend/src/app/services/genre.service.spec.ts`
      5 tests: create, getAll, getById, update, delete.

- [ ] T046 Create GenreService in `frontend/src/app/services/genre.service.ts`
      Full CRUD calling `/api/genres`.

- [ ] T047 Update Book model in `frontend/src/app/models/book.model.ts`
      Change: `author` → `authorIds + authorNames`; `genre` → `genreId + genreName`;
      Add: `readingStatus`, `isLiked`.
      Update: `CreateBookDto`, `UpdateBookDto`.

- [ ] T048 Update BookService in `frontend/src/app/services/book.service.ts`
      - `getAll(filters?)`: pass query params
      - `updateStatus(id, dto)`: PATCH /api/books/{id}/status

- [ ] T049 Update BookService tests in `frontend/src/app/services/book.service.spec.ts`
      Add tests for filtering and updateStatus.

---

## Phase 9: Frontend — Components

- [ ] T050 Update BookList component — filters
      - Add `<select>` for Genre (carregado de GenreService.getAll())
      - Add `<select>` for Author (carregado de AuthorService.getAll())
      - Add `<select>` for ReadingStatus (Lendo, Lido, Abandonado, Todos)
      - On change: reload with filters
      - Table: show `authorNames` (join), Status badge, Like icon/button

- [ ] T051 Update BookForm component — dropdowns
      - Author: `<select multiple>` carregado de AuthorService
      - Genre: `<select>` carregado de GenreService
      - Status: `<select>` Lendo/Lido/Abandonado
      - Like: checkbox or toggle
      - On submit: send `authorIds[]`, `genreId`, `readingStatus`, `isLiked`

- [ ] T052 Update BookDetail component — status + like display
      - Show ReadingStatus as badge color: Lendo (blue), Lido (green), Abandonado (red)
      - Show IsLiked as toggle button (heart icon)
      - Quick toggle: call PATCH on status/like change

- [ ] T053 Update BookList component tests
- [ ] T054 Update BookForm component tests
- [ ] T055 Update BookDetail component tests

---

## Phase 10: CI/CD

- [ ] T056 Create GitHub Actions workflow in `.github/workflows/ci.yml`
      Trigger: push, PR to main
      Jobs:
      - `dotnet build && dotnet test` on backend/
      - `npm ci && npx ng test --watch=false --browsers=ChromeHeadlessNoSandbox` on frontend/

---

## Dependencies & Execution Order

### Phase Dependencies
- Phase 1 (Domain): can start immediately — NO dependencies
- Phase 2 (Infrastructure): depends on Phase 1
- Phase 3 (Author CRUD): depends on Phase 1 + 2
- Phase 4 (Genre CRUD): depends on Phase 1 + 2
- Phase 5 (Book updates): depends on Phase 3 + 4
- Phase 6 (Controllers): depends on Phase 3 + 4 + 5
- Phase 7 (Build): depends on Phase 6
- Phase 8 (Frontend data): depends on none (can start after API contract is defined)
- Phase 9 (Frontend components): depends on Phase 8
- Phase 10 (CI/CD): depends on all previous phases

### Parallel Opportunities
- T001 + T002 + T003 (domain files) — parallel
- Phase 3 + Phase 4 — parallel (Author + Genre CRUD are independent)
- All [P] marked tasks — parallel
