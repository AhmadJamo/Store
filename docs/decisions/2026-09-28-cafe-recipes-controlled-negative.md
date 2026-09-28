# ADR: Versioned cafe recipes and controlled negative ingredient stock

> Date: 2026-09-28 | Status: Accepted

## Context

Cafe and restaurant POS products may be prepared on demand rather than held as finished stock. A pizza sale consumes flour, cheese and sauce according to a recipe, while actual kitchen consumption may differ through portion variance, waste, delayed receiving or measurement error. Blocking POS whenever an ingredient reaches zero would stop service, but silently allowing every stock item to become negative would weaken inventory and future weighted-average costing.

## Decision

- A product is either `Stocked` or `PreparedToOrder`. A prepared sale consumes the active recipe instead of the finished product balance.
- Recipes are immutable versions. Saving a change deactivates the previous version, creates the next version and leaves historical versions available. `SaleItem.ProductRecipeId` records the version used by the sale.
- Ingredients remain stocked products. Each product declares a stock unit; recipe lines may use a compatible count, mass or volume unit. Every version freezes both the authored quantity/unit and its converted stock quantity/unit at six-decimal precision, and a later stock-unit mismatch blocks use until stock is reconciled and a new recipe version is created.
- Negative stock remains blocked for normal sales, transfers and ordinary adjustments. It is allowed only for recipe consumption and kitchen variance when `AllowNegativeRecipeConsumption` is enabled on that ingredient.
- Recipe consumption is aggregated per ingredient for the sale, committed in the existing Serializable sale transaction and recorded as `RecipeConsumption`. Extra waste or actual-use variance is recorded explicitly as `KitchenVariance` with a required reason.
- Negative balances remain visible as exceptions that require receiving, count or variance reconciliation. This decision does not define their financial valuation; ACC-001 must add provisional costing and cost-variance settlement before COGS posting.

## Consequences

POS service can continue when an approved staple ingredient is temporarily negative without opening that behavior to all inventory. Recipe history is auditable and future cost calculation can reproduce the formula used by a sale. Batch production, semi-finished goods, actual batch inputs/yields, scales, approval thresholds and automated financial variance posting remain later phases.
