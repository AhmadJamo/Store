# ADR: Reserve identity mutations for administrators
> Date: 2026-09-13 | Status: Accepted

Users.Create permits assigning any existing role, and Roles.Edit permits changing the caller's own permissions. Delegating these capabilities without a grant hierarchy permits privilege escalation.

Decision: reserve Users.Create/Edit/Delete and Roles.Create/Edit/Delete for current database Admin members. Enforce the shared Application policy in both backend authorization and UI/service permission evaluation. Keep read permissions delegable and retain existing permission definitions/mappings for compatibility. This deliberately removes mutation access from non-Admin role holders. A future delegation model requires explicit grant ceilings and independent authorization tests.
