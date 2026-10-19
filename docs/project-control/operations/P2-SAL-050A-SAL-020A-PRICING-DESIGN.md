# SAL-020A — Pricing, tax and unit design closure

Date: 2026-10-19

## Outcome

Accepted `ADR-20261019-DETERMINISTIC-SALES-PRICING-TAX-UOM.md`. It defines the compatibility boundary, price-list ownership, supported first rule scopes, deterministic precedence, managed-unit normalization, dated exchange-rate direction, single final rounding order, tax/exemption line snapshots and the explainable result contract.

## Safety

- No source code, schema, migration, configuration, permission, user interface, business data or secret changed.
- Legacy Sale/POS continues to use the existing Product channel prices.
- Purchase supplier-price entities are not reused as sales price lists.
- Customer groups, sales teams, international tax jurisdictions and approval workflows remain explicit later capabilities rather than placeholder fields.

## Next gate

`SAL-020B` may add tenant-safe price-list/rule storage and a read-only evaluator. It must not cut over legacy Sale or create inventory/accounting effects.
