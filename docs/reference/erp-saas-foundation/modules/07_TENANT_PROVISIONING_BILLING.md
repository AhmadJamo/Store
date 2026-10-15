# Tenant Provisioning & Billing
Control plane: Tenant, TenantLifecycle, Subscription, Plan, Entitlement, UsageMeter, Invoice/PaymentReference, ProvisioningJob, StampAssignment. Tenant lifecycle: Requested -> Provisioning -> Active -> Suspended -> Offboarding -> Deleted (with retention/legal hold exceptions). Each transition idempotent, auditable, resumable and compensatable.

Provisioning workflow: validate signup -> reserve TenantId -> select stamp -> create logical data boundary -> seed roles/settings/numbering/chart templates as chosen -> verify health -> activate. No user-facing Active before readiness. Payment provider webhook signature verification, replay protection, deduplication and asynchronous reconciliation. Billing events must not directly mutate financial books of tenant customers; platform billing is a separate bounded context. Entitlements enforced server-side; graceful suspension/read-only policy, export on offboarding, usage reconciliation.

Tests: duplicate signup/webhook, failed provisioning resume, partial rollback, trial expiration, over-quota, suspension, plan downgrade, tenant data export, deleted tenant access.
