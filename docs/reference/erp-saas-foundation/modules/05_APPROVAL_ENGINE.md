# Approval Engine
ApprovalPolicy(TenantId, PolicyVersion, DocumentType, Conditions, Steps, EffectiveFrom); ApprovalRequest(DocumentRef, PolicySnapshot, State, Initiator); ApprovalDecision(Actor, Action, Timestamp, Reason). Conditions are structured, typed and validated (amount/currency, branch, cost center, risk), not arbitrary user-authored code.

Policy evaluation is deterministic and versioned; policy snapshot prevents in-flight changes from silently altering approvals. Multi-stage sequential/parallel approval, delegation with expiration, escalation, rejection, cancellation, self-approval prevention, segregation of duties. Approval authorizes a workflow transition but must not bypass domain rules or tenant permissions. Concurrency: optimistic version/unique transition guard.

Tests: thresholds, currency conversion rules, role changes, duplicate approvals, stale decisions, delegation expiry, no self-approval, tenant isolation, policy version changes, escalation races.
