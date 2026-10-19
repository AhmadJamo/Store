# P2-CUS-040D / CUS-030A — Configurable receivable policy evidence
> Completed: 2026-10-19 | Environment: owner-declared local Demo

- Adopted the configurable-policy/safe-history ADR for the whole ERP.
- Added shared control, automatic individual account and manual account-selection modes with effective date, control/parent account and automatic code prefix.
- New customers follow the selected policy; existing customer/account links and posted history remain unchanged. A configured mode with dependent sales cannot be switched through an ordinary save.
- Removed tenant-local AccountId uniqueness only to support shared control; CustomerCode remains tenant-unique and tenant-safe account relationships remain restrictive.
- Migration `20261019164743_AddConfigurableCustomerReceivablePolicy` applied as 117/117. Demo retained one customer and its account; both existing settings rows remained unselected so the system did not choose for the user.
- Release builds passed with zero warnings/errors, 802 focused checks passed, disposable SQL passed shared-account reuse plus the existing suite, and EF reported no pending model change.
