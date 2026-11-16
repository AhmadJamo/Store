# POS cash session foundation

ERP-007 adds five tenant-owned domain types without exposing runtime commands.

- `PosCashPolicy`: company currency, blind-close rule and variance thresholds.
- `PosCashDrawer`: named drawer scoped to a branch and POS terminal.
- `PosCashSession`: opening evidence, derived expected/declared cash, variance and independent approval state.
- `PosCashMovement`: immutable positive opening, sale, refund, paid-in or paid-out evidence with signed expected-cash effect.
- `PosCashCount`: immutable blind declared-cash evidence.

The database permits only one non-final session per tenant and terminal. Relationships use tenant-composite foreign keys. The aggregate prevents movements after count submission, repeated opening float, non-positive movements, self-approval and unreasoned approval. No API or Application service invokes these transitions in ERP-007; operational opening, POS attachment, closing and accounting are ERP-008.

## ERP-008 operational lifecycle

Policy can identify the cash payment method. Opening and manual movements use positive amounts and reasoned named counterpart accounts. Posted sales/returns carry source-unique evidence. Blind count freezes expected cash; exact close finalizes directly and any nonzero variance awaits a distinct authorized approver. Final sessions reject later movement.
