# Stock transfers
> Status: IMPLEMENTED WITH BLOCKER | Last reviewed: 2026-09-13

Workflow: Draft → Submitted → Approved → Posted; submitted transfers can be rejected then returned to draft; posted transfers can be cancelled, which makes compensating stock movements. Creator cannot approve own transfer. Post creates out/in stock transactions and adjusts both warehouse balances in UnitOfWork. A StockTransfer rowversion prevents two concurrent workflow transitions, including duplicate posting, from both committing.

Source and destination locations are optional per line. A specified location must belong to its matching warehouse. Posting moves exact location balances when supplied; omitted destination locations create unassigned destination stock, while omitted sources may consume only unassigned source stock. Creation requires `DocumentNumberSettings`; absence still blocks the feature. Sources: StockTransfer/Item/History/status/action entities; transfer commands and DTOs; interface/service/repository/configurations and transfer screens.

Warehouse policy can require an exact source or destination for structured warehouses. Simple warehouses reject exact locations and transfer only their warehouse-level balance. Destination capacity is checked during posting when capacity enforcement is enabled.
