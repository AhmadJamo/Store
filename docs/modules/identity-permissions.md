# Identity and permissions
> Status: IMPLEMENTED WITH FOLLOW-UPS | Last reviewed: 2026-10-26

ASP.NET Core Identity owns login credentials. Company authorization uses `TenantRole`, `TenantRolePermission` and `TenantUserRole`; startup and onboarding create Admin/WarehouseManager/Sales/Accountant independently for every tenant. Admin is protected and receives the current permission catalogue. Custom roles may reuse a name in different companies and hold different permissions. User/role screens show only the active company's data and are bilingual.

Sources: `Program.cs`, Identity/Permission seeders, tenant role entities/repositories/service, permission definitions, Infrastructure PermissionService, Web authorization classes, Users/Roles controllers and views, navigation definitions. See `05_PERMISSIONS.md` and `06_SECURITY.md` for risk and change rules.

## Angular account and session flow (2026-10-26)

`AuthenticationApiController` now exposes same-origin login and logout mutations for Angular while ASP.NET Core Identity remains the credential and lockout authority. `TenantAccountAuthenticationService` is shared by MVC and API, so password checking, active-company membership selection, tenant claim creation, onboarding detection and denied-login audit evidence cannot drift between presentations. Anonymous clients may obtain only a server-issued antiforgery token; login is CSRF protected and rate limited, logout additionally requires an authenticated cookie, and API limit rejection uses localized Problem Details rather than an MVC redirect.

Angular provides bilingual RTL/LTR login and access-denied pages, an authenticated route guard, current user/company evidence in the shell and a protected logout action. Expired API sessions clear only in-memory session state and return to login with a validated local return URL. Passwords and access tokens are never written to local/session storage; the existing HttpOnly Identity cookie remains authoritative. Registration remains on MVC pending the onboarding slice, and MVC account routes remain the default rollback path.

## Angular tenant users and roles (2026-10-26)

`UsersApiController` and `RolesApiController` expose the complete operations that currently exist in the MVC administration screens: company-user list/create and tenant-role list/create/edit/delete. Each endpoint uses its exact `Users.*` or `Roles.*` permission, and every POST/PUT/DELETE is antiforgery protected. The APIs return Application DTOs only; permission options contain stable language-neutral codes rather than Domain permission entities. System roles remain read-only, role edits carry rowversion and stale submissions return HTTP 409.

`TenantUserAdministrationService` is shared by MVC and API. It validates the active tenant, subscription user limit, active tenant-owned role and Identity result before binding the new account to the company. Membership, role assignment and the semantic `Security.TenantUserCreated` audit event commit together through the existing `UnitOfWork`; a failed tenant binding compensates by removing the just-created Identity account. Password fields are never returned, persisted in Angular storage or retained after a rejected save. Existing-user edit/delete and invitations are not part of the current MVC surface and remain explicit future work.

Angular `features/identity` supplies bilingual RTL/LTR user and role lists, user creation and role create/edit forms. The Settings overview and navigation show their destinations only when the session exposes the matching View permission. Role permission checkboxes use server-supplied codes, and rejected/concurrent edits preserve non-secret form state for correction. MVC remains the default rollback path.

`P1-AUD-012` records custom tenant-role creation, ordinary updates, permission-set changes and deletion as fixed semantic event types. Role, permission links and the audit event now share one `UnitOfWork` (وحدة العمل), so a failed operation leaves no success event. The payload records the role name, permission count and a stable permission-set fingerprint (بصمة مجموعة الصلاحيات), avoiding a long copied permission list and all user identity data or credentials. System-role protections remain unchanged.

`P1-AUD-013` records rejected company and Platform login attempts, including lockout as a distinct fixed reason code, without storing the submitted username or password. It also records an authenticated request only when endpoint authorization actually returns Forbidden (مرفوض), avoiding audit noise from UI visibility checks. These denied events commit through an independent bounded writer; persistence failure is visible instead of silently losing security evidence. Source IP addresses are reduced to `/24` for IPv4 or `/64` for IPv6 and user-agent text is stored only as a SHA-256 fingerprint.

`P1-AUD-014` adds three tenant permissions: `AuditEvents.View` (عرض سجل التدقيق), `AuditEvents.Export` (تصدير سجل التدقيق) and `AuditEvents.ViewSensitive` (عرض دليل الطلب الحساس المقنّع). MVC and Angular API export endpoints require both View and Export; sensitive request fields are projected only after an explicit server-side authorization check and the Angular request has no sensitive flag. Platform policies remain separate from tenant roles: Owner/Administrator/Billing can view, Owner/Administrator can export, and only Owner can view sensitive request evidence.

`P1-AUD-015` restricts legal holds, checkpoints and retention execution to the Platform Owner through `PlatformAuditRetention`. Database roles independently enforce INSERT-only writing, SELECT-only reading and controlled maintenance deletion. Hosting credentials are configuration secrets; no tenant role or ordinary Platform administrator inherits retention authority.

## Security boundary update (2026-09-13)
User/role mutations are Admin-only in both permission evaluators, even when custom roles retain mutation permission rows. Read permissions remain delegable. Role deletion is POST-only with antiforgery. Bootstrap is opt-in, has no committed password and refuses existing accounts. See security/configuration docs for operator rotation requirements.
