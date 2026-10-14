# ADR: Tenant-scoped automated replenishment execution

Date: 2026-10-14
Status: Accepted

## Decision

The web host runs one lightweight periodic worker. It discovers only tenant IDs whose saved replenishment schedule is due, then opens an explicit asynchronous tenant scope and a fresh dependency-injection scope for each company. Application services and repositories continue using the normal tenant query filters and UnitOfWork transaction.

Each scheduled time is unique per tenant. Every run stores its calculation inputs and outcomes, and active linked Purchase Requests or RFQs suppress later duplicate demand for the same rule.

## Consequences

- Controllers never bypass the Application layer and the worker reuses the same domain/application boundaries.
- Company data cannot be combined in one planning scope.
- Multiple hosts can race safely at the database uniqueness boundary; one transaction wins.
- Failed host/database connectivity is logged and retried by the next scan; operational deployment should run only the intended number of scheduler hosts or adopt a distributed job lease when scale requires it.
