# ADR: Policy-controlled direct RFQ path

> Status: Accepted  
> Date: 2026-10-13  
> Scope: PUR-200 purchasing sourcing

## Context

The original sourcing aggregate required exactly one approved Purchase Request. That is appropriate for controlled or high-value demand, but it makes routine low-value quotation collection unnecessarily slow. A fast path must not silently bypass tenant permissions, approval policy or the later Purchase Order approval boundary.

## Decision

Purchase sourcing supports two explicit paths:

1. `ApprovedPurchaseRequest`, which retains the existing approved-request source and marks that request as sourced.
2. `DirectRfq`, which has no fabricated Purchase Request and is allowed only after server-side evaluation of a tenant singleton policy.

The direct policy contains an enable switch, a positive maximum estimated amount in company currency, an optional tenant role and an active-supplier requirement. The command additionally requires `PurchaseSourcing.CreateDirect`. The Application service checks all gates again even though the controller is permission-protected. The event freezes the applied amount limit and role so later policy edits do not rewrite the historical decision.

A direct RFQ remains commercial intent. It can collect supplier quotations but cannot create an authorized Purchase Order by itself. Quotation award, order creation, order approval and order confirmation retain their existing independent permissions and state rules.

Both paths freeze response deadline, expected delivery date, buyer reference and estimated amount. Up to ten attachments of 5 MB each are stored as tenant-owned database rows and downloaded only through a permission-protected action. The bilingual print view relies on the browser print dialog for paper output or PDF saving; outbound email and reliable delivery evidence remain PUR-260 work.

## Consequences

- Existing sourcing records are backfilled as approved-request path records; no approval history is invented.
- Optional request and request-line foreign keys are permitted only because direct events have no request source. Tenant-composite constraints and filtered unique indexes remain in place for sourced requests.
- Users blocked by the direct amount limit are sent to the normal Purchase Request path. Missing/disabled policy errors link to policy settings only for authorized administrators.
- Rollback is safe only when no direct RFQ records remain. The migration refuses to make the optional source columns required while direct records exist.
