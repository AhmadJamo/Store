# InventoryAdjustment

Tenant-owned cycle-count and adjustment document.

- Scope: one warehouse and either one exact countable location or Unassigned.
- Lines freeze expected quantity, then store non-negative counted quantity and derived variance.
- Workflow: Draft → Counted → Approved → Posted, with cancellation before posting.
- Blind mode hides expected quantities while Draft.
- Posting creates compensating physical and valued inventory movements.
- Rowversion protects concurrent workflow actions.
