# Audit Trail
AuditEvent(EventId, TenantId, ActorId, ActorType, Action, ResourceType, ResourceId, TimestampUtc, CorrelationId, Outcome, ReasonCode, BeforeRedacted, AfterRedacted, SourceIpMasked, SchemaVersion). Append-only storage with tamper-evidence where needed. Separate security audit from accounting journal and domain event stream.

Record create/update/delete, approvals, posting/reversals, permissions, tenant settings, privileged access, import/export and billing changes. Audit write must not silently disappear; critical events transactionally persisted or via outbox. Redact secrets and sensitive PII, set retention policy and restrict audit readers. Include export/access logs and immutable correlation IDs.

Tests: one event per committed action, no event on rolled-back business transaction unless security failure event, concurrency ordering, redaction, tenant isolation, audit query pagination, retention, and tamper detection.
