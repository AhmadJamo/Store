# Settings, permission and audit entities
> Status: PARTIALLY IMPLEMENTED | Last reviewed: 2026-09-13

`GeneralSettings` and `DiscountSettings` use singleton key/rowversion. `InvoiceSettings` provides wholesale/POS prefixes and counters; it has rowversion but no singleton key. `DocumentNumberSettings` provides transfer number counter without rowversion. `Permission` and `RolePermission` model database permission assignment. `AuditLog` records entity name/id/action/user/time, created by interceptor but does not retain field values. Review configurations and settings/permissions docs for every change.
