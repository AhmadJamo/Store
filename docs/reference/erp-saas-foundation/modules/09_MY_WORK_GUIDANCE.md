# My Work & Workflow Assistant
WorkItem(TenantId, SourceModule, SourceRef, TaskType, State, AssigneeUser/Role, DueUtc, Version); GuidanceTopic(PageKey, Locale, Version, Content); GuidedTour(TourKey, Steps, UiVersion); UserGuidanceProgress(TenantId, UserId, TourKey, CompletedVersion).

My Work aggregates actionable items from existing workflows, not duplicate authority. Guidance reads current document state and authorized available actions via module application contracts. Show what step is next, why, prerequisites, and blockers; beginner/professional mode; Arabic/English RTL; accessibility and contextual error help. Never recommend actions a user cannot perform; never execute accounting changes via assistant without explicit authorized workflow.

Tests: status/role matrix, stale action invalidation, no cross-tenant task, localization, accessibility, page tour resilience, performance under 10k users, cache isolation.
