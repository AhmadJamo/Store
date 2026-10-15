# Permissions and authorization
> Status: PARTIALLY IMPLEMENTED  
> Source of truth: Code  
> Last reviewed: 2026-10-16

`InventoryReservations.View` protects the read-only inventory reservation history screen.

Inventory adjustments split `View`, `Create`, `Count`, `Approve`, `Post` and `Cancel` so operational counting and supervisory approval can be assigned separately.

Inventory tracking uses `InventoryTracking.View` for traceability, `InventoryTracking.Open` for the high-impact opening allocation/policy activation and `InventoryTracking.ManageQuarantine` for reasoned quarantine/release transitions.
`InventoryTracking.ManageRecall` controls recall creation and closure. Recall visibility follows `InventoryTracking.View`; closing a recall never grants or implies quarantine release authority.
`Inventory.Replenishment.View` protects reviewed replenishment suggestions; `Inventory.Replenishment.Manage` separately protects rule create/update and activation changes. Creating a transfer draft additionally requires `StockTransfers.Create`; creating purchase demand additionally requires `PurchaseRequests.Create`.
`InventoryInsights.View` protects the read-only slow/dead inventory activity report; it grants no stock mutation capability.
`InventoryScanning.View` protects the scan console. Scanned putaway additionally requires `ProductStock.Edit`; both policies are enforced on the POST action and the form is hidden without edit authority.
Scanned internal relocation additionally requires `LocationMovements.Create`; its POST action enforces both permissions and antiforgery, and uses a relocation-specific retry key.
Scanned Draft line counting additionally requires `InventoryAdjustments.Count`; it records an absolute line value only and grants no approval or posting authority.

Authentication is ASP.NET Core Identity cookie authentication. `PermissionAuthorizeAttribute` creates policies with `Permission:` prefix. Identity owns credentials, while `TenantRole`, `TenantRolePermission` and `TenantUserRole` own authorization inside each company. Both evaluators require an active tenant; Admin receives full access only from that company's protected Admin assignment. Non-Admin permissions resolve only through the active company's role definition.

## Defined permission groups
Products, Warehouses, ProductStock, StockTransactions, StockMovements, LocationMovements, InventoryReconciliation, Suppliers, Purchases, PurchaseReturns, Sales, SalesReturns, StockTransfers, Users, Roles, and Settings are defined in `MiniStore.Application/Permissions/PermissionDefinitions.cs`. CRUD permissions exist for several modules even where matching actions do not exist (for example purchase/sale edit/delete). Purchase and sales returns define View/Create. Stock transfer also defines Submit/Approve/Reject/Post/Cancel. Location movements define View/Create. Inventory reconciliation and the physical StockMovement pilot define View only.

## Enforcement map
| Module | Backend enforcement |
|---|---|
| Products, Warehouses, Suppliers, Product Stocks | View/Create/Edit/Delete controller actions have matching `PermissionAuthorize`. |
| Legacy direct purchases | View remains enforced for historical records. Create remains defined for compatibility but runtime creation is retired and redirects to permission-protected Purchase Requests; Edit/Delete definitions have no controller actions. |
| Purchase returns | View/Create enforced; Create also requires a posted original purchase and sufficient current stock. |
| Goods receipt returns | Separate View/Create permissions; Create accepts posted new-flow receipts only and revalidates remaining receipt quantity, current stock, tracking and accounting configuration. |
| Sales | View/Create enforced; Edit/Delete definitions have no controller actions. |
| Sales returns | View/Create enforced; the original-sale return action is also hidden without Create. |
| Stock transactions | View/Create enforced. |
| Location movements | View/Create enforced for history and internal relocation; putaway continues to require ProductStock.Edit. |
| Inventory reconciliation | Dedicated View permission protects the read-only exception report; the controller exposes no mutation action. |
| Inventory insights | Dedicated View permission protects the read-only activity report; thresholds and filters do not mutate stock. |
| Inventory scanning | View protects resolution; putaway adds ProductStock.Edit, relocation adds LocationMovements.Create and Draft line counting adds InventoryAdjustments.Count. Every mutation uses antiforgery; movement commands use scoped keys and counts use absolute state assignment. |
| Stock transfers | View/Create/Edit/Submit/Approve/Reject/Post/Cancel enforced. |
| Users | View/Create enforced; Edit/Delete definitions have no actions. |
| Roles | View/Create/Edit/Delete enforced; controller uses `TenantRoleService`, system Admin cannot be edited/deleted and all IDs are resolved inside the active tenant. |
| Settings | Requires `Administration.Access`, which only an Admin assignment in the active tenant can satisfy. |
| Accounting master data | Accounts, branches, customers, tax rates and payment methods require the same tenant-Admin boundary; navigation visibility remains separately gated. |

## Known inconsistencies
`Settings.View` and `Settings.Edit` remain mainly navigation capabilities. `Administration.Access` is an internal protected policy key and is absent from the delegable permission catalogue. The Settings workflow catalog uses this same company-admin boundary; links from each documented step are rendered only when the current user also has that operational destination permission. Identity mutations (`Users.Create/Edit/Delete`, `Roles.Create/Edit/Delete`) require tenant Admin membership and are filtered from custom-role editing, regardless of stored data. Assignment validates an active company membership and a role owned by the same tenant.

`P1-AUD-012` does not add an audit-reader permission or screen. Existing `Roles.*` and `Administration.Access` policies still authorize the mutation; the Application service then records one tenant-scoped semantic event in the same transaction/save. Viewing/exporting audit events remains deferred to dedicated `AuditEvents.*` permissions in `P1-AUD-014`.

## Update rules
Permission change → `PermissionDefinitions`, seeder, controller attributes, navigation/views, `05_PERMISSIONS.md`, related module/controller/screen docs and tests.

`InventoryBalances.View` protects the read-only Inventory Availability projection. Balance mutation remains owned by inventory workflows; no direct edit permission exists.

# PUR-010 supplier purchasing permissions

- `Purchases.SupplierTerms.View` allows viewing supplier/product purchasing data.
- `Purchases.SupplierTerms.Manage` allows create, edit, prefer, activate and deactivate commands.

The Index and mutation actions enforce these policies server-side. Permission seeding adds both definitions and grants them to protected tenant Admin roles through the existing idempotent seeder.

# PUR-025 purchase approval permissions

- `PurchaseApprovalRules.View` and `PurchaseApprovalRules.Manage` separate rule visibility from configuration changes.
- `PurchaseRequests.Approve` and `PurchaseRequests.Reject` separately protect decision commands.
- A decision requires both the matching command permission and membership in the tenant role frozen on the current approval step. Having only one of these authorities is insufficient.

# PUR-030 purchase sourcing permissions

- `PurchaseSourcing.View` protects sourcing-event listing and detail visibility.
- `PurchaseSourcing.Create` protects converting an approved request into exactly one sent supplier-invitation event.
- `PurchaseSourcing.CreateDirect` separately protects the fast path. The Application service also requires the permission and re-evaluates the tenant amount limit, optional role and supplier-active policy before persisting a direct RFQ.
- The command remains commercial intent only; it has no inventory or accounting permission because it cannot mutate either area.

# PUR-040 supplier quotation permissions

- `SupplierQuotations.View` protects quotation comparison visibility.
- `SupplierQuotations.Create` protects capture and submission of a supplier's invited response.
- `SupplierQuotations.Award` protects the one-time, reason-required per-line quantity allocation across invited supplier quotations. It does not grant Purchase Order, receipt, stock or accounting authority.
- `PurchaseOrders.View` protects order list/detail visibility.
- `PurchaseOrders.Create` permits creating the complete supplier-grouped set of orders from the recorded line allocations only; it cannot change the award.
- `PurchaseOrders.Approve`, `PurchaseOrders.Confirm` and `PurchaseOrders.Cancel` separately protect the corresponding state transitions; confirmation still has no inventory or accounting effect.
- `PurchaseServiceAcceptances.View` protects service-completion history and detail; `PurchaseServiceAcceptances.Create` separately permits recording completion against confirmed service-policy lines. Neither permission grants stock mutation or Vendor Bill posting.
- `GoodsReceipts.Create` protects capture and direct one-step posting or submission to quality inspection. `GoodsReceipts.Inspect` records the quality decision, and `GoodsReceipts.PutAway` stores only accepted staged quantity and triggers stock/accounting posting. `GoodsReceipts.View` protects list/detail history. Each action revalidates document state in the Application service.
- PUR-150 barcode draft creation, scanning and final posting remain protected by `GoodsReceipts.Create`; scanning does not introduce a broader stock-edit permission or bypass the final receipt-posting checks.
- `GoodsReceiptReturns.Create` protects capture and immediate posting of a receipt-based supplier return; `GoodsReceiptReturns.View` protects its list/detail history. These permissions are distinct from legacy `PurchaseReturns.*`.
- `VendorBills.Create` protects Draft entry only; `VendorBills.View` protects history, detail and attachment download.
- `VendorBills.Approve` protects normal matching review/approval and `VendorBills.Post` separately protects accounting posting of an Approved bill.
- `VendorBills.Cancel` releases a Draft or Approved bill's reserved quantity and requires a reason; it cannot cancel Posted accounting.
- `VendorBills.OverrideMatch` protects approval when quantity or net price exceeds configured tolerances; a reason is mandatory and persisted. It does not grant posting.
- `SupplierPayments.View` protects payment history/detail; `SupplierPayments.Create` separately protects allocating and immediately posting a partial supplier settlement.

# PUR-120 purchase agreement and template permissions

- `PurchaseAgreements.View`, `Create`, `Activate`, `Release` and `Cancel` separate agreement visibility from every lifecycle command. A release creates only a Draft Purchase Order and does not grant order approval or confirmation.
- `PurchaseTemplates.View`, `Create`, `Generate` and `Manage` separate template visibility, source-order capture, Draft Purchase Request generation and active-state administration. Generated demand still requires the normal Purchase Request permissions and workflow.

# PUR-130 landed-cost permissions

- `LandedCosts.View` protects landed-cost history and immutable details.
- `LandedCosts.Create` protects allocation and immediate inventory/accounting posting from a posted Goods Receipt. It does not grant receipt, return or Accounting Settings administration.

# PUR-140 purchasing analytics permission

- `PurchaseAnalytics.View` protects the read-only purchasing dashboard, KPI definitions, supplier/cost trends, buyer workload, open exception drill-through and legacy/modern comparison. Source-document links are shown only when the user also holds the destination module's view permission. It grants no purchasing command, inventory mutation or accounting-posting authority.

# PUR-160 purchase accounting reconciliation permission

# PUR-240 automated replenishment permissions

- `PurchaseReplenishment.View` protects settings, run history, calculation evidence and generated-document links.
- `PurchaseReplenishment.Manage` changes the tenant schedule, forecast horizon and document-creation mode.
- `PurchaseReplenishment.Run` starts an immediate planning run; the background runner uses the saved tenant policy and does not grant an interactive user any additional permission.

- `PurchaseAccountingReconciliation.View` protects the read-only Inventory/GL and GRNI/GL comparison. It grants no stock adjustment, journal posting or accounting-setup authority; setup links are shown only when the user separately holds `Settings.Edit` or the matching warehouse permission.

# PUR-165 purchase-order completion permissions

- `PurchaseOrders.Close` permits closing an unreceived remainder only after all received quantities are billed or returned and requires a reason. It does not grant receipt, bill, approval or confirmation authority.
- `PurchaseOrders.Reopen` separately permits reopening only an order closed through the remaining-quantity action and requires a new reason. Automatically completed orders cannot use this command.
- `PurchaseOrders.ConfigureFulfillment` permits the one-time Draft conversion to direct delivery and destination capture. `PurchaseOrders.RecordDirectDelivery` separately permits immutable partial supplier-to-customer proof after confirmation; neither permission grants receipt, billing, approval or confirmation authority.

# PUR-170 supplier-credit-note permissions

- `SupplierCreditNotes.View` protects posted Supplier Credit Note history and immutable details.
- `SupplierCreditNotes.Create` permits immediate billed-return inventory and accounting posting from a posted Vendor Bill. It does not grant Vendor Bill creation, payment, warehouse setup or accounting-setup authority.

# PUR-180 controlled-reversal permissions

- `PurchaseDocumentReversals.View`: view reversal history and audit details.
- `PurchaseDocumentReversals.Create`: create and immediately post a reasoned controlled reversal.

# PUR-260 supplier communication and invoice-extraction permissions

- `SupplierCommunications.View` protects prepared message content, attachments and status evidence.
- `SupplierCommunications.Prepare` creates a bilingual RFQ/PO message snapshot but does not mark it sent.
- `SupplierCommunications.RecordActivity` records actual send, delivery, reply, failure and follow-up evidence.
- `PurchaseOrders.RecordSupplierConfirmation` records a supplier-confirmed delivery date and preserves the previous date.
- `VendorBills.ManageAttachments` adds bounded attachments to a Draft Vendor Bill.
- `VendorBills.ReviewExtraction` creates, applies or rejects review-first extraction suggestions; it grants neither approval nor posting.

# PUR-270 supplier portal and integration permissions

- `SupplierIntegrations.View` protects credentials metadata, outbox state, original inbound payloads and security audit evidence. It never reveals stored API or portal secrets because only hashes are retained.
- `SupplierIntegrations.Manage` creates/deactivates scoped API credentials and creates/revokes expiring portal links.
- `SupplierIntegrations.Queue` queues an approved/confirmed Purchase Order to a matching supplier callback through the reliable outbox.
- `SupplierIntegrations.Retry` retries an undelivered outbox message without changing its business identity or idempotency key.
- Portal/API access is not an internal role permission: each request must independently prove a valid supplier-specific token/key and required external scope.

