# MOD-030J — Anonymous Public physical retirement

> Status: COMPLETE — 2026-11-12  
> Approval: explicit owner approval  
> Rollback: deploy the previous build

## Completed scope

- Removed `MiniStore.Web/Controllers/PublicController.cs`.
- Removed `MiniStore.Web/Views/Public/Index.cshtml` and `Pricing.cshtml`.
- Removed `Public:Index` and `Public:Pricing` from the reversible presentation-cutover catalog and removed the obsolete
  `Public` AngularDefault setting.
- Added exact anonymous GET/HEAD legacy redirects from `/` to `/app/welcome` and from `/pricing` to `/app/pricing`.
- Preserved the complete query string on both routes.

## Preserved boundaries

- `GET /api/v1/public/plans` remains anonymous, read-only and server-authored through `PlatformSaasService`.
- Angular passes only `planId` into registration; registration still reloads and validates current plan authority.
- Account login/register, registration POST, antiforgery, rate limiting, tenant cookies, onboarding and culture switching did
  not change.
- `_PublicLayout` remains because Account registration and Company Onboarding still use it.
- Permission-protected retired routes retain their existing policies; anonymous metadata applies only to the two exact Public
  patterns.

## Verification

- Source-generated inventory: 74 MVC controllers, 402 MVC actions, 190 user-facing views, 200 Razor control rows and 208
  total Razor files.
- Physical retirement: nine presentation actions across eight surfaces and nine Razor views.
- Hosted GET and HEAD probes for `/` and `/pricing` returned exact 302 Angular locations with query preservation.
- Hosted registration selection remained `/app/register?planId=17`; `GET /api/v1/public/plans` returned 200.
- Microsoft Edge live acceptance passed 146 authorized allowlisted GET routes.
- Release built with zero warnings/errors; 1,856 security/inventory checks, 242/242 Angular tests, 137 templates and the
  120-page consistency inventory passed. Bundles remained 337,191/358,400 initial bytes and 81,877/81,920 largest lazy bytes.
- Deployment rollback remains the previous build; Public no longer has a process-local MVC presentation fallback.

