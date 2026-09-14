# ADR: Separate SaaS control plane and entitlements

- Date: 2026-09-14
- Status: Accepted

MiniStore separates public marketing, tenant ERP and `/platform` control-center experiences inside the modular monolith. Platform access uses a dedicated cookie and explicit PlatformOperator allow-list; tenant Admin has no implied platform access. Commercial access is represented by bilingual plans, feature flags, numeric limits and one lifecycle-aware subscription per tenant. Application services enforce subscribed limits before creating users or warehouses. Promotion codes are platform-owned, time/plan/usage bounded and produce per-tenant redemption records.

Company registration creates its owner, membership, tenant Admin assignment and trial atomically. Subscription access is enforced in middleware. Checkout persists a short-lived server-calculated quote and redeems its promotion only when an authorized platform operator confirms payment inside a serializable transaction. This manual confirmation is the honest operational boundary until a specific payment provider is chosen; signed webhooks, idempotency, refunds/disputes, 2FA and plan-version migrations remain later phases.
