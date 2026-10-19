# CUS-030B — Customer commercial defaults closure

Date: 2026-10-19

## Delivered

- Added prospective payment-term days, preferred configured currency and default tax or reasoned tax exemption to Customer.
- Reused GeneralSettings, CurrencyExchangeRate and TaxRate sources; no duplicate currency or tax master was created.
- Added responsive bilingual create/edit/detail fields and preserved RTL behavior.
- Enforced tenant-composite Customer-to-TaxRate linkage and domain validation for contradictory tax choices.
- Kept all values as future document defaults; no legacy Sale, inventory balance or journal was rewritten.
- Deferred price-list and salesperson/team fields to their owning SAL-020+ capabilities because those masters do not yet exist.

## Database and verification

- Applied additive migration `20261019170222_AddCustomerCommercialDefaults` as local Demo migration 118.
- Existing customers received neutral values only; no currency, tax or exemption was guessed.
- Web build: succeeded with zero warnings and zero errors.
- SecurityRegression: 805 checks passed.
- InventorySqlIntegration: full SQL suite passed.
- EF model drift: no pending changes.

## Rollback

Disable use of the fields in future sales integration first. Do not rewrite posted history. Schema rollback is safe only before later documents begin referencing these defaults; otherwise use a forward corrective migration.
