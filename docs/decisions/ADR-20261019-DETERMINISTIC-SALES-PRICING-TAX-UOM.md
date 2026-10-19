# Deterministic sales pricing, tax and unit snapshots

Date: 2026-10-19  
Status: Accepted for `SAL-020`

## Context

MiniStore currently stores `SalePrice` and `WholesalePrice` on each concrete Product SKU and the legacy Sale chooses one by channel. Managed measurement units, dated currency rates and invoice-level tax snapshots already exist, but there is no price-list authority, deterministic rule precedence or reusable explanation result. Adding editable price fields directly to Customers or future documents would create competing sources of truth and would make historic prices impossible to explain.

## Decision

### Compatibility boundary

The existing Sale/POS behavior remains unchanged until an explicit cutover gate. Product `SalePrice` and `WholesalePrice` remain valid fallback prices and are not migrated or reinterpreted. Defining a price list or rule alone creates no inventory movement, journal entry, receivable or update to an existing document.

### Price-list ownership and selection

Sales owns tenant-scoped Price Lists and Price Rules. A future Customer link selects one default list; a document may select another only through an explicit authorized override. The transaction records which list was selected and why. A missing list uses the product channel fallback during compatibility mode.

Each list has a neutral code, bilingual-capable display name, currency, active state, validity range, final rounding increment and rowversion. It is archived rather than physically deleted after use. Stored business codes and trace codes remain language-neutral.

### Supported rule scope and computation

The first implementation supports these rule scopes without inventing missing masters:

1. exact Product SKU;
2. Product Category;
3. all eligible products.

Customer groups, sales teams and geographic segments are extension points, not placeholder fields. A rule may use a fixed unit price or a percentage adjustment from the product channel fallback. Rule validity, minimum quantity and optional sale unit are server validated.

### Deterministic precedence

After removing inactive, out-of-date, quantity-ineligible, currency-incompatible and unit-incompatible rules, exactly one rule is selected by:

1. lower explicit Priority number;
2. more specific scope: Product before Category before All Products;
3. higher qualifying minimum quantity;
4. later effective start date;
5. lower immutable rule ID as the final stable tie-breaker.

Ambiguous duplicate rules with the same list, scope target, unit, minimum quantity, validity start and priority are rejected at write time. The evaluator never depends on database return order.

### Unit and quantity authority

The Product managed MeasurementUnit is the stock/base pricing unit. Requested sale quantity is normalized using the immutable factor relationship:

`stock quantity = sale quantity × sale-unit factor ÷ stock-unit factor`.

A product fallback price is per stock unit. Its sale-unit price is multiplied by the same unit-factor ratio. A fixed rule price is per the rule's declared unit; it applies only to that compatible requested unit in the first implementation. Units from different dimensions are rejected. Quantity and factor snapshots are written to future document lines so later unit changes cannot rewrite history.

### Currency and rounding authority

Product fallback prices are company-currency amounts. A Price List has one transaction currency. The existing dated exchange-rate authority is reused: its rate means company-currency amount per one transaction-currency unit. Therefore a company amount converts to transaction currency by division, while transaction totals convert to company currency by multiplication.

The result freezes transaction currency, company currency, rate, rate date and rate source. Intermediate calculations use decimal precision without display rounding. Unit price is rounded once using the selected list increment and `AwayFromZero`; document line/tax totals use the company's monetary decimal places. Quantity uses the selected unit's decimal places. No binary floating-point arithmetic is allowed.

### Tax resolution and snapshot

Tax is resolved after price and commercial discount. A tax-exempt Customer resolves to no tax and freezes the exemption reason. Otherwise the future document's authorized explicit tax selection takes precedence over the Customer default tax; no selection means no tax until product tax categories are introduced by a separately approved requirement.

Each future line freezes tax ID, name, percentage, inclusive/exclusive policy, output account, taxable amount and tax amount. Exclusive tax is added to the line net amount. Inclusive tax is extracted using `gross - gross / (1 + rate/100)`. Tax uses the same final monetary rounding rule. Changes to TaxRate never rewrite a document snapshot.

### Explainable result contract

The pricing evaluator returns, at minimum: product and requested unit; requested and normalized quantity; channel fallback amount; selected list/rule IDs and neutral trace code; rule scope and computation type; unrounded and rounded unit price; transaction/company currencies; exchange-rate snapshot; tax snapshot or exemption evidence; and short explanation parameters for localized UI rendering. The engine stores codes and numeric evidence, not translated explanation text.

## Consequences

- Pricing is flexible through explicit lists/rules but deterministic for identical inputs.
- Future quotations, orders and invoices can snapshot one reusable result instead of recalculating from mutable masters.
- Existing Product prices, units, currency-rate and tax masters are reused.
- Price-below-cost and exceptional-discount approvals remain `SAL-110`; the result can expose warnings but `SAL-020` does not create a generic approval engine.
- International tax jurisdictions, customer groups, commissions and sales-team ownership remain separate approved extensions.

## Rejected alternatives

1. Customer-specific free price fields: competing source of truth and no reusable rule explanation.
2. First database row wins: non-deterministic and unsafe under index/query changes.
3. Rounding after every operation: cumulative pricing and tax drift.
4. Current exchange rate at posting: rewrites commercial intent and prevents document reconciliation.
5. Reusing purchase price-break entities: Purchasing owns supplier terms; Sales needs an independent bounded model.
6. Silently changing legacy Sale/POS: breaks historical and operational compatibility.

## Rollout and rollback

`SAL-020B` adds list/rule storage and a read-only evaluator first. It does not cut over legacy Sale. Rollback disables new-list selection while preserving any stored definitions and snapshots. Once a future document references a list or rule, rollback is forward-only and must not delete referenced evidence.
