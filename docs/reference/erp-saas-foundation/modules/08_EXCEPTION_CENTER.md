# Exception Center
ExceptionDefinition(Code, Severity, OwnerModule, DetectorVersion, SLA, RemediationLink); ExceptionInstance(TenantId, SourceRef, Code, Fingerprint, State, Assignee, DetectedUtc, ResolvedUtc). Generate from reliable events or scheduled bounded queries, deduplicate by stable fingerprint. Examples: stock-ledger mismatch, unbalanced draft journal, failed outbox dispatch, overdue receivable, incomplete import, expired approval.

States New -> Acknowledged -> InProgress -> Resolved / Dismissed (with reason); reopen on recurrence. Exception records are operational signals, not source of truth. Access controlled by tenant, role and resource scope. Suppression and rate limits avoid notification storms. Audit resolution.

Tests: deduplication, race/reopen, permission, no cross-tenant aggregation, high-volume detection, remediation link correctness.
