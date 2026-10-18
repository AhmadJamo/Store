# ADR — Local vendor-neutral observability gate

> Date: 2026-10-18  
> Status: Accepted for local Demo

## Context

The repository had repeatable focused and SQL executables but no single operational gate, health endpoints, request
measurements or measured latency baseline. The owner explicitly does not use Git as the execution gate and did not
select a hosted observability vendor.

## Decision

Use one local PowerShell runner, built-in ASP.NET Core health checks and vendor-neutral .NET `Meter` instruments.
Keep the health response secret-free. Treat structured error logs as the local alert destination and the Platform
Owner as response owner. Use a dependency-free console workload for reproducible p95/p99 evidence.

## Consequences

- The complete local gate runs with one command and has no third-party telemetry dependency.
- Any future OpenTelemetry or commercial exporter can subscribe to the stable meter without changing business code.
- The baseline proves local availability latency only; it does not prove ERP transaction capacity or production sizing.
- Production still requires a durable collector, alert channel, on-call owner and production-like load/soak evidence.
