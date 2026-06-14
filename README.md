# Yomimono

Sistema de catálogo de livros com API REST em .NET 10 (CQRS) e frontend Angular 18.

## Arquitetura

```
┌──────────────────────────────────────────────────────────┐
│                    Frontend (Angular 18)                  │
│  Nginx:80 → /api/* → backend:8080 → / → index.html      │
└────────────────────────┬─────────────────────────────────┘
                         │ HTTP (proxy reverso via nginx)
┌────────────────────────▼─────────────────────────────────┐
│              Backend (ASP.NET Core 10)                    │
│  ┌──────────┐  ┌──────────────┐  ┌───────────────────┐  │
│  │  Domain   │  │  Application │  │  Infrastructure   │  │
│  │          │  │  (CQRS +     │  │  (EF Core +       │  │
│  │  Book     │  │   MediatR)  │  │   PostgreSQL)     │  │
│  │  User     │  │             │  │                   │  │
│  └──────────┘  └──────────────┘  └───────────────────┘  │
└────────────────────────┬─────────────────────────────────┘
                         │ TCP 5432
┌────────────────────────▼─────────────────────────────────┐
│              PostgreSQL 16 (Alpine)                       │
└──────────────────────────────────────────────────────────┘
```

### Camadas (Backend)

| Camada | Responsabilidade |
|---|---|
| **Domain** | Entidades com regras de negócio (`Book.Create`, `Book.UpdateDetails`, `Book.Delete`), interfaces de serviço de domínio (`IBookUniquenessChecker`), exceções de domínio |
| **Application** | CQRS com MediatR (Commands, Queries, Handlers), DTOs, `Result<T>`, interfaces de repositórios e serviços de aplicação |
| **Infrastructure** | EF Core + PostgreSQL (`AppDbContext`), repositórios, Identity + JWT, serviços de infraestrutura |
| **Presentation** | Controllers da API REST, autenticação JWT Bearer |

## Pré-requisitos

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [Node.js 20+](https://nodejs.org/)
- [PostgreSQL 16+](https://www.postgresql.org/)
- [Docker + Docker Compose](https://www.docker.com/) (para execução conteinerizada)

## Setup rápido

### Com Docker (recomendado)

```bash
docker compose up --build
```

Acessar:
- Frontend: http://localhost
- API: http://localhost:5000
- PostgreSQL: localhost:5432

### Manual (desenvolvimento)

**1. Backend**

```bash
cd backend

# Criar banco PostgreSQL (ajuste connection string em appsettings.json)
createdb yomimono

# Executar migrations e iniciar
dotnet run --project Yomimono.Api
```

**2. Frontend**

```bash
cd frontend
npm install
npx ng serve
```

Acessar: http://localhost:4200

## Configuração

### Connection String (appsettings.json)

```json
"ConnectionStrings": {
  "DefaultConnection": "Host=localhost;Port=5432;Database=yomimono;Username=yomimono;Password=yomimono"
}
```

### JWT (appsettings.json)

```json
"Jwt": {
  "Secret": "sua-chave-secreta-aqui-com-pelo-menos-32-caracteres",
  "Issuer": "Yomimono",
  "Audience": "YomimonoWeb",
  "ExpireMinutes": "120"
}
```

## API

### Autenticação

| Método | Rota | Descrição |
|---|---|---|
| POST | `/api/auth/register` | Cadastro de usuário |
| POST | `/api/auth/login` | Login, retorna token JWT |

### Livros (requer token JWT)

| Método | Rota | Descrição |
|---|---|---|
| GET | `/api/books` | Lista todos os livros |
| GET | `/api/books/{id}` | Obtém livro por ID |
| POST | `/api/books` | Cria novo livro |
| PUT | `/api/books/{id}` | Atualiza livro |
| DELETE | `/api/books/{id}` | Remove livro (soft delete) |

### Exemplo de uso

```bash
# Registrar
curl -X POST http://localhost:5000/api/auth/register \
  -H "Content-Type: application/json" \
  -d '{"email":"user@email.com","password":"123456","userName":"user"}'

# Login
curl -X POST http://localhost:5000/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"email":"user@email.com","password":"123456"}'

# Usar token
TOKEN="eyJ..."
curl -H "Authorization: Bearer $TOKEN" http://localhost:5000/api/books
```

### Result Pattern

Todas as respostas seguem o formato:

```json
{
  "valid": true,
  "data": { ... },
  "messages": ["Operação realizada com sucesso."],
  "statusCode": 200
}
```

## Testes

### Backend (xUnit + Moq + Shouldly)

```bash
cd backend
dotnet test
```

**15 testes** — todos os handlers (Books CRUD + Auth) validam a propriedade `Valid` do `Result<T>`.

### Frontend (Jasmine + Karma)

```bash
cd frontend
CHROME_BIN=/caminho/para/chrome npx ng test --watch=false --browsers=ChromeHeadlessNoSandbox
```

**29 testes** — Service + componentes (BookList, BookDetail, BookForm, Login, Register).

## Estrutura do Projeto

```
YomimonoWeb/
├── backend/
│   ├── Yomimono.Domain/           # Entidades, interfaces de domínio
│   │   ├── Common/                # BaseEntity, DomainException, IBookUniquenessChecker
│   │   └── Entities/              # Book, User
│   ├── Yomimono.Application/      # CQRS, DTOs, handlers
│   │   ├── Auth/                  # Commands, Handlers, DTOs
│   │   ├── Books/                 # Commands, Queries, Handlers, DTOs
│   │   └── Common/                # Result<T>, interfaces
│   ├── Yomimono.Infrastructure/   # EF Core, Identity, JWT, repositórios
│   │   ├── Data/                  # AppDbContext, Configurations
│   │   ├── Repositories/          # BookRepository, BookUniquenessChecker
│   │   └── Services/              # IdentityService, TokenService
│   ├── Yomimono.Api/              # Controllers, Program.cs
│   │   └── Controllers/           # BooksController, AuthController
│   ├── Yomimono.Api.Tests/        # Testes xUnit
│   │   └── Handlers/              # Testes de todos os handlers
│   └── Yomimono.sln
├── frontend/
│   ├── src/
│   │   ├── app/
│   │   │   ├── components/        # book-list, book-form, book-detail, login, register
│   │   │   ├── services/          # book.service, auth.service, auth.interceptor
│   │   │   ├── guards/            # auth.guard
│   │   │   └── models/            # book.model, auth.model
│   │   ├── environments/          # environment.ts, environment.prod.ts
│   │   └── styles.css
│   ├── nginx.conf
│   ├── Dockerfile
│   └── package.json
├── docker-compose.yml
└── README.md
```
