# English and Arabic presentation localization
> Status: Accepted | Date: 2026-09-14

## Context
The ERP must operate in Arabic and English, including correct page direction. New features need one consistent translation path rather than view-specific language switches or translated domain values.

## Decision
Use ASP.NET request localization with `en-US` and `ar-JO`. English source keys are the fallback; Arabic values live in the centralized `SharedResource.ar.resx`. A user's validated ASP.NET culture cookie has first priority. Without a cookie, a custom provider reads the rowversion-protected company default from GeneralSettings and caches it. The shared layout selects LTR/RTL markup and the matching Bootstrap stylesheet.

Persist only the `UiLanguage` enum in domain settings. Keep translations in the Web layer. Language changes use POST with antiforgery, a strict culture allow-list, an HttpOnly SameSite cookie and local-only redirects. New user-facing strings must include their Arabic resource under the repository instructions.

## Consequences
- One Razor view serves both languages and direction changes.
- Users may keep a device-specific language while the company has a different default.
- The database is not queried for the default on every request because the provider caches it and Settings invalidates the cache.
- Legacy pages require incremental conversion to resource lookups; changing one now requires converting its affected text in the same change.
