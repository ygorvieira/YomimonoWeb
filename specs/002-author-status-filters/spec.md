# Feature Specification: Author como Entidade + Status de Leitura + Like + Filtros

**Feature Branch**: `002-author-status-filters`

**Created**: 2026-06-14

**Status**: Aprovado

## User Stories

### User Story 1 — Author como Entidade com CRUD (Priority: P1)

Como usuário, quero cadastrar autores separadamente dos livros e ter um CRUD completo
para gerenciá-los, para reutilizar autores em múltiplos livros.

**Independent Test**: Criar autor via POST /api/authors, listar via GET /api/authors,
editar via PUT /api/authors/{id}, excluir via DELETE /api/authors/{id}.

**Acceptance Scenarios**:

1. **Given** dados de autor válidos, **When** envio POST /api/authors, **Then** recebo 201 Created com o autor criado
2. **Given** nome de autor duplicado, **When** envio POST /api/authors, **Then** recebo 400 BadRequest com `valid: false`
3. **Given** um autor existente, **When** envio GET /api/authors, **Then** recebo a lista com o autor
4. **Given** um autor existente, **When** envio PUT /api/authors/{id}, **Then** o autor é atualizado
5. **Given** um autor existente, **When** envio DELETE /api/authors/{id}, **Then** o autor é removido (soft delete)

### User Story 2 — Livro com Múltiplos Autores via Dropdown (Priority: P1)

Como usuário, quero ao criar/editar um livro selecionar múltiplos autores de um
dropdown e um gênero de outro dropdown, em vez de digitar texto livre.

**Independent Test**: Criar um livro selecionando 2 autores + 1 gênero via POST /api/books
com `authorIds` e `genreId`.

**Acceptance Scenarios**:

1. **Given** authorIds e genreId válidos, **When** crio um livro, **Then** o livro é criado com os autores e gênero selecionados
2. **Given** authorId inexistente, **When** crio um livro, **Then** recebo erro 400
3. **Given** genreId inexistente, **When** crio um livro, **Then** recebo erro 400
4. **Given** um livro com autores, **When** edito alterando os autores, **Then** os autores são reconciliados (removidos os antigos, adicionados os novos)

### User Story 3 — Status de Leitura e Like (Priority: P1)

Como usuário, quero marcar em cada livro se estou lendo, já li ou abandonei, e
marcar se gostei do livro (like). Quero poder atualizar esses campos rapidamente sem
precisar editar todo o livro.

**Independent Test**: Criar livro sem status, depois atualizar via PATCH /api/books/{id}/status
com `readingStatus` e `isLiked`.

**Acceptance Scenarios**:

1. **Given** um livro existente sem status, **When** envio PATCH com `readingStatus: "Lendo"`, **Then** o livro retorna com `readingStatus: "Lendo"`
2. **Given** um livro existente, **When** envio PATCH com `isLiked: true`, **Then** o livro retorna com `isLiked: true`
3. **Given** um livro existente, **When** envio PATCH com ambos, **Then** ambos são atualizados
4. **Given** um livro inexistente, **When** envio PATCH, **Then** recebo 404 NotFound

### User Story 4 — Filtro por Gênero, Autor e Status (Priority: P2)

Como usuário, quero filtrar a listagem de livros por gênero, autor e status de leitura,
de forma combinada.

**Independent Test**: Chamar GET /api/books?genreId=X com 10 livros onde apenas 2 são
do gênero X, verificar que retorna apenas 2.

**Acceptance Scenarios**:

1. **Given** livros de múltiplos gêneros, **When** filtro por `genreId`, **Then** vejo apenas livros daquele gênero
2. **Given** livros de múltiplos autores, **When** filtro por `authorId`, **Then** vejo apenas livros que tenham aquele autor
3. **Given** livros com diferentes status, **When** filtro por `readingStatus`, **Then** vejo apenas livros com aquele status
4. **Given** parâmetros de filtro combinados, **When** filtro por `genreId` + `authorId`, **Then** vejo apenas livros que satisfazem ambos

### Edge Cases

- Author com nome vazio: rejeitado (400)
- Author com nome > 150 caracteres: rejeitado
- Book sem authors: permitido (criar sem BookAuthors)
- Book sem readingStatus: armazenado como null no banco
- Filter combinado vazio (sem params): retorna todos os livros
- Filtro sem resultados: retorna array vazio com valid: true

## Requirements

### Functional Requirements

- **FR-001**: Sistema DEVE ter entidade Author com Name único
- **FR-002**: Sistema DEVE ter CRUD completo para Author (Create, Read, Update, Delete)
- **FR-003**: Sistema DEVE ter CRUD completo para Genre (Create, Read, Update, Delete)
- **FR-004**: Sistema DEVE relacionar Book a Author via tabela associativa (N:N)
- **FR-005**: Sistema DEVE relacionar Book a Genre via FK (N:1)
- **FR-006**: Book DEVE ter `ReadingStatus` opcional: Lendo, Lido, Abandonado
- **FR-007**: Book DEVE ter `IsLiked` booleano (default false)
- **FR-008**: Sistema DEVE ter endpoint PATCH /api/books/{id}/status para atualizar
  apenas ReadingStatus e IsLiked
- **FR-009**: GET /api/books DEVE aceitar query params `genreId`, `authorId`, `readingStatus`
- **FR-010**: Frontend DEVE usar dropdowns carregados da API para Author (multi-select)
  e Genre (single-select) no formulário de livro
- **FR-011**: Frontend DEVE exibir filtros de Gênero, Autor e Status na listagem de livros
- **FR-012**: Frontend DEVE exibir ReadingStatus e IsLiked nos detalhes do livro com
  opção de toggle rápido

### Non-Functional Requirements

- **NFR-001**: Todos os handlers retornam `Result<T>` com `Valid` conforme Constitution V
- **NFR-002**: Todos os handlers têm testes TDD que verificam `result.Valid` (Constitution II)
- **NFR-003**: Soft delete em Author e Genre (BaseEntity.DeletedAt)
- **NFR-004**: Filtros aplicados no banco de dados (WHERE), não em memória
- **NFR-005**: Migration automática de dados existentes: criar Authors a partir das
  strings únicas em Book.Author, pular registros vazios
- **NFR-006**: API respostas seguem padrão `Result<T>` consistente

## Data Model Changes

```text
Books (antes)                Books (depois)
┌──────────────────┐         ┌──────────────────┐
│ Id (PK)          │         │ Id (PK)          │
│ Title            │         │ Title            │
│ Author (string)  │  ──►    │ Isbn             │
│ Isbn             │         │ PublicationYear  │
│ PublicationYear  │         │ Publisher        │
│ Publisher        │         │ GenreId (FK) ─────┐
│ Genre (string)   │         │ Description      │
│ Description      │         │ PageCount        │
│ PageCount        │         │ CoverUrl         │
│ CoverUrl         │         │ ReadingStatus?   │
│ CreatedAt        │         │ IsLiked          │
│ UpdatedAt        │         │ CreatedAt        │
│ DeletedAt        │         │ UpdatedAt        │
└──────────────────┘         │ DeletedAt        │
                             └──────┬───────────┘
Authors (NOVO)                      │
┌──────────────────┐         BookAuthors (NOVO)
│ Id (PK)          │         ┌──────────────────┐
│ Name (unique)    │◄────────┤ BookId (FK, PK)  │
│ CreatedAt        │         │ AuthorId (FK, PK)│
│ UpdatedAt        │         └──────────────────┘
│ DeletedAt        │
└──────────────────┘
```

## API Contract

### GET /api/books

```http
GET /api/books?genreId={guid}&authorId={guid}&readingStatus=Lendo
Authorization: Bearer {token}
```

Response 200:
```json
{
  "valid": true,
  "data": [
    {
      "id": "guid",
      "title": "Dom Casmurro",
      "authorIds": ["guid1", "guid2"],
      "authorNames": ["Machado de Assis"],
      "isbn": "9788535902778",
      "publicationYear": 1899,
      "publisher": "Garnier",
      "genreId": "guid",
      "genreName": "Romance",
      "description": null,
      "pageCount": 256,
      "coverUrl": null,
      "readingStatus": "Lido",
      "isLiked": true,
      "createdAt": "2024-01-01T00:00:00Z",
      "updatedAt": "2024-01-01T00:00:00Z"
    }
  ],
  "messages": [],
  "statusCode": 200
}
```

### PATCH /api/books/{id}/status

```http
PATCH /api/books/{id}/status
Authorization: Bearer {token}
Content-Type: application/json

{
  "readingStatus": "Lendo",
  "isLiked": true
}
```

Response 200:
```json
{
  "valid": true,
  "data": { ... book ... },
  "messages": ["Status atualizado com sucesso."],
  "statusCode": 200
}
```

### POST /api/books (atualizado)

```json
{
  "title": "Dom Casmurro",
  "authorIds": ["guid-author-1"],
  "isbn": "9788535902778",
  "publicationYear": 1899,
  "publisher": "Garnier",
  "genreId": "guid-genre-1",
  "description": null,
  "pageCount": 256,
  "coverUrl": null,
  "readingStatus": null,
  "isLiked": false
}
```
