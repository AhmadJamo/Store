# ADR: Configurable sales controls with frozen decision evidence

- Status: Accepted
- Date: 2026-10-22

## Context

Sales Order confirmation needs company-specific credit, overdue, discount, margin and amount policies. A fixed Customer credit-limit field would not cover different warning/block/approval thresholds, effective dates or customer exceptions. Re-evaluating historical decisions against current rules would also make audit evidence unstable.

## Decision

Store tenant-owned sales rules that may target every customer or one customer. Rules select a supported metric, comparison, threshold and Warn/RequireApproval/Block action, with priority and effective dates. Approval rules own ordered role steps and explicit requester/distinct-approver safeguards.

At confirmation, the Application service computes the supported metrics and persists a review with frozen triggered findings. Triggered approval steps are copied into the review; later rule activation or new rules cannot rewrite it. A warning may confirm with evidence. Block and Required Approval leave the order Draft. Only the final valid decision confirms it.

This is a sales-specific implementation, not a generalized approval engine. Purchase approval remains independent until repeated, stable cross-context semantics justify extraction.

## Consequences

- Administrators can layer and deactivate rules without destructive historical edits.
- Customer-specific credit limits and credit holds are represented by scoped rules.
- Historical reviews remain explainable after policy changes.
- Separation of duties can be stricter per rule and the strictest triggered policy wins.
- Margin uses the current product master cost reference at evaluation; prepared-product dynamic recipe costing remains a future enhancement if required.
- Evaluation creates no inventory or accounting effect, preserving downstream authority boundaries.
