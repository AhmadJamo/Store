# MOD-030I — Anonymous Public boundary audit

> Status: COMPLETE — audit only, 2026-11-12  
> Runtime impact: none  
> Deletion approval: not granted

## Scope and exact boundary

The remaining presentation-only `Public` surface consists of:

- `PublicController.Index` on the explicit anonymous legacy path `/` and `Views/Public/Index.cshtml`.
- `PublicController.Pricing` on the explicit anonymous legacy path `/pricing` and `Views/Public/Pricing.cshtml`.
- Angular `/app/welcome` and `/app/pricing`, backed by the anonymous read-only
  `GET /api/v1/public/plans` endpoint and `PlatformSaasService`.

No controller, view, route, API, authentication, registration or shared layout was deleted or changed by this audit.

## Parity and dependency findings

| Concern | Existing replacement/evidence | Audit result |
|---|---|---|
| Landing page | Angular `/welcome` with local registration and pricing links | Equivalent presentation route exists. |
| Pricing | Angular `/pricing` reads server-authored `PlanCardDto` values | Pricing authority remains on the server. |
| Plan selection | Angular passes only `planId` to `/register`; registration reloads and validates public plans | No plan authority moved to the browser. |
| Query preservation | Active cutover and prior anonymous acceptance preserve query strings | Must remain an explicit retirement test. |
| Identity/session | Login, registration POST, rate limiting, antiforgery, cookie issue and onboarding are outside `PublicController` | Must remain isolated from any retired redirect. |
| Shared Razor infrastructure | `_PublicLayout` is also used by Account registration and Company Onboarding | The layout is not eligible for deletion with `Public` views. |
| Rollback | Current active cutover can still return to MVC; physical retirement would require previous-build deployment | A later deletion needs an explicit deployment rollback decision. |

## Decision

`Public` is structurally presentation-only and has Angular/API parity, but it is not safe to place in the current retired-route
catalog unchanged. That catalog derives conventional `/Controller[/Index]` paths and always requires a permission policy.
`Public` instead owns the exact anonymous paths `/` and `/pricing`. It is therefore classified as **R1-A**, an anonymous
public boundary requiring a custom route contract rather than an ordinary internal R1 pilot.

The surface may be proposed for a later physical retirement only after separate owner approval. This audit does not approve
or perform that deletion.

## Required gate for a later MOD-030J

1. Extend the retired-route contract to declare exact custom legacy patterns and anonymous authorization explicitly; do not
   weaken authorization for the existing permission-protected retired routes.
2. Preserve GET/HEAD behavior and the complete query string for `/` and `/pricing` while targeting `/app/welcome` and
   `/app/pricing`.
3. Prove that `/Account/Login`, `/Account/Register`, registration POST, rate-limit evidence, culture switching, cookies,
   onboarding and `GET /api/v1/public/plans` remain unchanged.
4. Keep `_PublicLayout` and every shared Account/Onboarding dependency unless a separate source-authoritative audit proves
   they are unused.
5. Run Release, security regression, generated-inventory, Angular and independent anonymous Edge acceptance, including
   direct legacy links, query preservation, registration plan selection and previous-build deployment rollback.

## Verification

- Source inspection confirmed two GET-only `[AllowAnonymous]` actions and no mutation or download in `PublicController`.
- Source inspection confirmed the anonymous public-plans API and server-side registration plan validation.
- Reference inspection confirmed `_PublicLayout` remains shared by Account registration and Company Onboarding.
- The generated retirement inventory now distinguishes this special boundary as R1-A; active runtime counts are unchanged.
- The unified non-browser gate passed: Release built with zero warnings/errors, 1,860 security/inventory checks passed,
  Angular passed 242/242 tests, 137 templates and the 120-page consistency inventory; bundle budgets remained
  337,191/358,400 initial bytes and 81,877/81,920 largest lazy bytes.
- Browser acceptance was not rerun because this audit changed no runtime route or UI; the existing MOD-030E5N anonymous
  cutover/rollback evidence remains the applicable live evidence until a separately approved MOD-030J.

