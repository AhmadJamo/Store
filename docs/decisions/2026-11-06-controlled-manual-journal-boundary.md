# ADR: Controlled manual journals use the shared posting and reversal gateway

> Date: 2026-11-06  
> Status: Accepted

## Context

Accountants need manual adjustments, but direct persistence or browser-owned balancing would bypass tenant validation,
fiscal periods, document numbering, source uniqueness and immutable reversal evidence.

## Decision

Persist explicit manual Draft entries through an Application service and existing repository/Serializable UnitOfWork.
Posting and reversal pass through `JournalPostingService`; no controller accesses EF and Angular calculates presentation
totals only for immediate feedback. Server/domain validation remains authoritative.

Use separate View/Create/Post/Reverse permissions. Use stable GUID request keys as language-neutral source references and
reject reuse with different content. A posted entry cannot be edited or deleted; correction creates a separately posted
opposite entry and preserves the original as Reversed.

## Consequences

- The ledger retains one posting authority and all fiscal-period controls.
- The existing schema supports the lifecycle; only permission catalog data is extended.
- Drafts are visible evidence but excluded from financial reports.
- Bulk/import/recurring/approval workflows and close-specific adjustments require separate decisions.
