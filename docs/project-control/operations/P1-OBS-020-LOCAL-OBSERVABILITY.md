# P1-OBS-020 — Local verification and observability

> Status: COMPLETE FOR LOCAL DEMO — 2026-10-18  
> Scope: local Windows Demo only; no deployment or Git-hosted pipeline

## Decisions

- The owner does not use Git as an execution gate. The repeatable gate is therefore a local PowerShell command.
- SQL Server uses Windows authentication and a disposable process-specific database. No credential is committed.
- Health and metrics use the ASP.NET Core and `System.Diagnostics.Metrics` platform APIs without a paid vendor.
- Server-error and database-outage alerts are structured application logs. The local Platform Owner is the response owner.
- The first workload is a deterministic availability baseline: 20 warmups, 500 requests, concurrency 20 against `/health/live`.
- Initial limits are p95 <= 250 ms and p99 <= 500 ms. They are local guardrails, not production capacity claims.

## Delivered controls

- `/health/live` proves that the process can serve requests.
- `/health/ready` checks SQL connectivity and returns only `Healthy`, `Degraded` or `Unhealthy`; it never returns a connection string or exception.
- `MiniStore.Web` metrics cover request count, failures, duration, active requests, database probes and database-probe failures.
- HTTP 500 responses and slow requests emit structured logs; database readiness failures emit an error log.
- `tests/PerformanceSmoke` calculates p50/p95/p99, throughput and failures and writes a JSON artifact.
- `scripts/verify-local.ps1` builds, runs focused regression and disposable SQL integration, starts the application, verifies readiness, runs the workload and stops the child process.

## Repeatable command

From the repository root in PowerShell:

```powershell
pwsh -NoProfile -File scripts/verify-local.ps1
```

Optional switches include `-SkipSql`, `-SkipWorkload`, `-Requests`, `-Concurrency`,
`-P95LimitMilliseconds`, `-P99LimitMilliseconds`, `-BaseUrl` and `-SqlConnection`.
The full evidence is written below `artifacts/verification/` and is ignored as runtime output.

## Evidence

The 2026-10-18 full run produced:

- Release build: zero warnings and zero errors.
- Focused regression: 777 passed checks, including one emitted failure metric and one HTTP 500 alert log.
- SQL integration: passed the complete disposable migration, audit, tenancy, purchasing, accounting and inventory suite.
- Workload: 500/500 completed, zero failures, p95 16.85 ms, p99 28.35 ms, 3,687.45 requests/second.

## Failure response

1. If `/health/live` fails, inspect `artifacts/verification/web.log`, process startup and port ownership.
2. If `/health/ready` fails, inspect SQL service availability and protected connection configuration; do not print secrets.
3. If HTTP 500 or slow-request logs appear, use method, path and timestamp to reproduce, then inspect the server exception log.
4. If p95/p99 fails, repeat once on an idle machine, retain `performance.json`, then profile before increasing a limit.
5. SQL integration uses a disposable database and cleans it in its existing `finally` path.

## Rollback

Set `Observability:Enabled=false` to stop request instrumentation after restart. Health endpoints remain intentionally
available for diagnosis. Code rollback removes the Observability registrations/middleware and performance harness;
there is no schema migration or data rollback.

## Remaining production work

Choose the production host, exporter/collector, durable alert destination and on-call person before deployment.
Run business-transaction and soak workloads on production-like resources; this local health workload is not a sizing claim.
