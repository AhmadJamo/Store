# Versioned quotation templates and customer repricing

> Status: Accepted
> Date: 2026-10-22
> Decision: SAL-120A

## Context

Reusable quotation templates must be flexible enough for bundles, options and alternatives without becoming a second pricing engine. Editing an active template in place would also make it impossible to prove what configuration produced an older quotation.

## Decision

Quotation templates are tenant-owned versioned aggregates with Draft, Active and Archived states. An active template is immutable; changing it creates a higher version with the same code. Only one active default is allowed per tenant, and activating a new version archives an older active version with the same code.

Templates store product configuration and policy, not final prices. When a template is applied, the Application service reloads the exact active version, validates required items, quantities, groups and additional-line policy, then calls the existing sales-pricing evaluator for the selected customer and date. The quotation freezes the template identity/content and selected-line metadata together with the normal commercial price, currency and tax snapshots.

Product, unit and tax identifiers submitted by the browser are not authoritative for template lines. They are replaced from the stored template before pricing. Product sellability, active units and measurement dimensions are validated when the Draft is created.

## Consequences

- Historical quotations remain explainable after template archival or a newer version.
- Price-list, currency, customer and tax rules keep one authoritative evaluation path.
- Required/optional/alternative product behavior can evolve without altering the Sales Quotation aggregate lifecycle.
- The template feature creates no inventory or accounting effect.
- Branded PDF layout, customer self-service, signature and payment remain separate capabilities with independent security and integration decisions.

## Rejected alternatives

- Storing fixed prices in the template was rejected because it would bypass customer/date pricing and become stale.
- Editing active templates in place was rejected because it would rewrite the meaning of historical usage.
- Trusting client-submitted product, unit, tax or option values was rejected because crafted requests could evade policy.
- Combining product configuration, PDF layout, portal acceptance, signature and payment in one aggregate was rejected because their lifecycle and external dependencies differ.
