# Platform architecture
## Logical planes
Control plane: tenant registry, plan catalog, subscription state, provisioning, stamp routing, metering, platform administration. Data plane: modular business API, SQL tenant records, workers, cache, object storage, reporting.

## Deployment
WAF/CDN -> stateless ASP.NET Core API replicas -> modular application -> SQL Server. Workers consume durable jobs/outbox. Cache is optional acceleration, never system of record. Observability across all components. Start with one stamp but keep tenant->stamp mapping explicit; introduce more stamps only when justified by load/isolation.

## Module boundaries
Tenancy, Identity, Audit, Finance, Inventory, Approvals, Configuration, Billing, Exceptions, Work, Guidance, Imports. Domain owns rules; Application owns use cases; Infrastructure owns persistence/integrations; API/UI presents. Cross-module reads through contracts or projections; cross-module writes via application orchestration and transactional events.

## Scale target
10k concurrent authenticated sessions != 10k requests/second. Define active-user mix, think time, write/read ratio, tenant skew, dataset sizes, latency SLOs; prove capacity using load tests. SQL database is likely shared contention point. Use index and query plan evidence before introducing replicas/partitioning.

## ADR triggers
Separate database per tenant, RLS, message broker, cache, reporting store, stamps, cloud provider, payment provider, queue technology all require documented tradeoff decision.
