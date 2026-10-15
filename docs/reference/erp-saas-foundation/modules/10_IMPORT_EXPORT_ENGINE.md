# Import / Export Engine
Pipeline: upload to quarantine -> malware/type/size validation -> parse streaming -> schema mapping -> preview/dry-run -> validation + duplicate detection -> approval (if needed) -> chunked idempotent apply -> reconciliation -> result report. Each ImportJob has TenantId, MappingVersion, Checkpoint, IdempotencyKey, RowErrorFile, Status. Never run arbitrary formulas/scripts from imported files; protect CSV exports from formula injection.

Do not directly insert posted accounting or inventory balances bypassing domain services. Opening balances require explicit cutover policy and reconciled journal/movement operations. Exports enforce row-level permissions and data minimization; encrypted short-lived download URLs; background processing, quotas, retention and cancellation. Large jobs should not hold one transaction for entire file; define atomicity at business batch/document boundaries and recoverable checkpoints.

Tests: malformed file, malicious payload, duplicate rows, retry after crash, permission and tenant leak, mixed valid/invalid rows, currency rounding, cancellation, huge file memory, export injection.
