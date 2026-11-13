# MVC retirement cohort boundary

> Date: 2026-11-12
> Status: Accepted for inventory; physical deletion remains approval-gated

## Context

All presentation surfaces run Angular by default, but the MVC code still contains mutations, downloads, authentication
and session routing, validation redisplay paths and shared Razor infrastructure. Removing views by surface name alone can
break retained POST actions or eliminate the proven rollback path.

## Decision

Generate retirement classification from source and divide surfaces into four cohorts:

- R1: presentation-only candidates with every discovered action covered by Angular-default cutover.
- R2: mixed controllers; retain views until their MVC mutations/downloads and validation redisplay dependencies retire.
- R3: retained routing, session, ambiguous-verb or other compatibility boundaries requiring an explicit replacement.
- R4: declared mutation-only compatibility boundaries preserved unless a separate architecture decision replaces them.

No cohort classification authorizes deletion. Every bounded deletion batch must name exact files, preserve or deliberately
replace legacy-link behavior, reconcile cutover configuration, prove no dangling references, pass the full applicable
verification set and obtain explicit owner approval.

## Consequences

- The unified modernization gate now fails when the generated retirement inventory is stale.
- Broad controller/view deletion is blocked for mixed surfaces.
- The first proposed deletion pilot is limited to five read-only inventory surfaces.
- Financial, public, authentication, download and mutation boundaries remain for later explicit decisions.

## First implementation — MOD-030G

The owner separately approved the five read-only inventory R1 surfaces. Their controllers, Index views, active cutover
entries and AngularDefault settings were removed. A central endpoint catalog now preserves legacy GET/HEAD links under
the original permission policies and forwards query strings to Angular. This is intentionally not a second MVC fallback:
rollback deploys the previous commit/build. Financial and public R1 surfaces remain untouched.

## Second implementation — MOD-030H

The owner separately approved the two financial read-only R1 surfaces after confirming their existing API and Angular
replacements. `JournalEntries.Details` and `PurchaseAccountingReconciliation.Index` controllers/views and active cutover
entries were removed. The centralized catalog now supports exact non-Index actions and positive route-ID substitution,
while preserving permission policies and query strings. No posting, reconciliation calculation, repository or database
boundary changed. Anonymous Public remains unapproved and is the only remaining R1 surface.

## Anonymous audit — MOD-030I

The audit found functional Angular/API parity but a materially different routing boundary. Public owns exact anonymous `/`
and `/pricing` paths; the retired-route catalog currently derives conventional controller/action paths and always attaches a
permission policy. Public is therefore classified R1-A and remains active. A later physical retirement must be separately
approved, add explicit anonymous-safe route patterns without weakening protected redirects, preserve query strings and prove
registration/login/session/rate-limit isolation. Shared `_PublicLayout` is retained because Account registration and Company
Onboarding still use it.

## Anonymous implementation — MOD-030J

After explicit approval, the catalog gained optional exact legacy patterns and a nullable permission boundary. A null
permission is used only by the two declared Public patterns and maps explicit anonymous metadata; all existing retired rows
continue to require their original permission policies. `PublicController` and its two views were removed, while Account,
registration, API and `_PublicLayout` remain. This makes deployment of the prior build the rollback mechanism and closes the
last R1/R1-A cohort without authorizing any R2 deletion.

## R2 prioritization — MOD-030K

R2 ordering uses a generated dependency-count signal: mutations × 3, downloads × 2, other boundaries × 4 and Razor files.
This is reproducible triage evidence only; business criticality, validation recovery, session isolation and live fixtures can
override it. Tenant AuditEvents is the next proposed cohort because its single view and legacy export have complete
permission-separated Angular/API replacements. Deletion remains independently approval-gated.

## First R2 execution — MOD-030L

The owner separately approved tenant AuditEvents. Physical retirement removes both MVC actions and the single view, but the
compatibility catalog represents only safe GET/HEAD presentation navigation. It must not translate a POST/download into a
GET redirect. Consequently the old MVC export ends at HTTP 405 while the existing API retains View + Export, antiforgery and
server-side ViewSensitive enforcement. This pattern is specific to an already-proven replacement and does not authorize any
other mixed controller.

## Configuration-only R2 execution — MOD-030O

AnalyticsCenter proves the same safe split for a configuration mutation: only legacy GET/HEAD presentation routes receive
permission-preserving redirects. The old MVC POST is allowed to end at 405; it is never translated into a GET or replayed
against the API. Workspace validation, sharing authority and persistence remain in the Application/API path. This decision
does not authorize stock-moving controllers or any later cohort.

## Next bounded R2 recommendation — MOD-030P

The next low-count comparison selects Recipes ahead of StockTransactions, Subscription and Users. Recipes keeps its
immutable-version mutation inside the shared Application service and the Angular API preserves permission, antiforgery and
validation behavior without stock or accounting posting. Inventory, billing and identity mutations require their own
acceptance evidence. This recommendation does not authorize deletion; MOD-030Q remains approval-gated.

## Query-bound R2 execution — MOD-030Q

Recipes proves the retired catalog can safely move a positive query-bound identifier into an Angular path. The mapping is
explicitly configured, rejects missing/non-positive values with 404, removes only the consumed query key and preserves all
unrelated query values. The old MVC POST ends at 405 and is never replayed. Recipe mutation authority remains in the
antiforgery-protected API and shared Application service. This does not authorize any other query-bound cohort.

## Isolated configuration recommendation — MOD-030R

ProductCategories is preferred over purchase and sales approval rules because category create/status changes remain catalog
configuration and do not choose approvers or transition documents. All three replacements preserve their existing
Application services and server authorization, but workflow-rule retirement requires additional activation/resolution
evidence. This audit authorizes no deletion; MOD-030S remains separately approval-gated.
