# ADR-20261019 — Configurable policy with safe history

## Decision

MiniStore uses configuration and policy choices instead of hard-coded business assumptions. A setting is classified as freely editable, effective-dated for future documents, or transition-controlled. Posted accounting, inventory movements and frozen document snapshots are never rewritten by a normal settings edit.

Every sensitive policy must provide safe defaults, bilingual explanation, authorization, audit evidence, impact validation and a guided next step. A policy with existing dependent transactions may change only through an explicit transition workflow that previews impact, obtains approval, posts required adjustments and reconciles the result.

For customer receivables, each tenant may choose Shared Control Account, Automatically Created Individual Accounts, or Manual Account Selection. The choice applies prospectively; existing customer accounts and historical sales are preserved. A later switch after the first configured policy and dependent sales requires a separate transition workflow.

## Consequences

- Flexibility is a platform principle, not permission to mutate history.
- Services resolve active policy and freeze needed values into future business documents.
- Configuration screens explain tradeoffs and block unsafe direct changes with an actionable path.
- Defaults remain usable for non-expert users; advanced users may opt into controlled alternatives.
