# ADR-003: EF repositories and UnitOfWork
> Status: OBSERVED / PARTIALLY APPLIED  
> Last reviewed: 2026-09-13

## Decision observed
Repository interfaces sit in Domain; EF implementations use a shared AppDbContext. UnitOfWork wraps multi-record document operations in a transaction and one SaveChanges.

## Consequences
Purchase/sale/transfer operations use UnitOfWork. Master-data and settings services often call repository SaveChanges directly, so transaction ownership is inconsistent. Original rationale/alternatives: Unknown / Not determined from code.
