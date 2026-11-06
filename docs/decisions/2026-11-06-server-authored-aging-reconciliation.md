# ADR — Server-authored aging and reconciliation

Date: 2026-11-06
Status: Accepted

## Decision

AR/AP aging and control-account reconciliation are calculated in the Application layer from tenant-filtered repositories.
Angular is presentation-only. Journal balances are aggregated by IJournalEntryRepository and operating documents retain
their frozen company-currency carrying values.

Vendor bills now freeze payment-term days and due date for new documents. Historical rows remain nullable and use bill date
as a conservative fallback; the migration adds nullable columns and performs no bulk financial-data rewrite.

## Rationale

Client-side aging can diverge through time zones, partial applications, reversals and currency conversion. Reusing current
supplier terms for historical bills would also rewrite aging retrospectively. Server ownership and frozen due dates keep
the report reproducible while the nullable legacy boundary avoids inventing historical evidence.

## Consequences

- The report is read-only under JournalEntries.View.
- Search never changes full reconciliation totals.
- Unassigned control accounts are explicit differences.
- New vendor bills have reproducible due dates; legacy bills retain a documented fallback.
- No new posting, payment or reversal authority is introduced.
