# Controlled bank reconciliation

UX-017B/C provide tenant-owned bank accounts, bounded immutable CSV evidence and explicit partial allocations to posted journal lines. Matching is server-authoritative: the bank GL account, debit/credit direction, remaining amounts and rowversions are checked inside the existing UnitOfWork. Match and reasoned Unmatch are independent antiforgery-protected permissions and registered semantic audit events. Neither operation creates, edits, posts or reverses a journal.

Angular uses one cohesive accounting feature page for import, evidence, server-authored candidates, remaining balances, allocations and permission-aware Match/Unmatch commands. Raw files are not retained and suggestions never auto-commit.
