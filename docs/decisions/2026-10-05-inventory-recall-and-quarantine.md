# ADR: Inventory recall preserves quarantine as a separate decision

Date: 2026-10-05  
Status: Accepted

## Decision

An inventory recall is a tenant-owned workflow for one concrete Product plus normalized lot/serial identifier. Starting a recall atomically records the reason and quarantines every currently available positive balance for that identity across warehouses and locations. Historic receipt, issue, transfer and return references remain immutable and form the recall impact list.

Closing the recall does not release stock. Release is a separate permission-protected quality decision with its own reason and trace event.

## Why

Recall completion means the investigation or outreach process is closed; it does not prove remaining stock is safe. Keeping quarantine independent prevents an administrative close action from silently returning affected inventory to sale or transfer allocation.

## Consequences

- Only one active recall may exist per tenant, product and identifier.
- Recall references are unique per tenant.
- Depleted identities can still be recalled when history exists; no inventory history is invented.
- Available balances are blocked immediately through the existing quarantine status.
- Customer/supplier contact workflows and external notification delivery remain future integrations.
