# P1-AUD-012 — Semantic audit coverage evidence

> Status: COMPLETE (مكتملة)
>
> Environment: owner-declared local Demo (بيئة تجريبية محلية صرّح بها المالك)
>
> Date: 2026-10-16

## Implemented boundary

- Tenant role creation, ordinary update, permission-set change and deletion write one semantic `AuditEvent` (حدث تدقيق) in the same transaction as the role operation.
- Accounting posting-account settings, non-personal general operating settings, discount controls, inventory defaults, purchase matching tolerances and direct-RFQ sourcing policy write one event in the same `SaveChanges` (حفظ التغييرات).
- The Application layer owns fixed event codes and the semantic writer call. Controllers do not access EF or create audit rows.
- HTTP request context supplies the request correlation identifier and route template. The event writer still derives the active company and current actor from trusted runtime contexts.
- Allowlisted payloads contain only bounded scalar configuration evidence. Role events retain a permission count and stable set fingerprint instead of a potentially oversized permission list. General-settings audit excludes company names, phone and address; no password, token, key, raw request body or attachment is accepted.

## Fixed event codes

- `Security.TenantRoleCreated`
- `Security.TenantRoleUpdated`
- `Security.TenantRolePermissionsChanged`
- `Security.TenantRoleDeleted`
- `Configuration.AccountingSettingsChanged`
- `Configuration.GeneralSettingsChanged`
- `Configuration.DiscountSettingsChanged`
- `Configuration.InventorySettingsChanged`
- `Configuration.PurchaseMatchingSettingsChanged`
- `Configuration.PurchaseSourcingPolicyChanged`

## Transaction and rollback evidence

- Role persistence now participates in the shared `UnitOfWork` (وحدة العمل); role, permission links and audit event commit or roll back together.
- Settings add/update the event before their existing repository save, so the tracked setting and event use one database save.
- SQL integration creates, permission-updates and deletes a role, changes purchase-matching settings and confirms exactly four ordered, tenant-scoped events with valid HMAC-SHA256 (بصمة موثقة بمفتاح).
- A deliberately missing allowlist rejects the audit write and leaves neither the setting nor a success event.
- Existing foundation checks still prove disabled no-op, transaction rollback, two-company isolation, append-only behavior, platform filtering, duplicate operation rejection and database-round-trip integrity.

## Local secret activation

The Demo web project is enabled only through its local `User Secrets` (مخزن الأسرار). Verification reported
`Enabled=true`, active version `v1` and a 32-byte random key. The key value is not printed here, is not present in
Git (نظام حفظ الإصدارات), and committed `appsettings.json` remains disabled with an empty key ring.

## Verification

| Check | Result |
|---|---|
| Web Release build | Passed, 0 warnings and 0 errors |
| SecurityRegression | Passed 756 checks |
| InventorySqlIntegration | Passed, including semantic role/settings event count, rollback, isolation, redaction and integrity |
| EF model check | No pending model changes |
| Demo audit schema inspection | 707 legacy rows, 0 semantic rows before the next enrolled user operation, 113 migrations, 3 trusted checks and 6 enabled indexes |
| Schema impact | None; migration count remains 113 |

## Rollback

Set `AuditEvents:Enabled=false` in protected environment configuration. This stops new semantic writes without
deleting previously recorded evidence. Do not remove or rewrite existing events or the retained `v1` key.

## Explicitly deferred

Authentication failures, denied privileged requests, platform administration and external supplier/integration
coverage belong to `P1-AUD-013`. Reader/export permissions and screens remain `P1-AUD-014`; retention,
archiving and external checkpoints remain later gates.
