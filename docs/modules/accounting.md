# Accounting module

> Last reviewed: 2026-11-05

The current Angular accounting presentation includes the chart-of-accounts list/create surface, posted journal-entry detail
routes reached from business documents, and administration-owned accounting setup for fiscal periods, exchange rates and
branch sales-account mappings. The `/accounting` module hub exposes only permitted existing workspaces.

Current limitations remain explicit: the account page is a flat list rather than a managed hierarchy, there is no journal
directory/manual journal workflow, and financial statements are not implemented. UX-009 changed discovery only; it did not
change posting, fiscal-period enforcement, DTOs, schema, permissions or accounting rules.
