# Feature Specification: Relatórios Avançados, Data de Leitura e Paginação

**Feature Branch**: `003-reports-pagination-tracking`

**Created**: 2026-06-17

**Status**: Draft

**Input**: User description:
- Relatórios: Total de páginas lidas (contabilizar o total de páginas de todos os livros marcados como Lido)
- Relatórios: Livros por autor
- Relatórios: Autor com mais curtidas
- Tela de Livros com paginação (50)

## User Stories

### User Story 1 — Total de Páginas Lidas (Priority: P1)

Como usuário, quero ver no relatório o total de páginas que já li, somando todos os livros marcados como "Lido" ou "Relido", para acompanhar meu progresso de leitura.

**Independent Test**: Adicionar 2 livros com pageCount=200 e 300, marcar um como "Lido" e outro como "Relido", verificar que o relatório mostra totalPagesRead=500.

**Acceptance Scenarios**:

1. **Given** livros com pageCount definido, **When** consulto o relatório, **Then** o totalPagesRead soma pageCount de todos os livros com readingStatus "Lido" ou "Relido"
2. **Given** um livro com pageCount=null marcado como "Lido", **When** consulto o relatório, **Then** esse livro é ignorado no cálculo (null não soma)
3. **Given** nenhum livro lido, **When** consulto o relatório, **Then** totalPagesRead = 0
4. **Given** livros com status "Lendo" ou "Abandonado", **When** consulto o relatório, **Then** esses livros não são contabilizados

---

### User Story 2 — Livros por Autor (Priority: P1)

Como usuário, quero ver no relatório quantos livros cada autor possui e quantas páginas no total foram lidas de cada autor, para entender minha distribuição de leitura por autor.

**Independent Test**: Criar 2 livros do autor A (1 lido) e 3 livros do autor B (2 lidos), verificar que o relatório lista ambos com contagens corretas.

**Acceptance Scenarios**:

1. **Given** múltiplos autores com livros, **When** consulto o relatório, **Then** vejo lista de autores ordenada por total de livros (decrescente)
2. **Given** um autor com livros em co-autoria, **When** consulto o relatório, **Then** o livro é contabilizado para todos os seus autores
3. **Given** um autor sem livros, **When** consulto o relatório, **Then** ele não aparece na lista

---

### User Story 3 — Autor com Mais Curtidas (Priority: P1)

Como usuário, quero saber qual autor tem mais livros marcados com like (curtida), para descobrir meus autores favoritos.

**Independent Test**: Autor A tem 3 livros com isLiked=true, Autor B tem 1 livro com isLiked=true. Relatório deve mostrar Autor A como top author.

**Acceptance Scenarios**:

1. **Given** múltiplos autores com likes, **When** consulto o relatório, **Then** vejo o autor com mais curtidas em destaque
2. **Given** empate entre autores, **When** consulto o relatório, **Then** retorna o primeiro em ordem alfabética (ou ambos em lista ordenada)

---

### User Story 4 — Paginação na Listagem de Livros (Priority: P1)

Como usuário, quero que a listagem de livros mostre no máximo 50 livros por página, com controles de navegação entre páginas, para não sobrecarregar a tela com muitos registros.

**Independent Test**: Criar 150 livros, verificar que a listagem mostra 3 páginas de 50 livros cada.

**Acceptance Scenarios**:

1. **Given** mais de 50 livros, **When** acesso a listagem, **Then** vejo apenas os primeiros 50 livros
2. **Given** estou na página 1, **When** clico em "Próxima", **Then** vejo os próximos 50 livros
3. **Given** estou na última página, **When** clico em "Próxima", **Then** nada acontece (botão desabilitado)
4. **Given** aplico um filtro, **When** a lista é recarregada, **Then** a paginação é resetada para página 1
5. **Given** não há livros, **When** acesso a listagem, **Then** vejo "Nenhum livro cadastrado" sem paginação

### Edge Cases

- PageCount null: ignorado na soma de páginas lidas
- Livro com múltiplos autores: contado para cada autor no relatório "Livros por Autor"
- Like de livro sem autor: livro sem autor não entra nos rankings de autor
- Paginação com filtros: filtros são aplicados ANTES da paginação
- Co-autoria: livro com 2 autores conta como 1 livro para cada autor no relatório

## Requirements

### Functional Requirements

#### Reports (FR)
- **FR-001**: Sistema DEVE exibir total de páginas lidas (soma de PageCount dos livros com status "Lido" ou "Relido")
- **FR-002**: Sistema DEVE exibir lista de autores ordenada por quantidade de livros (decrescente)
- **FR-003**: Cada autor no relatório DEVE mostrar: nome, total de livros, total de páginas lidas, total de likes
- **FR-004**: Sistema DEVE exibir o autor com mais curtidas (maior soma de IsLiked em seus livros)
- **FR-005**: Relatório DEVE considerar livros com múltiplos autores contando para cada autor individualmente

#### Pagination (FR)
- **FR-006**: GET /api/books DEVE aceitar query params `page` (int, default 1) e `pageSize` (int, default 50, max 100)
- **FR-007**: Resposta DEVE incluir metadados de paginação: totalCount, pageNumber, pageSize, totalPages, hasNextPage, hasPrevPage
- **FR-008**: Frontend DEVE exibir controles de paginação (Anterior/Próximo + números de página)
- **FR-009**: Filtros DEVM ser aplicados antes da paginação
- **FR-010**: Ao mudar filtro, página DEVE resetar para 1

### Non-Functional Requirements

- **NFR-001**: Todos os handlers retornam `Result<T>` com `Valid` conforme Constitution V
- **NFR-002**: Todos os handlers têm testes TDD que verificam `result.Valid` (Constitution II)
- **NFR-003**: Paginação aplicada no banco de dados (Skip/Take), não em memória
- **NFR-004**: API respostas seguem padrão `Result<T>` consistente


## Ambiguities & Decisions

1. **"Total de páginas lidas" inclui "Relido"?**
   → **Decisão**: Sim. "Relido" também é uma leitura completa, então as páginas contam.

2. **"Livros por autor" mostra todos os autores ou só com livros?**
   → **Decisão**: Apenas autores que possuem pelo menos 1 livro (autores sem livros não aparecem).

3. **"Autor com mais curtidas" — ranking ou apenas o primeiro?**
   → **Decisão**: Retornar lista ordenada por total de likes (decrescente), destacando o primeiro como "top author". Limitar a top 10.

4. **Paginação: 50 é default ou fixo?**
   → **Decisão**: Default 50, máximo 100 (configurável via query param pageSize).

6. **Like de livro em co-autoria — conta para todos os autores?**
   → **Decisão**: Sim. Um livro curtido com 2 autores conta como like para ambos.

7. **ReportDto existente deve ser extendido ou criar novos endpoints?**
   → **Decisão**: Extender o ReportDto existente com novos campos. Um único endpoint /api/reports.

## Data Model Changes

### ReportDto (atualizado)
```csharp
public record ReportDto(
    int TotalBooks,
    int TotalRead,
    int TotalPagesRead,           // NOVO
    List<GenreReportDto> BooksByGenre,
    List<GenreReportDto> GenresByLikes,
    List<AuthorReportDto> BooksByAuthor,     // NOVO
    List<AuthorReportDto> TopAuthorsByLikes  // NOVO (top 10)
);

public record AuthorReportDto(     // NOVO
    Guid AuthorId,
    string AuthorName,
    int BookCount,
    int TotalPagesRead,
    int LikeCount
);
```

### GetAllBooksQuery (atualizado)
```
+ int PageNumber = 1
+ int PageSize = 50
```

### Retorno de GetAllBooks (novo envelope paginado)
```json
{
  "valid": true,
  "data": {
    "items": [ ... books ... ],
    "totalCount": 150,
    "pageNumber": 1,
    "pageSize": 50,
    "totalPages": 3,
    "hasNextPage": true,
    "hasPrevPage": false
  },
  "messages": [],
  "statusCode": 200
}
```

## API Contract

### GET /api/reports (atualizado)

Response 200:
```json
{
  "valid": true,
  "data": {
    "totalBooks": 50,
    "totalRead": 30,
    "totalPagesRead": 12500,
    "booksByGenre": [ ... ],
    "genresByLikes": [ ... ],
    "booksByAuthor": [
      { "authorId": "guid", "authorName": "Machado de Assis", "bookCount": 10, "totalPagesRead": 3200, "likeCount": 8 },
      { "authorId": "guid", "authorName": "Clarice Lispector", "bookCount": 7, "totalPagesRead": 1800, "likeCount": 5 }
    ],
    "topAuthorsByLikes": [
      { "authorId": "guid", "authorName": "Machado de Assis", "bookCount": 10, "totalPagesRead": 3200, "likeCount": 8 }
    ]
  },
  "messages": [],
  "statusCode": 200
}
```

### GET /api/books (atualizado com paginação)

```http
GET /api/books?genreId={guid}&authorId={guid}&readingStatus=Lido&page=1&pageSize=50
```

Response 200:
```json
{
  "valid": true,
  "data": {
    "items": [ ... books (até 50) ... ],
    "totalCount": 150,
    "pageNumber": 1,
    "pageSize": 50,
    "totalPages": 3,
    "hasNextPage": true,
    "hasPrevPage": false
  },
  "messages": [],
  "statusCode": 200
}
```

## Success Criteria

- **SC-001**: Relatório exibe total de páginas lidas, livros por autor e autor com mais curtidas
- **SC-002**: Listagem de livros mostra no máximo 50 por página com navegação funcional
- **SC-003**: Filtros e paginação funcionam juntos (filtros resetam página para 1)
- **SC-004**: Todas as respostas da API seguem `Result<T>` com `Valid`
- **SC-005**: 100% dos handlers têm testes que passam para `result.Valid`
- **SC-006**: Nenhuma migration quebra dados existentes (mudanças são aditivas)

## Assumptions

- Paginação default de 50 itens por página, máximo 100
- Livro em co-autoria conta para todos os autores nos relatórios
- Livro sem pageCount (null) é ignorado na soma de páginas lidas
- O envelope paginado é um novo tipo `PagedResult<T>` que pode ser reutilizado
