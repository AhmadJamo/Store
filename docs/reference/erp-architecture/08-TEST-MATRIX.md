# 08 — Integration and Adversarial Test Matrix

| Scenario | Expected invariant | Modules |
|---|---|---|
| Two cashiers sell last unit simultaneously | at most one valid reservation/sale or explicit negative-stock policy | POS, Inventory |
| Same webhook delivered 10 times | one payment and one accounting effect | Commerce, Treasury, GL |
| Click Post twice / retry timeout | one stock/GL posting and stable document number | all posting modules |
| Add cheese 20g twice to pizza BOM | two legitimate distinct BOM lines | Catalog, Manufacturing |
| Same product appears twice on order | both lines retained, separate tax/price/discount snapshots | Sales |
| Receive PO in 3 partial receipts | correct cumulative received; no overreceipt without approval | Purchase, Inventory |
| Bill before receiving goods | correct matching/accrual status | Purchase, AP |
| Return part of delivered order | physical return + proportional credit note without overcredit | Sales, Inventory, AR |
| Modify master price after order confirmation | posted order/invoice retains original price | Catalog, Pricing, Sales |
| Change tax rate after invoice posting | invoice tax snapshot unchanged | Tax, Billing |
| Cross-company foreign ID in command | denied, no data leak or write | all |
| Backdate into locked accounting period | rejected or authorized reopening process | GL |
| Re-run monthly subscription invoice job | one invoice per contract/period | Subscription, Billing |
| Re-run payroll | one approved run posting | HR, GL |
| Crash after DB commit before event publish | outbox eventually publishes, no lost update | Integration |
| Crash after consumer commit before ACK | inbox dedup avoids repeated effect | Integration |
| Out-of-order event versions | defer, reconcile or rebuild projection | Integration, Reporting |
| Two users reserve same sequence simultaneously | unique document numbers within scope | Numbering |
| Reconcile partial payments in different currencies | correct open balance and FX differences | Treasury, AR/AP, GL |
| Reverse posted transfer | compensating moves, no history deletion | Inventory |
| Disable permission in live session | subsequent commands denied | Identity |
| Concurrent editing of same draft | row-version conflict, no silent overwrite | all |
| Warehouse move with lots/serials | correct traceability, no duplicated serial at same physical point | Inventory |
| Duplicate customer/vendor entity roles | same party may be both without data duplication | Parties |
| Negative quantity, NaN, huge decimals, bad UOM | validation, no partial posting | all |
| Imported legacy order referencing deleted customer | migration reports broken reference, never silently discards | Migration |

## Testing layers
- Domain unit tests: state transitions, price/tax/UOM rounding, invariants.
- Application tests: permission, transaction, idempotency, duplicate requests.
- Database integration: real SQL Server constraints, rowversion, unique indexes, migrations.
- Contract tests: event schema/version, consumer backward compatibility.
- End-to-end: quote-to-cash, procure-to-pay, manufacture-to-stock, returns, payroll-to-GL.
- Property-based/fuzz: amounts, decimals, repeated lines, quantities, dates, malformed payloads.
- Load/race: reservation, document number allocation, inventory posting and webhook replay.
- Reconciliation: posted stock ledger vs projections; GL journal vs trial balance; AR/AP subledger vs control accounts.

## Exit criteria
No unexplained ledger mismatch, no duplicate postings, no cross-company leakage, no regressions in existing features, and no unresolved critical security issue. Document known noncritical exceptions explicitly.
