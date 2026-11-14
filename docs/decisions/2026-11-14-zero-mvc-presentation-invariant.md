# ADR: Zero ordinary MVC presentation invariant

## Status

Accepted on 2026-11-14 by MOD-031G.

## Context

The Angular migration retired every ordinary business presentation surface. Thirteen ASP.NET controllers remain because
they own identity/session, onboarding, downloads, billing, supplier-token, language POST or isolated Platform behavior.
A generated count alone did not fail the main acceptance gate when a future ordinary MVC presentation was introduced.

## Decision

The modernization gate runs `scripts/Test-MvcPresentationInvariant.ps1` after regenerating migration evidence. It requires:

- zero ordinary MVC presentation surfaces;
- the exact 13 reviewed runtime boundaries;
- Language as the sole explicit mutation-only compatibility boundary.

Changing the allowlist is an architecture decision, not a routine way to make the gate pass. The generator remains the
source scanner; the invariant consumes its committed generated evidence so stale evidence and architectural drift fail as
separate checks.

## Consequences

- A new MVC business page fails the standard modernization gate.
- Adding or removing a retained boundary requires an explicit review and documentation update.
- Server-owned authentication, downloads and public-token endpoints remain permitted without being mislabeled as incomplete
  Angular presentation.
- Runtime routing, permissions, schemas and business behavior are unchanged.
