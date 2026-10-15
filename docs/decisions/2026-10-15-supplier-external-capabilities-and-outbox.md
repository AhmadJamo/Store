# Supplier external capabilities and reliable outbox

> Status: Accepted
> Date: 2026-10-15

## Context

PUR-270 must let suppliers exchange purchasing documents without granting them internal user roles, inventory authority or accounting authority. External retries must not duplicate quotations, confirmations, invoices or outbound orders.

## Decision

Portal access is a short-lived capability bound to tenant, supplier and one document. API access is a supplier-bound credential with exact scopes. Raw portal/API secrets are shown once and only SHA-256 fingerprints are stored. Every external write requires an idempotency key and stores a request fingerprint plus result; the same key/content replays the result, while changed content is rejected.

UBL 2.1 Order output and Order Response/Invoice input use bounded XML parsing with DTD and external resolution disabled. Inbound invoices and delivery notices remain review evidence. They do not create stock, approve bills or post journals.

Outbound callbacks use a transactional outbox. A worker atomically leases a message, sends its stable idempotency key, records the attempt and retries with bounded backoff. HTTPS-only callback connections disable redirects/proxies, pin a DNS address verified as public and reject private/local targets.

## Consequences

- External suppliers need no internal Identity account or tenant role.
- Retry safety is enforceable at the database and protocol boundaries.
- External access remains narrower than internal purchasing permissions.
- A receiving system must honor the idempotency header to guarantee exactly-once business effect across network uncertainty.
