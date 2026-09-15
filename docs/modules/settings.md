# Settings
> Status: PARTIALLY IMPLEMENTED | Last reviewed: 2026-09-15

`SettingsController` is restricted by the active-company Admin permission boundary. General, Discount and Inventory settings use singleton keys and rowversions. Inventory settings define safe defaults for new warehouse type, control mode, picking strategy, POS eligibility, location-capacity enforcement and optional exact transfer-location requirements. Effective operational policy can then be adjusted per warehouse.

Document Numbering is the single administration page for wholesale invoice, POS invoice, stock-transfer and journal-entry sequences. Each type supports prefix, suffix, a tokenized layout (`PREFIX`, `NUMBER`, year/month/day), number padding, forward-only next number, reset starting number and Never/Yearly/Monthly/Daily reset, with a live preview. Missing rows receive safe defaults. Tenant/type uniqueness, rowversion and transactional generation protect against duplicate use and concurrent setting edits. The former InvoiceSettings and DocumentNumberSettings paths are retired; the old Invoices URL redirects to the centralized page.

General Settings stores the company default interface language: English (`en-US`) or Arabic (`ar-JO`). A user can override it with the navigation language switch; the choice is stored in an essential, HttpOnly culture cookie. Arabic requests use RTL markup and Bootstrap RTL. Shared navigation, authentication, notifications, delete confirmation and the Settings hub/general screen use centralized localization resources. Remaining legacy feature screens are migrated when they are next changed.

Sources: setting entities, DTOs, services, repository interfaces/implementations/configurations, `SettingsController`, settings views. Number changes affect new sales, transfers and journal entries only; saved documents keep their identifier.

Inventory Access configures branch warehouse operation permissions/priorities, creates POS terminals and assigns each terminal a default and prioritized alternatives.

POS Experience configures each terminal independently. A business preset supplies a safe starting layout for retail, grocery, cafe, restaurant or quick service, while the administrator can save a custom product layout, theme, cart position, accent color, header, grid density, compact cards, touch sizing, search focus and product barcode/price/stock visibility. Settings use SQL Server rowversion concurrency and the screen includes a live preview.

The same terminal screen configures operational order behavior: enabled/default Walk-in, Dine-in, Takeaway and Delivery types, required dine-in table/service reference, guest count, item preparation notes and fast barcode addition. Applying a business profile resets both appearance and workflow defaults; a custom save validates that at least one type is enabled and the default belongs to the enabled set.

## New-company defaults (2026-09-15)
Guided onboarding creates only missing settings and master records. It links the branch sales account, warehouse inventory subaccount, payment accounts and optional tax accounts, then applies the selected POS profile. Every generated value remains editable through its normal Settings or master-data screen.
