# ADR: Guided, transactional company onboarding

- Date: 2026-09-15
- Status: Accepted

## Context

New SaaS companies previously entered an empty ERP immediately after registration. The owner had to understand the chart of accounts, branch revenue accounts, warehouse inventory accounts, payment settlement, tax, inventory policies and POS profiles before recording a safe transaction.

## Decision

Registration creates a pending `CompanyOnboarding` control-plane record and signs the owner into the new tenant. The next screen collects business type, country, currency, language, fiscal-year start, tax, inventory control and POS use. A versioned template creates the initial accounting and operational records in one Serializable transaction. Existing matching records are preserved, and a resolved onboarding cannot be applied twice.

Owners may skip the template and configure the ERP manually. Both Completed and Skipped resolve the gate. Login accepts an optional company slug and validates it against the authenticated user's active membership before issuing the tenant claim.

## Consequences

- New companies reach a usable ERP without bypassing accounting or warehouse relationships.
- Restaurant, cafe, grocery, quick-service, retail, wholesale and service businesses receive a matching POS recommendation.
- Failed setup rolls back all template records and remains resumable.
- Template changes require a new version and must never overwrite tenant customizations.
- Existing companies are backfilled as Skipped because they predate the wizard and may already be configured manually.
