# Customer return authorization boundary
> Status: Accepted — 2026-10-21

## Decision

Professional sales returns begin from an exact Posted `SalesDelivery`, not from a product picker or from the legacy immediate `Sale`. `CustomerReturnRequest` owns authorization only. Receipt/inspection, inventory disposition, customer credit notes and refunds remain separate downstream documents.

Draft, Submitted and Approved requests consume return eligibility; Rejected and Cancelled requests release it. Cumulative quantity validation runs with creation in the existing Serializable UnitOfWork.

## Consequences

- Users cannot request more than the delivered quantity across simultaneous/open requests.
- Approval is auditable but deliberately has no stock or accounting effect.
- The legacy `SalesReturn` remains unchanged for Sale/POS compatibility.
- `SAL-100B` must consume Approved request lines and decide restock, quarantine or reject before inventory changes.
