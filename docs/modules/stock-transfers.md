# Stock transfers
> Status: IMPLEMENTED WITH BLOCKER | Last reviewed: 2026-09-13

Workflow: Draft → Submitted → Approved → Posted; submitted transfers can be rejected then returned to draft; posted transfers can be cancelled, which makes compensating stock movements. Creator cannot approve own transfer. Post creates out/in stock transactions and adjusts both warehouse balances in UnitOfWork. A StockTransfer rowversion prevents two concurrent workflow transitions, including duplicate posting, from both committing.

Creation requires `DocumentNumberSettings`; no creation/seeding was found, so absence blocks the feature. Sources: StockTransfer/Item/History/status/action entities; transfer commands and DTOs; interface/service/repository/configurations; `StockTransfersController`; `Views/StockTransfers/{Index,Create,Edit,Details}.cshtml`. Permissions include View/Create/Edit/Submit/Approve/Reject/Post/Cancel.
