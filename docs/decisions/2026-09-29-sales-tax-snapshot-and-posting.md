# ADR: Invoice-level sales tax snapshots
> Date: 2026-09-29 | Status: Accepted

MiniStore applies one optional tax rate to a sale invoice in the current accounting phase. At creation, the sale freezes the selected tax-rate ID, percentage, output-account ID, inclusive/exclusive policy and calculated tax amount. Historical totals and posting therefore do not change if tax master data changes later.

Tax is calculated after line and invoice discounts. Exclusive tax is added to customer settlement. Inclusive tax is extracted from the discounted total; posting separates net revenue, the net-of-tax invoice discount and output tax so the journal remains balanced. Sale posting credits the frozen output-tax account and remains source-unique and idempotent.

This invoice-level model is intentionally narrower than product tax categories or mixed-rate invoices. A future mixed-rate design must add immutable line-level tax snapshots and a controlled migration without reinterpreting existing invoices.
