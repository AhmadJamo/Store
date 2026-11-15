# Controlled partial bank reconciliation

Use allocation rows rather than a matched flag. One statement line and one journal line may each participate in multiple allocations, while active-allocation sums enforce both remaining balances. Only posted, same-account, same-direction journal lines are eligible. Match/Unmatch preserve imported and ledger evidence, use rowversion and semantic audit, and never mutate journals.
