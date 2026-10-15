# Retire new Legacy Direct Purchase creation

> Status: Accepted
> Date: 2026-10-15

## Context

The historical `Purchase` aggregate receives inventory immediately while capturing a supplier invoice in the same operation. The completed procure-to-pay workflow separates internal demand, sourcing, Purchase Order, Goods Receipt, Vendor Bill, matching, payment, return and closure evidence. Leaving both creation paths active would offer two conflicting functions for new purchasing and let users bypass approvals, receiving controls and three-way matching.

## Decision

New Legacy Direct Purchase creation is retired. The controller redirects attempted UI creation to the historical list with an actionable link to create a Purchase Request, and the Application service independently rejects creation so a crafted request cannot bypass the decision.

Existing records remain tenant-filtered and visible under the explicit **Legacy direct purchases** label. Their identifiers and accounting/inventory history are not migrated, reinterpreted or deleted. Existing downstream evidence remains available for audit and compatibility. New purchasing starts with Purchase Requests and follows the procure-to-pay aggregates.

The purchasing dashboard compares historical legacy count/value with modern Purchase Order count/value for a selected period. This makes adoption visible without treating unlike documents as one aggregate.

## Consequences

- Users have one authoritative entry point for new purchasing demand.
- Historical inventory and accounting evidence remains intact.
- Existing roles may still contain `Purchases.Create`, but it grants no runtime creation path; `PurchaseRequests.Create` controls the replacement action.
- Re-enabling the old path requires a new explicit architectural decision and code change, not a presentation-only toggle.
