# SAL-100C Customer Return Disposition Posting Evidence
> Source completed: 2026-10-21
>
> Database/SQL status: UNVERIFIED — host SQL encryption/SSPI connectivity blocked execution before migration or test mutation.

- Added Posted state, actor/time and immutable history for inspected return receipts.
- Restock restores warehouse/location/tracking quantity at the frozen delivery unit cost.
- Added `CustomerReturnDisposition` stock transaction and `CustomerReturnIn` physical movement evidence.
- Posts one balanced source-unique Inventory debit / COGS credit journal with warehouse and branch dimensions.
- Quarantine remains unavailable custody; Reject/Destroy do not enter inventory. Credit note/refund remain separate.
- Added `CustomerReturnReceipts.PostDisposition`, antiforgery enforcement, Arabic localization, actionable setup links and linked journal evidence.
- Migration 129 is additive; EF reports no pending model changes.
- Release solution build: zero warnings/errors.
- Regression: 962 checks passed, including Posted transition, movement and endpoint permission.
- Disposable SQL scenario was added for exact `0.2 × 10 = 2` Restock/COGS reversal but the suite could not connect (`Cannot generate SSPI context`).
- Demo migration attempt also failed before execution because the SQL instance required encryption unsupported by the current host connection.

Next: repair SQL connectivity, apply migration 129 and rerun the disposable suite; then implement `SAL-100D` customer credit notes without repeating inventory effects.
