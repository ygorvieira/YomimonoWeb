# Feature Specification: Busca, Trade Paperback, Digital, Remoção de ISBN e Páginas Restantes

**Feature Branch**: `feature/yg-book-enhancements`

**Created**: 2026-06-25

**Status**: Implemented

## User Stories

### User Story 1 — Busca textual em Livros, Autores e Gêneros
Como usuário, quero pesquisar livros por título, autores por nome e gêneros por nome, para encontrar rapidamente o que procuro.

### User Story 2 — Remover ISBN
Como usuário, não quero mais o campo ISBN no cadastro de livros, pois não utilizo esta informação.

### User Story 3 — Trade Paperback (Encadernado)
Como usuário, quero marcar um livro como encadernado e descrever sua edição, além de filtrar por este atributo.

### User Story 4 — Livro Digital
Como usuário, quero marcar um livro como digital, ocultando o campo de páginas, e filtrar por este atributo.

### User Story 5 — Páginas Restantes no Relatório
Como usuário, quero saber quantas páginas ainda faltam ler (ignorando livros digitais e já lidos).

## Requirements

### Functional Requirements

- **FR-001**: GET /api/books DEVE aceitar `searchTerm` (string) para filtrar por título
- **FR-002**: GET /api/authors DEVE aceitar `searchTerm` (string) para filtrar por nome
- **FR-003**: GET /api/genres DEVE aceitar `searchTerm` (string) para filtrar por nome
- **FR-004**: Campo `Isbn` DEVE ser removido de Book entity, DTOs, formulários e banco
- **FR-005**: Book DEVE ter `IsTradePaperback` (bool, default false)
- **FR-006**: Book DEVE ter `TradeEdition` (string?, max 200, opcional)
- **FR-007**: Book DEVE ter `IsDigital` (bool, default false)
- **FR-008**: Formulário de livro DEVE ter checkbox "Encadernado (Trade Paperback)"
- **FR-009**: Quando "Encadernado" marcado, DEVE exibir campo "Edição"
- **FR-010**: Formulário DEVE ter checkbox "Livro Digital"
- **FR-011**: Quando "Digital" marcado, campo "Páginas" DEVE ser desabilitado
- **FR-012**: Relatório DEVE exibir "Páginas Restantes" (soma de PageCount onde readingStatus NOT IN ('Lido','Relido') AND IsDigital = false)
- **FR-013**: Listagem de livros DEVE mostrar indicadores visuais de "Encadernado" e "Digital"
- **FR-014**: Listagem DEVE ter input de busca por título

### Non-Functional Requirements

- **NFR-001**: Busca textual DEVE ser server-side (Contains no banco)
- **NFR-002**: Remoção de ISBN DEVE incluir migration sem quebra
- **NFR-003**: Todos os handlers retornam `Result<T>` (Constitution V)
- **NFR-004**: TDD obrigatório (Constitution II)

## Data Model Changes

### Book (entidade)
```
- string? Isbn              ← REMOVIDO
+ bool IsTradePaperback      ← default false
+ string? TradeEdition       ← max 200
+ bool IsDigital             ← default false
```

### BookDto / CreateBookDto / UpdateBookDto
```
- string? Isbn   ← REMOVIDO de todos
+ bool IsTradePaperback
+ string? TradeEdition
+ bool IsDigital
```

### ReportDto
```
+ int TotalPagesRemaining
```

### API Changes

#### GET /api/books
```
+ [FromQuery] string? searchTerm
```

#### GET /api/authors
```
+ [FromQuery] string? searchTerm
```

#### GET /api/genres
```
+ [FromQuery] string? searchTerm
```

## Ambiguities & Decisions

1. **"Páginas restantes" considera "Relido"?** → Não. Relido já foi lido. Apenas livros com status diferente de "Lido" e "Relido" são considerados.
2. **"Listar edições encadernadas"** → Campo textual opcional "Edição" (ex: "1ª Edição") + filtro na listagem.
3. **Livro digital com pageCount** → Campo de páginas desabilitado no formulário se digital; relatório ignora páginas de digitais.
4. **Busca textual** → Server-side com `Contains` no banco (case-insensitive via PostgreSQL/EF Core).
