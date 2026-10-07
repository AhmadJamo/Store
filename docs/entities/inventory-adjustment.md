# InventoryAdjustment

Tenant-owned cycle-count and adjustment document.

- Scope: one warehouse and either one exact countable location or Unassigned.
- Lines freeze expected quantity, then store non-negative counted quantity and derived variance.
- Draft lines may be counted individually by absolute scanner input; replaying the same product quantity leaves the same state. Completing the count still requires every line.
- Workflow: Draft → Counted → Approved → Posted, with cancellation before posting.
- Blind mode hides expected quantities while Draft.
- Blind mode also hides current balances on the scanning console.
- Posting creates compensating physical and valued inventory movements.
- Rowversion protects concurrent workflow actions.
