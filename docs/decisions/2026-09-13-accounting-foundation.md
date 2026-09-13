# ADR: One chart of accounts with branch dimensions
> Date: 2026-09-13 | Status: Accepted

Use one hierarchical chart of accounts for the company. Accounts can have parent accounts; warehouse inventory accounts are leaf accounts below the inventory parent. Journal entries have an unlimited number of lines, and each line can be tagged with a Branch and Warehouse. Entries post only when debit equals credit. The selected stock valuation method is moving weighted average.

This permits company-wide statements and branch-level reporting without duplicating account trees. It also leaves tax, product and cost-centre dimensions to be added explicitly before automatic posting is enabled.
