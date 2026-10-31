# MOD-002 — Versioned internal API foundation

> Status: COMPLETE  
> Completed: 2026-10-24  
> Database migration: None

## Outcome

The existing `MiniStore.Web` host now exposes a same-origin, versioned internal API foundation for Angular while MVC remains operational. Business rules continue to flow through Application services and existing repositories.

## Contracts

- `GET /api/v1/session`: authenticated user, active tenant, UI culture/direction and granted permission codes.
- `GET /api/v1/security/csrf`: authenticated antiforgery request token and header name.
- `POST /api/v1/security/mutation-probe`: side-effect-free proof that unsafe API methods are protected by antiforgery validation.
- `/openapi/internal-v1.json`: authenticated OpenAPI document filtered to `/api/v1` routes only.
- `PageRequest` and `PageResult<T>`: bounded page size, normalized search and explicit sort allow-list.
- RFC Problem Details: validation and unexpected failures include a correlation identifier and do not expose exception details.

## Security decisions

- Existing Identity cookie authentication is reused; no JWT or browser token storage was added.
- Internal API and OpenAPI authentication failures return JSON `401/403` instead of redirecting to MVC HTML.
- Permission discovery uses the tenant-scoped `IPermissionService`; the controller never accesses EF Core.
- Granted permissions are fetched in one tenant-filtered query.
- Angular obtains a CSRF token at startup and a central interceptor adds it only to unsafe same-origin `/api/` requests.
- Arabic translations were added for new server-facing error titles.

## Verification evidence

| Verification | Result |
|---|---|
| Web + SecurityRegression Release build | Passed, 0 warnings/errors |
| SecurityRegression | Passed 1072 checks |
| Angular production build | Passed; 232.18 kB initial raw bundle |
| Angular Vitest | Passed 2/2 files and 3/3 tests, including CSRF interceptor behavior |
| `npm audit --omit=dev` | 0 production vulnerabilities |
| Live anonymous `GET /api/v1/session` | `401 application/problem+json` |
| Live anonymous `GET /api/v1/security/csrf` | `401 application/problem+json` |
| Live anonymous `GET /openapi/internal-v1.json` | `401 application/problem+json` |

Authenticated browser verification remains part of each future vertical slice; no test credential is stored in source or documentation.

## Rollback

Remove the `/api/v1` controllers, API middleware/configuration, Angular API client files and OpenAPI package reference. No schema or business data rollback is required.
