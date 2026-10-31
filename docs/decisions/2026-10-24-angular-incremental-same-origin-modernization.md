# ADR: Incremental Angular modernization through the existing same-origin host

## Status

Accepted on 2026-10-24.

## Context

MiniStore is an operational ASP.NET Core MVC modular monolith with mature Application services, cookie authentication, tenant middleware, dynamic permissions, antiforgery, audit and extensive financial/inventory rules. A separate Angular UI is required without rebuilding those authorities or retiring MVC prematurely.

## Decision

Create Angular in the `MiniStore Front-End` workspace inside the main repository. During migration, add versioned JSON endpoints to the existing `MiniStore.Web` host and route Angular through the same origin in production and a restricted development proxy locally. MVC and Angular coexist until verified parity.

Angular owns presentation and workspace state only. Application services remain the use-case boundary; Domain, repositories and UnitOfWork remain unchanged. API endpoints return explicit DTOs and enforce server-side tenant, permission, validation and concurrency rules.

## Consequences

- Existing authentication, tenant isolation and DI remain reusable.
- No duplicated backend composition or second transaction model is introduced.
- MVC provides rollback while Angular slices mature.
- Temporary dual UI maintenance is accepted.
- Extracting a separate API host is deferred until deployment evidence justifies the extra boundary.
- Same-origin cookies and CSRF are the default; long-lived browser tokens are prohibited.
