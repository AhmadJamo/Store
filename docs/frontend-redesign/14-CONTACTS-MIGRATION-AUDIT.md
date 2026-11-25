# 14 — Contacts Customers/Suppliers migration audit

> Status: COMPLETE — FRD-G closed 2026-11-25
> Date: 2026-11-25
> Source of truth: Angular components, API clients, Application services, API controllers and existing regression coverage

## Current surfaces

| Surface | Routes | Current behavior | Main presentation gap |
|---|---|---|---|
| Customer List | `/customers` | Server search, archived filter, server sort, paging and safe saved-view preferences | FRD-G slice 1 complete: shared FilterBar/SummaryStrip/typed DataTable, localized context, filtered-empty and component spec |
| Customer Form | `/customers/new`, `/customers/:id/edit` | Create/update, generated code, receivable and commercial options, row-version recovery and dirty guard | FRD-G slice 3 complete: FormSection/FieldCue/WorkflowFooter, localized context and one action zone |
| Customer Record | `/customers/:id` | Profile, commercial defaults, contacts, addresses and permission-aware Edit | FRD-G slice 5 complete: localized PageHeader context, status/type/code, four DetailSections, recovery retry and component coverage |
| Supplier List | `/suppliers` | Server search, inactive filter, paging and safe saved-view preferences | FRD-G slice 2 complete: shared FilterBar/SummaryStrip/typed DataTable, localized context, filtered-empty and component spec |
| Supplier Form | `/suppliers/new`, `/suppliers/:id/edit` | Create/update, payable-account and named-product selection, row-version recovery and dirty guard | FRD-G slice 4 complete: FormSection/FieldCue/WorkflowFooter, localized context and one action zone |
| Supplier Record | `/suppliers/:id` | Profile, commercial defaults, assigned-product count and permission-aware Edit | FRD-G slice 6 complete: localized PageHeader context, status/product count, two DetailSections, recovery retry and component coverage; no supplier code exists in the API contract |

## Contracts that must not change

- Customer reads/create/edit remain separated by `Customers.View`, `Customers.Create` and `Customers.Edit`.
- Supplier reads/create/edit remain separated by `Suppliers.View`, `Suppliers.Create` and `Suppliers.Edit`.
- Server search, paging, customer sorting, archived/inactive filters and page-context allowlists remain authoritative.
- Saved list preferences remain limited to approved view fields; search text is not persisted.
- Customer receivable accounts, currencies, tax rates and price lists remain name/code selections with internal IDs.
- Supplier payable accounts and product assignments remain name/code selections with internal IDs.
- Row versions, concurrency recovery, dirty guards, tenant scope, validation and mutation protection remain unchanged.
- No delete/archive capability is invented by the redesign. Customer archived state and Supplier active state continue to follow existing Application behavior.

## Migration order

1. Customer List: COMPLETE — FilterBar, SummaryStrip and typed DataTable adopted; application eyebrow localized; filtered-empty/reset behavior and focused coverage preserve sort and saved-view behavior.
2. Supplier List: COMPLETE — proven list hierarchy reused while preserving inactive filtering, paging and preferences.
3. Customer and Supplier Forms: COMPLETE — FormSection, FieldCue and WorkflowFooter adopted without changing commands, option identities or conditional fields.
4. Customer Record: COMPLETE — DetailSection and record context adopted with recovery retry while retaining all facts and Customers.Edit.
5. Supplier Record: COMPLETE — DetailSection and record context adopted with recovery retry while retaining all facts and Suppliers.Edit; no nonexistent supplier code was invented.
6. COMPLETE — parity, permission and presentation acceptance recorded in `acceptance/contacts-pilot-2026-11-25/`; live authenticated API checks remain explicitly external.

## First slice decision

Customer List is first. It exercises the broader list contract—search, archived status, server sorting, paging and safe preferences—without touching create/edit commands. Supplier List follows only after the Customer component test and full gates pass. This avoids a bulk two-module restyle and keeps every rollback bounded to one surface.

## Exit criteria

- Both lists distinguish true empty from filtered empty and retain server query behavior.
- Forms retain every current field, option source, validation, recovery and dirty-state rule.
- Records retain every displayed profile/commercial/contact/address/product fact and permission-aware action.
- EN/AR, RTL/LTR, desktop/390px, keyboard, loading/error/empty/validation/concurrency states pass.
- API methods, DTOs, schema, Application rules and tenant/permission authority do not change.
