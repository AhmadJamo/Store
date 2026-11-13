# MOD-030M — PlatformAuditEvents retirement readiness audit

> Status: COMPLETE — audit only, 2026-11-13  
> Runtime impact: none  
> Platform MVC deletion approval: not requested

## Source result

- MVC `PlatformAuditEventsController` and `PlatformAuditEventsApiController` both delegate to `AuditEventReadService` for
  the same platform/system query and bounded CSV export.
- Both boundaries use the isolated `PlatformCookie` scheme. View allows Owner, Administrator and Billing; export allows
  Owner and Administrator; sensitive request evidence is server-derived and Owner-only.
- Both export endpoints require antiforgery. Angular sends only query and purpose and cannot request sensitive projection.
- Angular `/app/platform-audit` covers the complete filter, paging, sensitive-display and export workflow. Its session gate
  redirects an unauthenticated operator to `/platform-login`.
- Static API-client tests cover GET search and POST export routes; security regression covers the isolated scheme, policies
  and antiforgery metadata.

## Live session evidence

No stored browser state contains the `MiniStore.Platform` cookie; all available authenticated states contain only tenant
Identity and antiforgery cookies. Read-only hosted probes proved the isolation boundary:

- Tenant-authenticated request to `GET /api/v1/platform/audit-events`: 401.
- Tenant-authenticated request to `/platform/audit-events`: 302 to the Platform login with encoded return URL.
- Anonymous request to the Platform audit API: 401.

No export was created and no audit, retention or business record was mutated.

## Decision

Do not retire `PlatformAuditEventsController` or its Razor view yet. Static parity is strong, but physical deletion requires
an independently captured authorized Platform session proving MVC/API same-scope reads, exact Angular redirect behavior and
the View/Export/Sensitive role separation. A tenant session cannot be reused as substitute evidence.

## Current inventory

No runtime file changed. Counts remain 73 MVC controllers, 400 MVC actions and 207 Razor files; 53 R2 mixed-controller
blockers remain.

## Required future gate

1. Capture a git-ignored `MiniStore.Platform` browser state through the normal Platform login without reading credentials.
2. Prove Owner and at least one non-Owner role behavior for read, export visibility and sensitive projection.
3. Run read-only MVC/API parity and Angular redirect acceptance. Do not create a CSV solely for acceptance.
4. Reconfirm static callers and define the legacy POST retirement contract.
5. Obtain separate owner approval naming the controller/view before deletion.
