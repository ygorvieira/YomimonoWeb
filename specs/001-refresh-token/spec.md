# Feature Specification: Refresh Token para Autenticação JWT

**Feature Branch**: `001-refresh-token`

**Created**: 2026-06-14

**Status**: Draft

**Input**: User description: "Adicionar autenticação JWT com refresh token para a API de livros"

## User Scenarios & Testing

### User Story 1 - Login com Refresh Token (Priority: P1)

Como usuário da API, quero receber um refresh token junto com o access token ao fazer
login, para poder obter novos access tokens sem precisar reenviar minha senha.

**Why this priority**: Base de todo o fluxo de refresh — sem isso, nada funciona.
Cobre registro e login.

**Independent Test**: Pode ser testado chamando POST /api/auth/login e verificando
que a resposta contém accessToken + refreshToken.

**Acceptance Scenarios**:

1. **Given** um usuário registrado, **When** faço login com email e senha válidos,
   **Then** a resposta inclui accessToken (JWT válido) e refreshToken (string).
2. **Given** um usuário registrado, **When** faço login, **Then** o refreshToken
   está armazenado no banco com validade de 7 dias.
3. **Given** um usuário registrado, **When** faço login novamente, **Then** um novo
   refreshToken é emitido e o anterior é revogado.

---

### User Story 2 - Renovar Access Token com Refresh Token (Priority: P1)

Como usuário com um access token expirado, quero usar meu refresh token para obter
um novo access token, sem precisar fazer login novamente.

**Why this priority**: Caso de uso principal do refresh token — manter a sessão ativa.

**Independent Test**: Pode ser testado chamando POST /api/auth/refresh com um
refresh token válido e recebendo um novo par de tokens.

**Acceptance Scenarios**:

1. **Given** um refresh token válido, **When** chamo POST /api/auth/refresh,
   **Then** recebo um novo accessToken e refreshToken.
2. **Given** um refresh token expirado (>7 dias), **When** chamo POST /api/auth/refresh,
   **Then** recebo 401 Unauthorized com valid: false.
3. **Given** um refresh token já revogado, **When** chamo POST /api/auth/refresh,
   **Then** recebo 401 Unauthorized com valid: false.
4. **Given** um refresh token inválido (string aleatória), **When** chamo
   POST /api/auth/refresh, **Then** recebo 401 Unauthorized com valid: false.

---

### User Story 3 - Revogação de Refresh Token (Priority: P2)

Como usuário, quero poder revogar meus refresh tokens ativos ao fazer logout, para
que ninguém mais possa usá-los.

**Why this priority**: Segurança — importante mas não bloqueia o fluxo principal.

**Independent Test**: Pode ser testado fazendo login, revogando o token, e tentando
usá-lo em /api/auth/refresh.

**Acceptance Scenarios**:

1. **Given** um refresh token válido, **When** chamo POST /api/auth/revoke com o
   token, **Then** o token é marcado como revogado e retorna 200.
2. **Given** um token já revogado, **When** chamo POST /api/auth/revoke novamente,
   **Then** retorna 200 (idempotente).

---

### User Story 4 - Frontend com Auto-Refresh (Priority: P2)

Como usuário do frontend, quero que o sistema renove automaticamente meu access
token quando ele expirar, sem que eu perceba.

**Why this priority**: UX — evita logout forçado durante o uso.

**Independent Test**: Pode ser testado alterando o expiration do token para 1 segundo
e verificando que uma requisição após a expiração ainda funciona (renovação automática).

**Acceptance Scenarios**:

1. **Given** um token expirado, **When** o interceptor faz uma requisição e recebe
   401, **Then** ele tenta renovar com o refreshToken e retenta a requisição original.
2. **Given** um refresh token expirado, **When** o interceptor tenta renovar e falha,
   **Then** o usuário é redirecionado para /login.

### Edge Cases

- Refresh token expirado retorna 401 com mensagem clara
- Refresh token revogado retorna 401 com mensagem clara
- Refresh token de usuário deletado/inexistente retorna 401
- Concorrência: múltiplas requisições simultâneas com token expirado devem resultar
  em apenas uma chamada de refresh
- Frontend: se o refresh token expirou durante a sessão, redirecionar para login

## Requirements

### Functional Requirements

- **FR-001**: Sistema DEVE emitir refresh token junto com access token no
  registro e login
- **FR-002**: Refresh token DEVE ser armazenado no banco com expiração de 7 dias
- **FR-003**: Sistema DEVE permitir renovar access token via refresh token
- **FR-004**: Sistema DEVE revogar refresh token antigo ao emitir um novo
- **FR-005**: Sistema DEVE permitir revogação manual de refresh token (logout)
- **FR-006**: Frontend DEVE renovar access token automaticamente ao receber 401
- **FR-007**: Frontend DEVE redirecionar para login se refresh falhar
- **FR-008**: Múltiplas requisições simultâneas com token expirado DEVEM resultar
  em apenas uma tentativa de refresh (lock)

### Key Entities

- **RefreshToken**: Armazena tokens de refresh no banco com UserId, Token (hash),
  ExpiresAt, CreatedAt, RevokedAt
- **AuthResponse** (atualizado): Agora inclui accessToken + refreshToken em vez de
  apenas token
- **AuthService** (frontend atualizado): Adiciona store/refreshToken, getRefreshToken
- **AuthInterceptor** (frontend atualizado): Adiciona lógica de auto-refresh em 401

### Backend Changes

| Arquivo | Mudança |
|---------|---------|
| `Yomimono.Domain/Entities/RefreshToken.cs` | NOVO - Entidade RefreshToken |
| `Yomimono.Infrastructure/Data/AppDbContext.cs` | Adicionar DbSet\<RefreshToken\> |
| `Yomimono.Infrastructure/Data/Configurations/RefreshTokenConfiguration.cs` | NOVO - EF config |
| `Yomimono.Application/Auth/DTOs/AuthDtos.cs` | Adicionar RefreshTokenResponse com accessToken + refreshToken |
| `Yomimono.Application/Auth/Commands/RefreshTokenCommand.cs` | NOVO - Command |
| `Yomimono.Application/Auth/Commands/RevokeTokenCommand.cs` | NOVO - Command |
| `Yomimono.Application/Auth/Handlers/RefreshTokenCommandHandler.cs` | NOVO - Handler |
| `Yomimono.Application/Auth/Handlers/RevokeTokenCommandHandler.cs` | NOVO - Handler |
| `Yomimono.Infrastructure/Services/TokenService.cs` | Adicionar GenerateRefreshToken, HashToken |
| `Yomimono.Infrastructure/Services/IdentityService.cs` | Atualizar RegisterAsync e LoginAsync |
| `Yomimono.Api/Controllers/AuthController.cs` | Adicionar endpoints refresh + revoke |
| `Yomimono.Infrastructure/Data/Migrations/` | NOVA migration |

### Frontend Changes

| Arquivo | Mudança |
|---------|---------|
| `src/app/models/auth.model.ts` | Adicionar RefreshTokenResponse, atualizar AuthResponse |
| `src/app/services/auth.service.ts` | Adicionar refreshToken(), revokeToken(), armazenar refreshToken |
| `src/app/services/auth.service.spec.ts` | Atualizar testes |
| `src/app/services/auth.interceptor.ts` | Adicionar lógica de auto-refresh em 401 |

### AuthResponse Change

**Atual**:
```json
{ "token": "eyJ...", "email": "...", "userName": "..." }
```

**Novo**:
```json
{ "accessToken": "eyJ...", "refreshToken": "rT3...", "email": "...", "userName": "..." }
```

## Success Criteria

### Measurable Outcomes

- **SC-001**: Usuário consegue fazer login, receber ambos os tokens, renovar o
  access token após expiração, e fazer logout revogando o refresh token
- **SC-002**: Todas as respostas da API seguem o padrão `Result<T>` com `Valid`
- **SC-003**: 100% dos handlers têm testes que passam para a propriedade `Valid`
- **SC-004**: Frontend renova token automaticamente sem perda de dados do usuário

## Assumptions

- Refresh token expira em 7 dias (configurável via appsettings)
- Refresh token é armazenado como hash SHA-256 no banco (não plaintext)
- Ao emitir novo refresh token, o anterior é revogado (rotação)
- Frontend armazena refresh token em localStorage (mesmo local que access token)
- A rota `/api/auth/register` também retorna refresh token
