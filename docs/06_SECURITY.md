# Security assessment
> Status: PARTIALLY VERIFIED
> Source of truth: Static inspection and focused executable regression checks
> Last reviewed: 2026-10-27

## Internal Angular API boundary (2026-10-24)

The same-origin `/api/v1` boundary reuses the Identity application cookie, tenant session middleware and backend permission service. Authentication and authorization failures return RFC Problem Details JSON with a correlation ID instead of redirecting API callers to MVC pages. Unsafe methods remain under the global antiforgery filter; Angular retrieves a request token from an authenticated endpoint and sends it through `X-CSRF-TOKEN`. OpenAPI requires authentication and contains only internal v1 routes. No JWT, local-storage credential, broad CORS policy or client-authoritative permission decision was added.

The MOD-019 presentation cutover filter runs after authorization and cannot supply an arbitrary destination. It accepts
only a validated local Angular base path and a code-owned controller/action allowlist, encodes route values, and ignores
POST plus every unlisted action. Enabling a surface therefore cannot redirect a mutation or replace endpoint permission checks.

Customer and Supplier Angular slices use operation-specific View/Create/Edit permissions on every endpoint. Supplier form options are projected by the Application layer, and submitted account/product identifiers are revalidated by existing services; client filtering is never treated as authorization. Supplier stale-write failures return HTTP 409 while keeping global antiforgery and tenant repository filters intact.

Angular user/role administration repeats exact `Users.*` and `Roles.*` policies on every API operation; navigation visibility cannot authorize a request. Unsafe operations require antiforgery, system roles cannot be mutated, role updates use rowversion with HTTP 409 on stale data, and all supplied role/permission identifiers are resolved server-side. User creation shares one Application contract with MVC, checks the tenant subscription limit and active company role, and commits membership, assignment and sanitized semantic audit evidence in one `UnitOfWork`. If that tenant binding fails after Identity account creation, the new account is removed after clearing the rolled-back tracker state. Passwords are never returned, logged, or stored by Angular and are cleared from the form after rejection.

The Angular settings mutation surface requires authenticated `Administration.Access` for every identity, logo,
preferences, discount and purchase-matching operation. Unsafe PUT/POST/DELETE requests use the shared antiforgery
interceptor/filter. Logo content is limited to 2 MB and validated by PNG/JPEG signature in the Application service.
General, discount and purchase-matching writes carry database rowversions; stale submissions return coded HTTP 409,
and semantic audit remains inside the existing Application save path. The client never treats route visibility as
authorization and never writes EF entities directly.

The Command Center never aggregates unauthorized backend data: its provider list is session-permission filtered and its previews call the already-protected Customer/Supplier APIs with bounded paging. Local Command Center persistence contains only validated navigation/provider identifiers and view mode; restoration drops unknown, unauthorized, oversized or injected values. Record payloads, search terms, arbitrary URLs and permission decisions are not stored.

MOD-009 keeps Customer/Supplier draft payloads in memory only. Saved list preferences use a versioned allowlist of booleans, sort enum and bounded page size; search text and record data are excluded and hostile JSON falls back to defaults. Server error field names must match a simple form-control identifier before DOM focus, preventing selector injection. Keyboard shortcuts invoke the normal validated form submit path and are suppressed while saving.

MOD-010 layout preferences accept only explicit theme, density, sidebar and content-width enum values. They cannot grant permissions, reveal hidden navigation or store tenant/business payloads. Legacy version recovery imports only valid presentation values; reset removes the stored preference object. Mobile navigation remains filtered by the same session-aware registry.

MOD-011 adds stable language-neutral Problem Details codes and correlation IDs without exposing exception types or
stack traces. Angular offers retry only for transient read requests; failed mutations retain user input but require an
explicit new save. Concurrency opens the already-authorized record route in a separate tab and never bypasses the
server row version. Forbidden and permanent failures expose no retry or setup action.

MOD-012 exposes no generic entity lookup. Sales Order and Customer Invoice evidence use separate routes and matching
View policies; cross-document links require the destination View permission. Lists are capped, tenant filters remain in
the reused services/repositories, and the contract contains display snapshots rather than mutable entity instances.

MOD-013 action items require both the source View permission and the matching action permission before the server emits
them. Contextual links open read-only document evidence; Angular never performs the mutation. MOD-014 profiles persist
only allowlisted presentation values and reorder an already permission-filtered navigation list.

MOD-015 keeps `AnalyticsCenter.View` on the API and repeats provider permission filtering in the Application service.
Only Customer and Supplier Angular routes are valid drill targets, and export preview displays the bounded returned
dataset without fetching hidden rows or generating a file. MOD-016 filters each worker by its existing module View
permission, returns no exception text or tenant payload, and clearly marks analytics refresh plans as non-delivery.

MOD-017C stores delegation inside the tenant-owned workspace aggregate. Only the owner or
`AnalyticsCenter.Manage` may administer it, and all access endpoints additionally require
`AnalyticsCenter.Customize`. Target users must be active company members; target roles are resolved inside the active
tenant and protected by a tenant-composite foreign key. Revoked, expired and not-yet-active grants are excluded.
Workspace discovery evaluates current user and role grants, but execution independently rechecks the provider's source
permission, so delegation cannot elevate data access. Rowversion rejects stale grant/revoke commands.

The Customers API independently applies `Customers.View`, `Customers.Create` and `Customers.Edit` to its matching operations. Sidebar visibility is only guidance; every endpoint reauthorizes. POST/PUT retain global antiforgery, DTO validation and tenant-filtered Application/repository access. Customer form data and row versions are never written to browser storage; stale updates return a correlation-bearing `409` response.

SAL-130A analytics remain read-only and pass through tenant-filtered repositories in the Application layer. Saved workspaces are tenant-owned; personal rows are owner-filtered and shared creation requires a separate management permission rechecked in the Application service. The center filters and executes providers only after checking each provider-declared permission, including during crafted workspace creation. Date windows are capped at two years, module/widget keys are validated against the registered provider catalog, and database constraints prevent duplicate widget keys/orders. Comparison queries reuse the same tenant-scoped provider and filters for the derived prior period.

SAL-120A separates `SalesQuotationTemplates.View` from `SalesQuotationTemplates.Manage`; every mutation is POST plus antiforgery. Tenant query filters and composite foreign keys isolate templates and items. The server reloads the active template, requires the exact template version/item set, enforces required items, quantity and option-group limits, rejects disallowed custom lines and replaces submitted product/unit/tax identifiers with authoritative template values before repricing. This prevents a crafted request from weakening template policy or supplying a stale price.

SAL-110 keeps rule administration, ordinary order confirmation and exception approval as separate permissions. Current-step role membership is rechecked server-side; POST antiforgery protects every decision. Tenant query filters and composite relationships isolate rules/reviews. Per-rule snapshots can prohibit requester self-approval and reuse of one approver across steps, while a filtered database index prevents concurrent duplicate Pending reviews. Blocked or pending evaluations leave the order Draft and cannot mutate inventory or accounting.

WMS-050 reservation writes run inside the existing serializable UnitOfWork. Tenant-scoped source uniqueness, balance and reservation rowversions, positive-quantity constraints and exact-dimension indexes prevent cross-tenant, duplicate and concurrent over-allocation.

SAL-050 preserves StockTransfer source uniqueness but permits multiple SaleOrder allocation records. Every order allocation stores its exact SourceLineId; the Application layer recomputes active totals inside the serializable transaction and rejects quantities above the remaining order line. Separate Reserve and ReleaseReservation permissions plus antiforgery protect mutations. SQL integration proves tenant isolation, release/reallocation restoration, unchanged OnHand/journal counts and one winner when two contexts claim the final unit.

SAL-060 keeps delivery mutation behind separate Create/Post/Cancel permissions and antiforgery. The Application layer ignores client inventory cost and status, reloads the tenant order/reservation/product/warehouse/accounting graph, checks rowversions and cumulative order/reservation limits, and issues only from the reservation's exact warehouse/location. Tracking identities are resolved by the inventory authority. Source-unique journals and movement idempotency keys prevent duplicate cost or physical evidence. SQL integration proves tenant isolation, partial/final reservation consumption, stock/balance reduction, one-time COGS posting and cancellation blocking after posted delivery.

WMS-060 enforces antiforgery and action-specific permissions, separates counter from approver, rejects stale expected balances and results below Reserved, and posts the document, quantity, valuation and movement evidence in one serializable transaction.

WMS-070A requires full dimensional reconciliation before tracking activation, rejects negative stock, constrains serial quantities and uniqueness in SQL, and writes policy, balances and history in one serializable tenant boundary.

## Confirmed findings and fixes
- **Critical — privilege escalation:** Users.Create could assign Admin; Roles.Edit could grant the caller arbitrary permissions through their role. Both permission evaluators now reserve all Users/Roles mutation permissions for current database Admin members. Read access remains delegable. Stored mappings cannot override this rule.
- **High — CSRF:** Roles.Delete mutated state through GET, which global antiforgery deliberately skips. It now requires POST and antiforgery. All five list delete controls now submit POST forms with generated antiforgery tokens; other delete endpoints already required POST.
- **Critical — default credential:** Removed the committed admin password. Bootstrap is disabled by default and requires AdminUser:Enabled plus externally supplied username/email/password. It refuses to promote an existing account. On 2026-10-15 the owner classified the only evidenced local environment and all its data/users as Demo: the exposed account was disabled, nine existing session stamps were rotated, the old value was rejected and a replacement administrator passed login. A separate stale `BootstrapAdmin:Password` key identified by the owner was removed from local User Secrets without exposing its value. Secret-free evidence is in `project-control/operations/P0-SEC-001-CREDENTIAL-CLOSURE.md`; any later-discovered external deployment must repeat the procedure.
- **Local Demo platform ownership:** On 2026-10-17 the owner explicitly authorized promoting the rotated Demo administrator to the existing `PlatformOperatorRole.Owner`. Promotion used the opt-in seeder, its temporary enable flag was disabled afterward, and an antiforgery-protected login was verified to redirect to `/platform`. No username or password was added to committed configuration or documentation.
- **High — password guessing:** Failed login attempts now count toward Identity lockout (5 attempts, 15 minutes). Login POST also has a 10 requests/minute/IP fixed-window limiter and no queue; rejection returns to the login form with retry timing. Existing accounts with LockoutEnabled=false require an operator update.
- **Medium — error disclosure:** Broad catch(Exception) handlers now log the exception server-side and show a generic message. Typed business-validation exceptions remain visible and require review if new infrastructure operations are added to those paths.
- **Critical — cross-company Admin claim:** Controllers no longer trust the global Identity `Admin` role for company administration. Settings, chart accounts, branches, customers, taxes and payment methods require `Administration.Access`, resolved from `TenantUserRoles` for the active tenant.
- **Critical — global role definitions:** Company authorization now uses tenant-owned roles and permission sets. Names are unique only within a tenant, role IDs are resolved against the active company, assignments require active membership, and the protected Admin role cannot be renamed or deleted.

## Verified protections and boundaries
MVC antiforgery, local returnUrl validation, Identity password hashing, HTTPS redirection and production HSTS are enforced. The 2026-10-23 UI audit closed legacy gaps by adding controller-level automatic validation to Accounts, Branches, Payment Methods, Tax Rates, Products, Recipes, Product Stocks, Stock Transactions, Warehouses, Purchases, Suppliers, Company Onboarding and Settings. Regression now fails if any public MVC `[HttpPost]` action lacks explicit or controller-level validation. Inspected active data access uses EF LINQ; no raw SQL found. No confirmed XSS was identified in inspected dynamic row rendering; this is not a penetration-test guarantee.

The language switch is an antiforgery-protected POST, accepts only `en-US` and `ar-JO`, uses `LocalRedirect` after validating the return URL, and stores an essential HttpOnly SameSite=Lax culture cookie. Arabic translation values are output through Razor encoding.

Actionable purchase-error guidance never bypasses authorization. A resolution button is rendered only when the current user satisfies the target screen's existing management permission; otherwise the notification directs the user to an administrator. The destination controller continues to enforce its own server-side permission independently of button visibility.

Successful purchase, inventory and warehouse notifications apply the same boundary to workflow guidance. The next-step description remains visible, but its button is rendered only after authorization against the destination permission; the destination controller independently enforces that permission. The automatic completion filter adds presentation feedback only after successful scoped POST redirects and does not perform or authorize the next mutation.

PUR-200 direct RFQs require the dedicated `PurchaseSourcing.CreateDirect` controller policy and repeat that permission check inside the Application service. The service also evaluates the tenant amount ceiling, optional tenant role and supplier-active requirement before persistence, freezes the applied policy evidence and cannot create or authorize a Purchase Order. Attachments are tenant-owned database rows, file names are reduced to their final segment, each file is capped at 5 MB, each RFQ is capped at ten files and downloads require sourcing view permission.

PUR-210 split awards require `SupplierQuotations.Award`, bind only the nested allocation command and resolve every supplied quotation-line identity inside the current tenant and sourcing event. The Application service rejects duplicates, non-submitted quotes, missing reasons, quantities above a supplier quote and any sourcing line whose awarded total differs from the approved requested quantity. The sourcing event closes in the same Serializable transaction as its immutable allocation evidence. `PurchaseOrders.Create` remains separate and can only materialize supplier-grouped orders from those frozen allocations; split awards cannot be converted into a single-supplier agreement.

PUR-220 service evidence separates `PurchaseServiceAcceptances.View` from `PurchaseServiceAcceptances.Create`. The Application service resolves every supplied line through the current tenant's confirmed Purchase Order, requires the frozen Service Completion policy and caps cumulative completion. Goods Receipt paths independently reject non-receipt policies and non-stocked products, so a crafted request cannot turn a service into warehouse stock.

PUR-250A separates receipt capture, quality inspection and storage permissions. The Application service requires the exact state at every transition, validates every submitted receipt line against the tenant-loaded aggregate and Purchase Order, requires discrepancy reasons and caps shortage evidence. A filtered tenant/order uniqueness boundary prevents competing staged receipts. No client-submitted status or route is trusted, and stock/accounting effects remain inside the serializable storage transaction.

PUR-250B separates fulfillment configuration and delivery-evidence permissions. The Application service resolves product Expense accounts and the tenant-loaded order/line rather than trusting client policy or account values, enforces Draft-only route selection, confirmed-only evidence and cumulative quantity caps. Tenant-composite relationships isolate allocations/evidence. The Goods Receipt service rejects direct-delivery orders even through a crafted URL.

PUR-260 keeps supplier communication and invoice extraction inside tenant-owned aggregates and separate permissions. Preparing a mailto message never asserts Sent or Delivered; delivery requires an evidence reference and replies require a note. Files are name-sanitized and capped at ten per record and 5 MB each. Invoice extraction remains a Pending Review suggestion tied to the exact bill attachment; only an explicit Apply action can change the Draft header, and it cannot approve or post the bill. Supplier delivery confirmations preserve old/new dates and a required reference.

PUR-270 external access uses 256-bit random secrets shown once and persisted only as SHA-256 hashes. Portal capabilities expire, can be revoked and bind one supplier to one RFQ or Purchase Order. API keys are supplier-bound, expiring/deactivatable and exact-scope checked; write endpoints require a maximum-100-character idempotency key, preserve the request fingerprint and reject reuse with different content. Portal and API have independent fixed-window limits. Portal responses disable caching/referrers/indexing. UBL parsing caps input at 2 MB and prohibits DTD/external resolution. Callback delivery accepts only HTTPS public hostnames, disables redirects and proxies, resolves and pins a public address at connection time, and rejects loopback/private/link-local networks. The outbox atomically claims one message for five minutes, keeps a stable idempotency key and caps automatic attempts at ten. External endpoints cannot mutate inventory or post accounting; inbound invoices remain evidence. Tenant filters, composite foreign keys, global secret-hash uniqueness and focused regression checks protect cross-company isolation.

PUR-280 analytics remain read-only and consume tenant-filtered repositories through the Application layer. The date window is bounded to two years, source drill-through links require the destination permission, and retiring Legacy Direct Purchase creation is enforced in both the controller and application service so a crafted POST cannot reopen the old path.

Vendor Bill entry, approval and posting are separated by `VendorBills.Create`, `VendorBills.Approve` and `VendorBills.Post`; match override remains an additional reason-required permission and never implies posting. Attachments inherit the bill tenant boundary, downloads require View, names are reduced to their final segment, count is capped at ten and each file at 5 MB. Supplier invoice identity is rechecked before approval and posting, while rowversion rejects stale commands.

Business data is isolated by an authenticated tenant claim backed by an active `TenantMembership` and active `Tenant`. Middleware validates that membership on every authenticated request, upgrades older cookies with a valid tenant claim and signs out users with no active company. EF global query filters scope reads; the DbContext stamps inserts and rejects changes made without a tenant or against another tenant. Every tenant-owned table has a restrictive tenant foreign key. Focused checks verify all 34 protected entity types and the no-tenant write guard.

PUR-240 background execution does not invent an HTTP identity or disable tenant filters. The singleton tenant context accepts a disposable AsyncLocal company scope only for the worker; each company receives a new dependency-injection/DbContext scope, normal tenant query filters and tenant-stamped writes. The three interactive view/manage/run permissions remain enforced by controller attributes.

Database relationship hardening appends TenantId to every relationship between tenant-bearing records. The local schema has 59 composite tenant foreign keys and no unsafe remaining relationship. A two-company SQL test attempted to change a stock balance to the other tenant; SQL Server rejected it with FK error 547 and the transaction rollback preserved the original row.

Public company registration uses a configurable fixed-window limit that defaults to 20 submissions per hour per connection IP and creates the user, company, owner membership, tenant Admin assignment and trial inside one database transaction. Rejections redirect to the bilingual registration page with the server-calculated retry time instead of exposing a raw HTTP 429 browser page. Platform login has its own cookie, lockout-aware password check, rate limit and role policies, and uses the same friendly rejection flow. Subscription middleware denies tenant ERP routes when the subscription is suspended, cancelled or expired while preserving login, public, platform and subscription-management routes.

The Angular registration API keeps the same independent limiter and adds explicit antiforgery to the anonymous POST; API rejection is localized RFC Problem Details rather than an MVC redirect. MVC and API share `TenantCompanyRegistrationService` for provisioning plus cookie/tenant-claim sign-in, and the API response never returns the Identity user ID or a credential. Guided Complete and Skip require authentication, `Administration.Access` and antiforgery, then delegate to the existing serializable setup service. Angular retains non-secret rejected registration values but clears the password and stores no token or setup payload.

Platform login, plan creation and promotion creation forms emit explicit antiforgery tokens. A live GET/POST regression check covers the separate-cookie login form so missing-token failures do not appear as an unexplained HTTP 400.

SAL-070 customer invoices are tenant-owned at header, line, source-allocation and history level. Composite tenant foreign keys prevent cross-company order, customer, product, unit, pricing, tax and delivery references. Tenant-unique invoice numbers and request keys provide identity/idempotency, rowversion rejects stale lifecycle commands, and Serializable creation rechecks cumulative active quantity. Dedicated View/Create/Approve/Cancel permissions and antiforgery protect every controller mutation. Approval cannot create journals or payments, and the company eligibility policy cannot change after dependent invoices exist without a future controlled transition.

## Remaining risks / deployment work

Angular fiscal-period administration is authenticated and requires `Administration.Access` for list, create and
status change. The API delegates only to `FiscalPeriodService`, keeps global antiforgery enforcement for POST/PUT,
rejects overlaps and invalid transitions server-side, requires a reason for status changes and maps a stale rowversion
to coded HTTP 409 without exposing EF entities.

Angular currency exchange-rate reads and upserts also require `Administration.Access` and use the global unsafe-method
antiforgery contract. The API accepts only the explicit Application DTO and delegates to
`CurrencyExchangeRateService`; server/domain validation normalizes three-letter codes, requires a positive rate and
bounded source, and prevents a redundant rate for the company currency.

Angular measurement-unit list, create and deactivate operations require `Administration.Access`; POST commands remain
under antiforgery. The API delegates to `MeasurementUnitService`, which normalizes and rejects duplicate codes, while
the domain validates dimensions/factors/precision and independently blocks deactivation of built-in units.

- Configure production AllowedHosts; wildcard remains because the deployment hostname is unknown.
- Supply a production SQL connection string with validated TLS certificates; local development configuration trusts the certificate.
- Rate limiting is per process and uses the connection IP. Multi-instance/reverse-proxy deployments need trusted proxy configuration and shared edge limits; do not trust arbitrary forwarded headers.
- `P1-AUD-011` through `P1-AUD-014` implement integrity-sealed semantic events, independent denied-event writing and permission-aware reading/export. `P1-AUD-015` implements the approved 7-year protected, 3-year operational and 90-day detailed-login policy, Owner-only legal holds, a signed external checkpoint, manifest-before-delete retention, and separate writer/reader/maintainer SQL roles and connection hooks. A batch is deleted only after its checkpoint and manifest exist and the maintenance transaction revalidates every ID/hash and legal hold. Local Demo stores its key/path only in User Secrets and produced its first external checkpoint; production must supply real role-mapped identities and off-host durable storage.
- `P1-AUD-016` fixes a 37-control catalog: 28 semantic event controls plus nine named authoritative domain-history controls. Regression fails when a declared semantic event is absent from the catalog. New generic `AuditLogs` writes are disabled by committed configuration, but the table remains readable and a separate restart-only flag safely restores future writes. Demo pre/post checkpoints retained all 710 legacy rows, produced two new checkpoint events and reported zero unexplained gaps or unexpected event types.
- Existing-user role editing, invitations, role data scopes and a before/after authorization audit trail remain future controls.
- Online payment remains unconfigured. The current checkout is a server-calculated quote followed by explicit platform confirmation; a provider adapter, signed webhook validation, idempotency store and refund/dispute handling are required before accepting card payments.
- No deployed HTTP/database penetration test performed. Focused tests use a stub Identity manager, real permission evaluators and action metadata.
- Local health responses expose only aggregate state and never connection details or exceptions. HTTP 500, slow-request and database-readiness failures are recorded server-side; production still needs a protected durable log/metric collector.

## Verification
Run `pwsh -NoProfile -File scripts/verify-local.ps1`. The 2026-10-18 run built with zero warnings/errors, passed 777 focused checks, passed SQL integration and completed 500 health requests with zero failures (p95 16.85 ms, p99 28.35 ms). Demo enables semantic audit and retention only through local User Secrets with separate 32-byte random keys; committed legacy generic writing is disabled and all committed configuration remains secret-free.

## Company-code login selection (2026-09-15)
Password validation runs before company selection. A supplied globally unique tenant slug must resolve to an active membership for that user before the TenantId claim is issued. Empty company code retains default-membership behavior. Errors do not expose another company's records.
WMS-020A uses a dedicated `StockMovements.View` permission for its read-only ledger. Putaway and Relocation retain their existing mutation permissions. Server-side tenant filters, composite tenant foreign keys, antiforgery-protected forms and tenant-unique idempotency keys prevent cross-company linkage and repeated submissions.
WMS-040 availability is read-only under `InventoryBalances.View`. Tenant filters and composite foreign keys isolate dimensions; filtered unique indexes prevent duplicate Unassigned/exact-location rows, and rowversion plus database reservation constraints protect future concurrent allocation.
## MOD-021A product API boundary (2026-10-27)

Product reads and form-option projections remain authenticated and require their matching View/Create/Edit permission.
Create, update and delete use independent server permissions and explicit antiforgery validation. The controller accepts
Application DTOs, delegates to `ProductService`, returns normalized validation/not-found/conflict Problem Details and
never exposes persistence exceptions or EF entities. Tenant query and write guards remain authoritative.

## MOD-021B product-category boundary (2026-10-27)

The versioned category API requires authentication and `Products.View` for reads. Create and status changes both require
`Products.Edit` and explicit antiforgery. The DTO/domain enforce a language-neutral bounded code, duplicate validation
is localized, and the API returns only Application DTOs. Angular visibility is guidance only and cannot grant access.

## MOD-021C product-attribute boundary (2026-10-27)

The versioned attribute API requires authentication and `Products.View` for the definition directory. Definition
creation/status and all per-product value reads/writes require `Products.Edit`; POST/PUT operations explicitly validate
antiforgery. Neutral codes and bounded input are DTO-validated, while `ProductAttributeService` revalidates category
applicability, required/type rules, option ownership and template variant refresh transactionally. No EF entity or
client-computed variant signature crosses the API boundary.

## MOD-021D product-template boundary (2026-10-27)

Template reads require authentication and `Products.View`. Create, assignment/removal, preview and generation require
`Products.Edit` and antiforgery even though preview has no persistence effect. Angular never calculates a trusted
variant identity: `ProductTemplateService` rechecks category/source/template relationships, values/options, neutral
SHA-256 signatures, duplicate combinations and the 50-combination limit, then rebuilds the plan inside generation.

## MOD-021E recipe boundary (2026-10-27)

Recipe list reads require authentication and `Products.View`; editor reads and immutable-version saves require
`Products.Edit`, with explicit antiforgery on save. `RecipeService` revalidates product/ingredient tenancy through
repositories, stocked eligibility, active managed units, matching dimensions and positive quantities transactionally.
Angular receives DTOs only and cannot author trusted conversion snapshots or recipe cost.

## MOD-021F catalog-master boundary (2026-10-27)

The combined tax/payment/branch API requires authentication and `Administration.Access` at controller level; every
create action explicitly validates antiforgery. Explicit Application DTOs project named accounts and avoid exposing
domain/EF entities. Account existence, duplicate payment/branch rules and invariant 0–100 tax validation are repeated
server-side; the Angular permission check only controls navigation visibility.

## MOD-021G warehouse boundary (2026-10-27)

Warehouse list/create-form/edit-form/create/update/delete endpoints require their independent
`Warehouses.View/Create/Edit/Delete` permissions. Every mutation validates antiforgery. Application DTOs expose named
branch/account options while `WarehouseService` repeats SaaS entitlement, tenant/account, duplicate, location/control,
POS-access and deletion constraints; client-side disabled fields are guidance only.

## MOD-022A inventory-ledger boundary (2026-10-27)

`GET /api/v1/stock-transactions` requires an authenticated tenant session and `StockTransactions.View`. The API exposes
no POST, PUT, PATCH or DELETE route, so the Angular ledger cannot mutate historical StockTransaction evidence. Paging,
search, direction/type/date filters and sort columns are bounded by the existing Application/repository path and tenant
query filters. Product/warehouse names and valuation snapshots are DTO projections; anonymous live access returns 401.

## MOD-022B stock-mutation boundary (2026-10-27)

ProductStock and manual-adjustment APIs require authentication, tenant context and operation-specific permissions. POST, PUT and
DELETE require antiforgery; explicit DTOs use invariant non-negative quantity validation and return JSON Problem Details without
exposing entities. Controllers call Application services only. Tenant filtering, serializable UnitOfWork behavior, rowversion,
location/tracking allocation, negative-stock policy and moving-average valuation remain server-owned. The delete endpoint deliberately
preserves the service rejection, so authorization never becomes permission to erase stock history.

## MOD-022C read-only inventory projections (2026-10-27)

Availability and reservation endpoints are authenticated, tenant-filtered, versioned and GET-only. Their independent View
permissions are enforced at the controller, paging is bounded in the Application services and explicit DTOs expose names and
quantities without returning entities. Angular cannot change OnHand, Reserved or Available, and it cannot reserve, consume or
release stock. Existing tenant filters, composite keys, rowversions and reservation concurrency constraints remain authoritative.

## MOD-022D inventory-adjustment command boundary (2026-10-27)

All adjustment endpoints require authentication and tenant context. Create, Count, Approve, Post and Cancel have separate permissions
and antiforgery checks; explicit DTOs validate invariant non-negative counts and named options avoid exposing entities. Controllers
call `InventoryAdjustmentService` only. Domain status checks, creator/counter approval separation, frozen-stock revalidation, reserved
floor protection, serializable UnitOfWork and rowversion remain authoritative. Angular cannot skip a state or author variance/cost.

## MOD-022F warehouse-operation command boundary (2026-10-27)

Location, Unassigned Stock, relocation and physical-ledger APIs require authentication, tenant context and independent permissions;
all POST/PUT commands validate antiforgery. Explicit DTOs validate identifiers, quantities, lengths and idempotency keys, and API
controllers call Application services only. The server owns same-warehouse hierarchy, cycle/duplicate checks, active receive/pick
capabilities, total destination capacity, exact source quantity, serializable transactions, dimensional balances and immutable linked
movement facts. Replaying a putaway/relocation key cannot apply stock twice; Angular cannot grant authority through navigation state.

## MOD-023A Purchase Request command boundary (2026-10-27)

The versioned API requires authentication and tenant context, exposes DTOs only and delegates to `PurchaseRequestService`. Create,
Submit and Cancel require independent permissions plus antiforgery; status commands carry route-matched identity and row-version
evidence. Server-side rules revalidate warehouse, active purchasable product, managed-unit dimension, supplier, positive quantity,
duplicate line, numbering and eligible state. Angular cannot author trusted conversion, approval or inventory/accounting effects.

## MOD-023B approval command boundary (2026-10-27)

Rule and decision APIs require authentication, tenant context, independent permissions and antiforgery. Rule commands validate name,
one-to-three ordered tenant roles, warehouse scope, priority range and row-version activation. Decision commands route-match identity,
bound notes and return forbid when the user lacks the frozen current role. `PurchaseApprovalService` re-resolves tenant roles and
advances/rejects the instance and request transactionally; Angular visibility cannot skip or fabricate an approval step.
