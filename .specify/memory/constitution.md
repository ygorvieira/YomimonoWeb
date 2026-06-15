<!--
Sync Impact Report
- Version: 0.0.0 → 1.0.0 (initial adoption)
- New constitution created from template
- Added all 5 principles, tech stack section, workflow section, governance
- Templates requiring updates: none (first constitution)
-->
# Yomimono Constitution

## Core Principles

### I. .NET 10 Stack
All code MUST target .NET 10. Nullable enabled MUST be on (`<Nullable>enable</Nullable>`).
ImplicitUsings MUST be enabled. Latest C# features (primary constructors, collection
expressions) SHOULD be used where appropriate. All projects MUST follow the established
solution structure (Domain → Application → Infrastructure → Api).

### II. TDD (NON-NEGOTIABLE)
TDD is mandatory for all features. The cycle MUST be: tests written first → user
approves → tests fail → then implement the production code. Red-Green-Refactor cycle
MUST be strictly enforced. Every handler MUST return `Result<T>` and ALL handler tests
MUST assert `result.Valid` is true. No production code without a preceding failing test.

### III. API Integration Tests
ALL API endpoints MUST have integration tests. Every new endpoint requires: contract
tests for request/response format, integration tests for the full flow (success + error
scenarios), and auth flow tests. Changes to existing endpoints MUST include updated
tests. Tests MUST use the real `Result<T>` response structure.

### IV. CI/CD with GitHub Actions
Every push MUST pass CI pipeline on GitHub Actions. The pipeline MUST include:
`dotnet build` (all projects), `dotnet test` (backend tests), `npm test` (frontend
tests), and lint/format checks. No merge SHOULD be allowed if CI fails. Pipeline
configuration MUST live at `.github/workflows/`.

### V. CQRS with Result\<T\>
All handlers MUST return `Result<T>` as defined in `Yomimono.Application.Common`.
Commands and Queries MUST be separated via MediatR. The `Result<T>` type MUST include:
`Valid` (bool), `Data` (T | null), `Messages` (string[]), `StatusCode` (int). Domain
entities MUST inherit from `BaseEntity` (Id, CreatedAt, UpdatedAt, DeletedAt).

## Technology Stack & Constraints

| Category | Technology |
|---|---|
| Runtime | .NET 10 (ASP.NET Core) |
| Database | PostgreSQL 16 (Alpine) |
| ORM | Entity Framework Core 10 + Npgsql |
| CQRS | MediatR 14 |
| Auth | ASP.NET Core Identity + JWT Bearer |
| Frontend | Angular 18 (standalone) |
| Backend Tests | xUnit + Moq + Shouldly |
| Frontend Tests | Jasmine + Karma |
| Containerization | Docker + Docker Compose |
| Proxy | Nginx (reverse proxy for API + SPA) |

All containers MUST use Alpine-based images where available. PostgreSQL MUST use port
5433 externally to avoid conflicts. Backend MUST listen on port 8080 internally.
Frontend MUST be served via Nginx on port 80 internally.

## Development Workflow

Git feature branch flow MUST be used. All work MUST follow this order:
1. Write tests (fail) → Implement code → Verify tests pass
2. Run full test suite locally before committing
3. Push branch → CI runs on GitHub Actions
4. Create Pull Request → Code review required
5. Merge only after CI passes and review is approved

Commits MUST be atomic and descriptive. Constitution compliance MUST be verified on
each PR. Complexity must be justified — if a pattern adds more than 3 layers or 5
files for a single endpoint, document the rationale.

## Governance

This constitution supersedes all other practices. Amendments require: documented
rationale, team approval, migration plan for existing code. The constitution version
follows semantic versioning:
- MAJOR: Backward incompatible governance/principle removals
- MINOR: New principle/section added or materially expanded guidance
- PATCH: Clarifications, wording, typo fixes

All PRs/reviews MUST verify constitution compliance. Non-compliance MUST be flagged
and documented before merge. Use AGENTS.md for runtime development guidance.

**Version**: 1.0.0 | **Ratified**: 2026-06-14 | **Last Amended**: 2026-06-14
