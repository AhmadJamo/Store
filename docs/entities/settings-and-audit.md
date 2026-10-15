# Settings, permission and audit entities
> Status: PARTIALLY IMPLEMENTED | Last reviewed: 2026-10-16

`GeneralSettings`, `DiscountSettings` and `InventorySettings` use singleton key/rowversion. GeneralSettings stores the English/Arabic default interface language in addition to company and regional values. InventorySettings supplies defaults for warehouse operating use, inventory control, picking, POS eligibility, capacity and transfer-location rules.

`FiscalPeriod` is a tenant-owned accounting control with an inclusive start/end range, Open/SoftClosed/Closed state, status-change reason/user/UTC timestamp and rowversion. Periods are administered through Settings; Application-layer validation prevents overlap and the central journal gateway enforces Open status after the first period is configured.

`DocumentSequence` is the single numbering aggregate for wholesale sales, POS sales, stock transfers and journal entries. A tenant/type unique index guarantees one sequence of each type per company. The aggregate validates the prefix, suffix, tokenized format, 1–18 digit padding, forward-only next number and optional yearly/monthly/daily reset. Reset formats must contain enough date tokens to keep generated identifiers unique. RowVersion detects concurrent administrator edits; operational generation runs in the same Serializable transaction as its document.

`Permission` is the global technical catalogue; `TenantRolePermission` selects permissions for a company-owned role. `AuditLog` records entity name/id/action/user/time, created by interceptor but does not retain field values. Review configurations and settings/permissions docs for every change.

`AuditEvent` is the additive semantic security/operational envelope. It supports explicit Tenant, Platform and System scope; outcome, actor/channel, correlation/causation/operation, resource and reason codes; allowlist-redacted bounded JSON; and a versioned HMAC-SHA256 integrity hash. Tenant scope requires the active matching company, platform/system events require no tenant scope, and AppDbContext rejects update/delete. The feature flag is disabled by default, so the foundation records nothing until an approved Application service is enrolled. Journals, inventory movements and document histories remain their own sources of truth.
