<!--
Sync Impact Report
- Version: 1.0.0 → 1.1.0 (MINOR: added standardized Gitflow with branch/conventions)
- Modified sections: Development Workflow (fully expanded to Standardized Gitflow)
- Templates requiring updates: none (no .specify/templates/ directory)
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

### VI. Standardized Gitflow (NON-NEGOTIABLE)
All development MUST follow a standardized Gitflow with strict branch naming, commit
conventions, and PR discipline. This principle exists to ensure auditability, trace-
ability, and consistent collaboration across the project.

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

## Standardized Gitflow

### Branch Naming

| Branch Type | Pattern | Example |
|---|---|---|
| Feature | `feature/<initials>-<short-description>` | `feature/yg-book-filters` |
| Fix | `fix/<initials>-<short-description>` | `fix/yg-null-ref-error` |
| Hotfix | `hotfix/<initials>-<short-description>` | `hotfix/yg-security-vuln` |
| Release | `release/<version>` | `release/1.2.0` |

All branches MUST branch from `main`. Feature, fix, and hotfix branches MUST be
deleted after merge. Release branches MAY remain for traceability.

### Commit Conventions

Every commit MUST follow Conventional Commits format:

```
<type>(<scope>): <short-description>

<body> (optional)
```

Allowed types: `feat`, `fix`, `docs`, `style`, `refactor`, `test`, `chore`, `ci`.
Scope examples: `api`, `domain`, `frontend`, `infra`, `specs`.
Breaking changes MUST append `!` before the colon (e.g., `feat(api)!: ...`).

The commit body (when present) MUST explain the *why*, not the *what*. Commits MUST
be atomic: one logical change per commit. Squash commits on merge.

### Pull Request Workflow

1. Create feature/fix branch from `main` following branch naming conventions
2. Write tests (must fail) → Implement code → Verify tests pass
3. Run full test suite (`dotnet test` + `ng test`) locally before pushing
4. Push branch and verify CI passes on GitHub Actions
5. Open Pull Request against `main`
6. Title MUST follow Conventional Commits format
7. Description MUST include: what changed, why, how tested, screenshots (UI)
8. Code review MUST be performed by at least one other contributor
9. Merge ONLY after: CI passes, review approved, no merge conflicts
10. Use squash merge (single commit into `main`)

### Release Flow

1. Create `release/<version>` branch from `main`
2. Bump version numbers, update changelog
3. Run full test suite and CI
4. Open PR from release branch to `main`
5. After merge, tag the merge commit with `v<version>`
6. Delete release branch after tagging

### Complexity Discipline

If a single change adds more than 3 file layers or 5 files across the stack,
document the rationale in the PR description. Constitution compliance MUST be
verified on each PR.

## Governance

This constitution supersedes all other practices. Amendments require: documented
rationale, team approval, migration plan for existing code. The constitution version
follows semantic versioning:
- MAJOR: Backward incompatible governance/principle removals
- MINOR: New principle/section added or materially expanded guidance
- PATCH: Clarifications, wording, typo fixes

All PRs/reviews MUST verify constitution compliance. Non-compliance MUST be flagged
and documented before merge. Use AGENTS.md for runtime development guidance.

**Version**: 1.1.0 | **Ratified**: 2026-06-14 | **Last Amended**: 2026-06-15
