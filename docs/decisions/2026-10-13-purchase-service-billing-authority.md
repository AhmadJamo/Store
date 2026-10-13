# Purchase service billing authority

## Status

Accepted incrementally for PUR-220 on 2026-10-13.

## Decision

Each Product owns a purchase billing policy: Ordered Quantity, Received Quantity or Service Completion. Any policy that bypasses a Goods Receipt requires an Expense account. A Purchase Order line freezes both values so later product-master edits cannot rewrite an approved commercial document.

Physical receipt remains the only authority allowed to mutate warehouse stock. Service Completion is a separate immutable purchasing document that proves completed quantity without ProductStock, location, tracking, StockMovement or journal effects. Vendor Bill posting will consume the frozen policy and its corresponding authority: order quantity, posted receipt quantity, or recorded service completion.

## Consequences

- Mixed goods and services may share one Purchase Order while retaining distinct operational evidence.
- Goods Receipt screens and services must reject every non-receipt policy and every non-stocked product.
- Service completion requires separate view/create permissions and cumulative quantity validation.
- Vendor Bill Draft/approval/post staging is implemented; policy-aware ordered/service accounting consumption remains required to finish PUR-220.

## Vendor Bill review increment

Vendor Bills now separate Draft entry, matching approval and accounting posting. Attachments belong to the Draft aggregate, and Draft/Approved/Posted rows reserve their receipt allocations. Posting rechecks active supplier invoice identity and requires Approved status. Policy-aware ordered/service sources remain the final increment of this decision.
