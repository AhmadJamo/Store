# Settings, permission and audit entities
> Status: PARTIALLY IMPLEMENTED | Last reviewed: 2026-09-15

`GeneralSettings`, `DiscountSettings` and `InventorySettings` use singleton key/rowversion. GeneralSettings stores the English/Arabic default interface language in addition to company and regional values. InventorySettings supplies defaults for warehouse operating use, inventory control, picking, POS eligibility, capacity and transfer-location rules.

`DocumentSequence` is the single numbering aggregate for wholesale sales, POS sales, stock transfers and journal entries. A tenant/type unique index guarantees one sequence of each type per company. The aggregate validates the prefix, suffix, tokenized format, 1–18 digit padding, forward-only next number and optional yearly/monthly/daily reset. Reset formats must contain enough date tokens to keep generated identifiers unique. RowVersion detects concurrent administrator edits; operational generation runs in the same Serializable transaction as its document.

`Permission` is the global technical catalogue; `TenantRolePermission` selects permissions for a company-owned role. `AuditLog` records entity name/id/action/user/time, created by interceptor but does not retain field values. Review configurations and settings/permissions docs for every change.
