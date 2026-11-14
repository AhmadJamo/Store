# Retained runtime boundaries

> Verified: 2026-11-14  
> Phase: MOD-031F  
> Scope: non-destructive verification after ordinary MVC presentation retirement

The generated inventory reports zero ordinary MVC presentation surfaces. The remaining 13 controllers are intentional
runtime boundaries rather than unfinished Angular pages. Their Angular/API counterparts exist, but the controllers still
own rollback, cookie issuance, public tokens, downloads or commands that must not be redirected as presentation reads.

| Boundary | Why it remains | Live anonymous evidence |
|---|---|---|
| Account | Identity cookie login/register/logout and access-denied rollback | Login GET redirects to `/app/login` and preserves local `returnUrl`; unsafe POSTs require antiforgery |
| CompanyOnboarding | Tenant setup completion/skip commands | Anonymous GET/POST are challenged through tenant login |
| Dashboard, Home | Authenticated landing/session routing compatibility | Anonymous requests are challenged or normalized without exposing tenant data |
| Settings | Administration commands and organization-logo download rollback | Anonymous requests are challenged; Angular uses the versioned Settings API |
| Subscription | Public plan/checkout rollback and billing transition | Anonymous MVC request preserves the authentication/public-plan boundary; checkout remains non-redirected |
| SupplierIntegrations | Protected credential/outbox commands and XML/JSON downloads | Anonymous access is challenged; Angular/API retains operation-specific permissions |
| SupplierPortal | Token-scoped public workflow and rate-limited denied-event recording | Invalid token returns the generic unavailable page with `no-cache, no-store` |
| Language | POST-only culture compatibility | Anonymous POST without antiforgery returns 400; no GET redirect exists |
| PlatformAccount | Separate Platform cookie issuance/logout | Login GET redirects to `/app/platform-login`; tenant cookie is not accepted as Platform authority |
| Platform | Owner/operator management and payment commands | Anonymous request redirects to `/platform/account/login`, not tenant login |
| PlatformAuditEvents | Isolated Platform audit read/export | Anonymous request redirects to Platform login; export stays POST/download |
| PlatformAuditRetention | Owner-only holds, checkpoint and retention run | Anonymous request redirects to Platform login; commands remain POST-only |

## Verification result

- Live application: `https://localhost:54052`.
- Tenant and Platform login destinations remained distinct.
- Invalid supplier token returned 200 with a generic unavailable response and `Cache-Control: no-cache, no-store`.
- Direct Language, Account logout and Supplier Portal mutation probes without antiforgery returned 400.
- Auth-protected tenant and Platform commands challenged the anonymous caller before reaching business services.
- Release and the full modernization gate remain the authoritative regression evidence.

## Decision

No controller in this list is approved for batch deletion. Removing one requires a separate bounded task that replaces its
non-presentation responsibility, proves the correct tenant or Platform session, and verifies command/download behavior.
Keeping these boundaries does not mean the Angular migration is incomplete: ordinary business presentation is Angular,
while server-owned authentication, public-token and compatibility endpoints remain server responsibilities.
