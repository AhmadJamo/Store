# Configuration Engine
Hierarchical typed settings: platform defaults -> plan -> tenant -> branch -> user (only allowed scopes). Define setting keys with schema, validation, owner, version, sensitivity, allowed override scopes, effective date, cache invalidation. Tenant settings include fiscal calendar, costing method, tax rules, stock-negative policy, numbering and approval defaults.

Never retroactively reinterpret posted transactions when settings change. Sensitive secrets go to managed secret store, not settings table. Feature flags separate from business policy; immutable settings snapshots for posted documents when relevant. Audit changes; support draft/approve/publish for high-risk settings.

Tests: precedence, concurrent updates, effective dating, invalid schema, cross-tenant leak, stale cache, retroactive posting behavior, audit.
