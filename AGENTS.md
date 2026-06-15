<!-- SPECKIT START -->
For additional context about technologies to be used, project structure,
shell commands, and other important information, read the current plan

## Yomimono Project Quick Reference

### Key Commands
- Backend tests: `cd backend && dotnet test`
- Frontend tests: `cd frontend && npx ng test --watch=false --browsers=ChromeHeadlessNoSandbox`
- Build all: `docker compose up --build -d`
- Seed user: `docker compose exec backend dotnet Yomimono.Api.dll seed-user <email> <password>`

### Architecture
- Backend: .NET 10, CQRS with MediatR, EF Core + PostgreSQL, JWT auth
- Frontend: Angular 18 standalone, Jasmine + Karma tests
- Pattern: Result<T> returned by all handlers

### Constitution
See `.specify/memory/constitution.md` for full governance rules.
Key principles: .NET 10, mandatory TDD, API integration tests, CI on GitHub Actions, CQRS with Result<T>.
<!-- SPECKIT END -->
