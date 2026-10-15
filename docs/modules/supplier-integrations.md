# Supplier portal and integrations

> Status: PUR-270 IMPLEMENTED
> Last reviewed: 2026-10-15

## Internal administration

Authorized users open **Supplier portal and integrations** from Purchasing. A credential belongs to exactly one active supplier, receives only selected scopes and may include an HTTPS callback plus an encrypted bearer secret. The API key is shown once; only its prefix and SHA-256 fingerprint remain stored. Deactivation is final for future requests.

RFQ and Purchase Order detail pages create 1–90 day private portal links. The token is shown once, stored only as a hash and can be revoked from the source document. Existing links show active, expired or revoked state without exposing the token again.
While an active document link exists, the creation action is replaced by its Active state. After a Purchase Order is queued to one callback credential, that destination is removed from the queue action and the completed delivery state is shown instead. A missing callback configuration error links directly to supplier integration settings.

## Supplier portal

- RFQ access shows only invited lines and accepts one complete quotation through the normal Supplier Quotation aggregate.
- Purchase Order access shows only that supplier's order, records reasoned delivery-date confirmation and accepts delivery notices as review evidence.
- Portal commands carry a server-rendered antiforgery token plus a unique idempotency key.
- A portal command never receives inventory, Vendor Bill approval or accounting authority.

## Supplier API

Base route: `/api/supplier/v1`. Send `X-Supplier-Api-Key` on every request and `Idempotency-Key` on every POST. Writes replay the original result for the same key/content and return conflict for changed content.

| Method and route | Required scope | Result |
|---|---|---|
| `GET /rfqs/{id}` | `rfq.read` | Supplier-specific RFQ snapshot. |
| `POST /quotations` | `quotation.write` | Submitted Supplier Quotation. |
| `GET /purchase-orders/{id}` | `po.read` | Supplier-specific Purchase Order snapshot. |
| `GET /purchase-orders/{id}/ubl` | `po.read` | UBL 2.1 Order XML. |
| `POST /purchase-orders/{id}/confirmations` | `confirmation.write` | JSON delivery-date confirmation. |
| `POST /purchase-orders/{id}/confirmations/ubl` | `confirmation.write` | UBL Order Response confirmation. |
| `POST /purchase-orders/{id}/delivery-notices` | `delivery.write` | Review-only delivery evidence. |
| `POST /purchase-orders/{id}/invoices/ubl` | `invoice.write` | Review-only UBL Invoice evidence. |

## Reliable outbox

Queuing a Purchase Order creates one tenant-unique message for the supplier credential and order. The worker atomically leases due messages, sends UBL XML with the stable idempotency key, and records delivery or a bounded retry schedule. After ten failed attempts the message remains Failed until an authorized manual retry. Callback validation is repeated at connection time to resist server-side request forgery and DNS rebinding.
