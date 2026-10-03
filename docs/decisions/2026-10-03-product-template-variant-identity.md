# ADR: Product remains the inventory SKU under optional templates

## Decision
`ProductTemplate` is an optional catalogue grouping only. Every sellable or stockable variant remains a concrete `Product` with its existing ProductId, prices, unit, tracking policy, balances and document history.

## Variant identity
An assigned Product must share the template category and provide a value for every active applicable variant-defining attribute. The Application layer builds a canonical sequence from neutral definition and option codes, hashes it with SHA-256 and stores both the signature and a readable label on Product. A tenant-aware filtered unique index prevents two Products in one template from representing the same combination.

## Consequences
- Templates never own inventory or replace Product foreign keys.
- Existing Products can be grouped or ungrouped without moving stock or rewriting history.
- Attribute changes refresh the signature transactionally and duplicate combinations fail safely.
- Category changes require unassignment first.
- Automatic creation is limited to active selection-based attributes, a server-enforced maximum of 50 combinations and explicit confirmation after preview.
- An assigned source SKU supplies safe product configuration and non-variant values; barcode, stock, recipes and history are excluded.
- Prepared-to-order and non-selection variants remain manual.
