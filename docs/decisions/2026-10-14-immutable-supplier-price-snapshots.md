# ADR: Immutable supplier price snapshots
> Date: 2026-10-14 | Status: Accepted

Supplier price history is append-only and stores a complete after-change snapshot rather than a mutable change log. Every revision freezes supplier, product and purchase-unit identity, minimum order quantity, base price, currency, validity dates, actor, timestamp, change source and all quantity-price-break values. Existing rows receive a baseline snapshot immediately before their first recorded change; newly assigned rows begin with an initial snapshot.

The current `SupplierProductPurchasingInfo` aggregate owns the revision counter and creates snapshots. The Application service records snapshots only when pricing fields or quantity breaks actually change, while bulk and CSV changes remain inside their existing all-or-nothing transaction. Historical records expose no update/delete operation, use restrictive deletion from the owning purchasing row and remain tenant isolated. Posted quotation and order snapshots are not rewritten.

This makes old and new commercial terms independently reconstructable, avoids noisy revisions for non-price edits and preserves one price-history authority across manual, bulk, CSV and quantity-break workflows.
