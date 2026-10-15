# P1-AUD-013 — Security audit coverage evidence

> Status: COMPLETE (مكتملة)
>
> Environment: owner-declared local Demo (بيئة تجريبية محلية صرّح بها المالك)
>
> Date: 2026-10-16

## Implemented boundary

- Rejected company and Platform login attempts write System/Platform events without the submitted username or password; lockout uses a fixed reason code.
- The authorization middleware records only an authenticated request that actually ends Forbidden (مرفوض), not ordinary permission checks used to hide UI elements.
- Platform company-subscription changes, payment confirmation, plan creation and promotion creation write successful Platform events in the same save/transaction as the business operation.
- Supplier API credential creation/deactivation and portal-access creation/revocation write successful tenant events atomically. Raw API keys, portal tokens, callback secrets/URLs and request bodies are never copied.
- Anonymous supplier API/portal authentication denial writes a System event with a generic reason and no presented credential.

## Independent denied/failed writer

Denied security events use a fresh dependency scope and database context after the rejected operation. Tenant events require an explicit tenant identifier; Platform/System requests reject any tenant identifier. Persistence failure is logged as Critical and rethrown so security evidence cannot disappear silently. The existing `AuditEvents:Enabled` feature flag still makes the writer a no-op when disabled.

## Request evidence

- IPv4 source addresses retain only a `/24` network prefix; IPv6 retains only `/64`.
- User-Agent is stored only as an uppercase SHA-256 fingerprint.
- Correlation identifier and endpoint route template are retained; raw request paths with route values are not copied.
- No username input, password, cookie, authorization header, token, API key or raw payload is stored.

## Verification

| Check | Result |
|---|---|
| Web build | Passed, 0 warnings and 0 errors |
| InventorySqlIntegration build | Passed, 0 warnings and 0 errors |
| SecurityRegression | Passed 761 checks, including masking, hashing, disabled no-op and visible independent-writer failure |
| InventorySqlIntegration | Passed, including exactly two independent denied events with strict System/Tenant scope and redacted request evidence |
| Schema impact | None; migration count remains 113 |

The first sandboxed SQL attempt could not authenticate with Windows (`SSPI`), so the same test was rerun with the normal local Windows identity and passed. No production or non-Demo environment was touched.

## Rollback

Set `AuditEvents:Enabled=false` in protected environment configuration. This stops both atomic and independent new writes without deleting prior evidence. Do not remove historical events or integrity keys.

## Explicitly deferred

Reader/export permissions and screens remain `P1-AUD-014`. Separate SQL identities, retention, archiving and external checkpoints remain `P1-AUD-015`; legacy-stream reconciliation remains `P1-AUD-016`.
