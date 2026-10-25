# ADR-20261025: Reversible presentation cutover

## Status

Accepted on 2026-10-25.

## Context

MiniStore must migrate 189 MVC views and their actions incrementally without changing business rules, bypassing
authorization, or forcing an all-at-once route switch. A view-level inventory alone cannot distinguish safe GET
navigation from mutations, downloads, and actions that have no Angular equivalent.

## Decision

- Generate the migration registry from source with separate view, MVC-action, and Razor-control inventories.
- Configure cutover per MVC surface with `Mvc`, `AngularPilot`, or `AngularDefault`; absent entries resolve to `Mvc`.
- Redirect only GET/HEAD actions that are present in a code-owned allowlist.
- Keep POST and every non-allowlisted action on MVC, including downloads and incomplete workflows.
- Use a local Angular base path validated at startup. Route values are encoded and no external redirect target is accepted.
- Keep all committed surface modes on `Mvc` until parity evidence and explicit cutover approval exist.

## Consequences

Migration waves can be enabled and rolled back independently. Authorization still runs before the MVC action filter,
and Application/domain behavior remains unchanged. The allowlist and generated registry must be updated whenever a
new Angular action reaches pilot status. Final MVC retirement remains gated by MOD-029 and MOD-030.
