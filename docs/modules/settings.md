# Settings
> Status: PARTIALLY IMPLEMENTED | Last reviewed: 2026-10-25

## Angular migration foundation (2026-10-25)

The first six `MOD-020` slices provide a `/settings` overview and complete Angular write cycles for operating
preferences, bilingual organization identity, ordered custom identity fields, PNG/JPEG logo upload/removal, discount
controls, purchase-matching tolerances, inventory defaults, purchase-sourcing policy, the complete 30-type document-number table, fiscal-period list/create/status operations, currency exchange-rate list/upsert, measurement-unit list/create/deactivate, accounting posting and receivable policy, branch sales-account assignment, and branch/POS inventory access. Authenticated users with `Administration.Access` use explicit GET/PUT/POST plus
logo POST/DELETE endpoints backed only by the existing Application services. The forms preserve rowversion concurrency,
semantic audit, server validation, culture-cache invalidation where applicable, failed input, dirty-state navigation
protection and bilingual status/error presentation without exposing EF entities. Document designs
and the other settings workflows remain on MVC until their own complete-operation parity gates pass.

Organization Identity is now a dedicated bilingual settings screen rather than a small group of fields inside regional settings. It keeps the commercial and registered legal names in English and Arabic, commercial/tax registration numbers, primary and secondary contact details, bilingual address components, country code and the preferred trade/legal display policy. A tenant-scoped PNG/JPEG logo is validated by file signature and capped at 2 MB.

Administrators can add up to 20 ordered custom identity fields for licenses, branch identifiers or other organization-specific facts. Each field has bilingual labels/values, a Registration/Contact/Address/Other section and independent visibility for sales documents, purchasing documents, receipts and document footers. Identity evidence in semantic audit deliberately records only display policy, logo presence and field counts, not names, addresses, contact values or logo bytes.

**Document Designs** is a separate central Settings workspace with complete Angular administration. An administrator can filter/list, inspect, create or clone immutable versions, activate, archive and set the active default. The editor controls English/Arabic/Bilingual output, paper or custom dimensions, orientation and logical margins; brand colors and font; header structure and independent logo/company-name placement; legal/trade/registration/tax/address/contact/custom-field visibility; page numbers, status watermark, QR/barcode and line/tax/total columns; bilingual cover/header/footer/watermark content; and up to 30 ordered visible/hidden/page-breaking standard or custom bilingual sections. A live preview provides immediate visual feedback. Active versions cannot be edited silently: changes are cloned as a newer version. All writes use the existing `DocumentDesignService`, rowversion lifecycle commands return 409 on stale input, and rejected creates retain every field and section.

PDF creation is also centralized rather than implemented inside each screen. `IHtmlToPdfEngine` accepts reusable HTML/page options and `PlaywrightPdfEngine` owns the shared Chromium lifecycle and output settings. Each business module only supplies its encoded content composer. This is the required extension point for future invoices, orders, receipts and reports.

`SettingsController` is restricted by the active-company Admin permission boundary. General, Discount and Inventory settings use singleton keys and rowversions. Inventory settings define safe defaults for new warehouse type, control mode, picking strategy, POS eligibility, location-capacity enforcement and optional exact transfer-location requirements. Effective operational policy can then be adjusted per warehouse.

The Settings hub includes a bilingual **Work procedure catalog** with 62 steps across seven modules: setup and administration, customer management, sales and receivables, purchasing and payables, inventory and warehouses, accounting and close, and analytics and management review. Every step identifies the responsible role, reviewer/approver, required action, completion evidence and accounting/administrative control principle. The catalog emphasizes segregation of duties, groups steps by module and phase, and provides permission-aware links to real controller actions without replacing service or domain validation. Users without the destination permission see an explanatory locked state.

All literal shared-localizer keys in Web C#/Razor must have an Arabic resource entry. Regression coverage also rejects direct English text in common Razor UI elements, case-insensitive duplicate resource keys and hard-coded controller notification/validation messages.

Document Numbering is the single administration page for wholesale invoice, POS invoice, stock-transfer and journal-entry sequences. Each type supports prefix, suffix, a tokenized layout (`PREFIX`, `NUMBER`, year/month/day), number padding, forward-only next number, reset starting number and Never/Yearly/Monthly/Daily reset, with a live preview. Missing rows receive safe defaults. Tenant/type uniqueness, rowversion and transactional generation protect against duplicate use and concurrent setting edits. The former InvoiceSettings and DocumentNumberSettings paths are retired; the old Invoices URL redirects to the centralized page.

General Settings stores regional preferences, including the company default interface language: English (`en-US`) or Arabic (`ar-JO`). A user can override it with the navigation language switch; the choice is stored in an essential, HttpOnly culture cookie. Arabic requests use RTL markup and Bootstrap RTL. Shared navigation, authentication, notifications, delete confirmation and the Settings hub/general/identity screens use centralized localization resources. Remaining legacy feature screens are migrated when they are next changed.

General Settings also defines the maximum decimal places shown for money and quantities. Purchase screens respect those company choices but suppress insignificant trailing zeros, so configured precision remains available without displaying `5.0000` for a whole-number quantity. Formatting affects presentation and HTML number values only; persisted precision and calculations remain unchanged.

Decimal validation limits are parsed with invariant culture while displayed messages and layout follow the selected UI language. Arabic requests therefore validate dot-based HTML decimal values safely, including document-design dimensions and watermark opacity.

Sources: setting entities, DTOs, services, repository interfaces/implementations/configurations, `SettingsController`, settings views. Number changes affect new sales, transfers and journal entries only; saved documents keep their identifier.

`P1-AUD-012` writes one semantic event in the same database save for Accounting posting accounts, non-personal General operating values, Discount controls, Inventory defaults, Purchase Matching tolerances and Purchase Sourcing policy. General-setting evidence deliberately excludes company names, phone and address. Allowlisted payloads remain scalar and bounded; passwords, tokens, integrity keys, attachments and raw requests are never accepted.

Inventory Access now has a complete Angular read/write workflow. It configures every branch warehouse operation permission and priority, creates POS terminals only from server-authorized branch/POS warehouses, and assigns each terminal a default or prioritized alternative. The API delegates all validation and persistence to `InventoryAccessService`; failed mutations preserve the relevant form and the server remains authoritative for branch, warehouse and terminal boundaries.

POS Experience configures each terminal independently. A business preset supplies a safe starting layout for retail, grocery, cafe, restaurant or quick service, while the administrator can save a custom product layout, theme, cart position, accent color, header, grid density, compact cards, touch sizing, search focus and product barcode/price/stock visibility. Settings use SQL Server rowversion concurrency and the screen includes a live preview.

Fiscal Periods manages non-overlapping accounting date ranges with Open, Soft Closed and Closed states. Its Angular screen now provides the complete list, create and reason-required status-change cycle through the existing Application service; stale status commands return coded HTTP 409 while validation remains 400. Creating the first period activates date coverage: central journal posting then accepts dates only inside an Open period. Companies with no configured periods retain the legacy all-dates-open behavior. Status changes store actor/time metadata; rowversion protects concurrent administration edits.

Currencies and Exchange Rates now has a complete Angular list and add/update cycle. The screen shows the company
currency, accepts a three-letter foreign-currency code, effective date, positive rate and bounded source, and delegates
normalization and the company-currency prohibition to `CurrencyExchangeRateService`. Existing currency/date rows are
updated by the same POST contract; failed validation keeps the user's entered values.

Measurement Units now has complete Angular list, create and deactivate operations. It preserves Count/Mass/Volume/Length
dimensions, positive base-unit factors, zero-to-six decimal precision and normalized unique codes. Built-in units are
visible but cannot be deactivated; custom active units require explicit confirmation and the domain rechecks the rule.

The same terminal screen now has complete Angular GET/PUT parity and configures operational order behavior: enabled/default Walk-in, Dine-in, Takeaway and Delivery types, required dine-in table/service reference, guest count, item preparation notes and fast barcode addition. Applying a business profile resets both appearance and workflow defaults; a custom save validates that at least one type is enabled and the default belongs to the enabled set. Rowversion conflicts return HTTP 409 and rejected saves retain custom edits.

The role-aware Work procedure catalog is also available in Angular as an intentionally read-only reference. The API localizes all 62 server-authored steps and authorizes every destination independently; Angular receives a work URL only when the current user has the destination permission. No artificial mutation was introduced because the catalog is code-defined reference content rather than editable settings state.

## New-company defaults (2026-09-15)
Guided onboarding creates only missing settings and master records. It links the branch sales account, warehouse inventory subaccount, payment accounts and optional tax accounts, then applies the selected POS profile. Every generated value remains editable through its normal Settings or master-data screen.
