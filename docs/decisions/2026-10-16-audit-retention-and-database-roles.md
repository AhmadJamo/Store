# ADR — Audit retention, legal holds and database roles

> Status: Accepted  
> Date: 2026-10-16  
> Scope: `P1-AUD-015`

## Context

`AuditEvent` was integrity-sealed and application-append-only, but it had no approved retention execution, legal hold, external checkpoint or database identity boundary. The owner approved seven years for security/administration/financial decision evidence, three years for ordinary operations and ninety days for detailed failed-login evidence. Only the Platform Owner may manage legal holds.

## Decision

1. Keep successful business-event writing inside the existing business transaction. Database separation therefore applies per table privilege: the application writer receives INSERT on `AuditEvents`, the independent reader receives SELECT, and the maintenance identity receives SELECT/DELETE only for controlled retention. Deployment/migration remains separately privileged.
2. Every retention run writes an HMAC-SHA256 signed external checkpoint first, then a signed minimal destruction manifest. It deletes only the exact ID/integrity-hash pairs that remain unheld when rechecked inside the maintenance transaction.
3. Legal holds are mixed-scope records with explicit Tenant/Platform/System scope and UTC range. They are created/released only by the Platform Owner and are themselves semantic audit events.
4. Committed configuration remains disabled and secret-free. Archive directory and signing key are environment secrets. Local Demo may use a same-machine directory; production must use durable off-host storage and actual role-mapped identities.
5. Detailed failed-login events are eligible after ninety days. Security, administration and financial decision actions are eligible after seven years; other semantic events after three years.

## Consequences

- A failed checkpoint, manifest, integrity comparison or legal-hold recheck prevents deletion.
- Destruction manifests contain identifiers, classifications and integrity fingerprints only; they do not copy actor, IP or user-agent evidence.
- The local development fallback connection supports simple startup, but does not prove production identity assignment. SQL integration impersonates members of all three roles to prove the permission matrix.
- Legacy `AuditLogs` remains unchanged until the separate `P1-AUD-016` reconciliation and freeze gate.
