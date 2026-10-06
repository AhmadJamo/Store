# MiniStore WMS evolution plan
> Status: APPROVED ARCHITECTURAL PLAN — IMPLEMENTATION NOT STARTED  
> Source of truth: current code and migrations  
> Last reviewed: 2026-09-30

## 1. Executive decision

MiniStore will evolve into a location-aware WMS incrementally. It will not replace the existing inventory subsystem in one migration. The current business documents, transfer approval workflow, moving-average valuation, warehouse policies, tenant isolation, permissions and audit history remain valuable and are preserved.

The target separates four responsibilities:

```text
Business document and approval
        ↓ authorizes
Physical StockMovement ledger
        ↓ updates atomically
InventoryBalance projection
        ↓ reconciles with
Warehouse ProductStock valuation aggregate
```

- Business documents express commercial or operational intent: Purchase, Sale, StockTransfer, Return, InventoryAdjustment and future manufacturing documents.
- `StockMovement` will become the immutable physical movement ledger, with source/destination and document linkage.
- `InventoryBalance` will be a transactionally maintained current-position projection. It is not an independent ledger.
- `ProductStock` remains the authoritative Product + Warehouse quantity/AVCO aggregate during the transition and continues to own valuation.
- `StockTransaction` remains the existing warehouse-level valuation and quantity snapshot ledger until every producer has migrated. It is not deleted or silently reinterpreted.

### Current database data classification

All records currently stored in the development database are classified as **demo/test data**, not production business records. This classification covers current master data, users created for testing, stock quantities, locations, products and transactional history. It does **not** mean that the database schema, migrations, domain rules or tenant/security boundaries are disposable.

During WMS development, demo rows may be reseeded, corrected or recreated when a clean baseline is required, provided the action is deliberate and preceded by the relevant reconciliation or backup check. No plan item should assume that the current demo quantities are a production migration constraint. Nevertheless, every new migration and workflow must remain production-safe because the same architecture will later carry real customer data. Destructive demo-data reset is an explicit execution task, not an automatic side effect of applying a migration.

### Execution authorization and autonomy

The project owner has authorized continuous autonomous execution of the approved WMS roadmap. Routine in-scope code edits, documentation updates, builds, tests, migrations against the named development environment and creation/removal of dedicated temporary test databases should proceed without pausing for a separate confirmation each time. This standing authorization does not expand the roadmap beyond inventory/warehouse work and cannot bypass an approval prompt imposed by the execution platform. A new decision is still required for an unclear target, production data, credentials, external publication, or destructive action outside the explicitly named development/test scope.

## 2. Repository inspection summary

### Implemented strengths to preserve

- DDD-style Domain/Application/Infrastructure/Web dependency direction and manual DTO mapping.
- Shared-database tenant isolation, tenant-aware keys/relationships and query/write guards.
- `ProductStock` warehouse balance with rowversion, moving weighted-average cost, inventory value and provisional-negative variance settlement.
- `StockTransaction` history with before/after quantity, average, value, unit cost, transaction value and variance snapshots.
- Structured `StorageLocation` records with warehouse code uniqueness, zone/aisle/rack/level/bin attributes, operational type, status and quantity capacity.
- `ProductLocationStock` location balances with rowversion and unique Product + Location rows.
- Hybrid, Simple and LocationManaged warehouse policies; transfer-location requirements, POS eligibility, picking policy and capacity enforcement.
- Unassigned Stock and audited Putaway into exact locations.
- Same-warehouse relocation with immutable `LocationMovement` history and no change to warehouse totals.
- StockTransfer Draft → Submitted → Approved → Posted, Reject → ReturnToDraft and compensating Cancel workflow with actor/time/reason history and self-approval prevention.
- Transfer source/destination locations, unassigned source/destination behavior and source-cost carry into destination AVCO.
- Atomic Serializable inventory transactions plus ProductStock/ProductLocationStock/StockTransfer concurrency tokens.
- Recipe consumption, explicit kitchen variance and narrowly controlled negative ingredient stock.
- Posted sales and purchase returns with traceable stock transactions.
- Branch/POS warehouse access configuration, centralized numbering, permissions, localization and audit interceptor.

### Current limitations discovered

- `StorageLocation` is a flat warehouse-owned record; zone/aisle/rack/level/bin are labels, not a parent/child tree.
- Location type does not yet express configurable capabilities such as reservable, pickable, receivable or shippable, and locations have no barcode/sequence.
- `StockTransaction` is warehouse-level and contains no source/destination location, initiating/posting user, stable reference type/id, UOM snapshot or lifecycle status.
- `LocationMovement` covers Putaway/Relocation only, while sales, purchases, returns, recipe consumption and most adjustments are recorded separately.
- `ProductStock` and `ProductLocationStock` form a dual-level balance model, but Unassigned Stock is calculated as a difference rather than represented by an explicit location.
- Sales and ordinary warehouse issues do not consistently consume an exact location, so allocated location totals may temporarily exceed the warehouse total after warehouse-level issues.
- Several inventory queries load full tables before grouping for Unassigned Stock and capacity checks; this will not scale.
- No reservation model, incoming/outgoing projection or forecast semantics exist.
- No lots, serials, expiration, packages, ownership/consignment or recall traceability exist.
- Manual adjustments are immediate reasoned movements, not a count/approval/posting workflow.
- No cycle count, replenishment, putaway rule, removal allocation or advanced picking aggregate exists.
- Product barcode exists, but locations, lots, serials, packages and warehouse documents have no unified scan workflow.
- Current capacity is quantity-only and shared across products; weight, volume, packages and storage compatibility are absent.
- `InventoryPickingStrategy` is a warehouse policy but does not yet allocate stock automatically.
- Stock valuation supports AVCO only. Physical removal and accounting costing are correctly separate in concept, but no formal valuation-entry abstraction exists.
- Critical concurrency coverage is mainly model/domain regression; database-backed parallel sale/reservation/movement tests are still missing.

## 3. Capability gap analysis

| Capability | Existing | Partial | Missing | Keep | Modify | New | Priority |
|---|---:|---:|---:|---:|---:|---:|---|
| Tenant-safe warehouse master | ✓ | | | ✓ | | | Foundation |
| Product category/logistics profile | product type/unit only | ✓ | weight/dimensions/storage traits | ✓ | ✓ | category/profile | P1 |
| Flexible business-specific attributes | | | ✓ | Product remains SKU | | ✓ | P1 |
| Product variants | each Product is one SKU | ✓ | template/variant grouping | ✓ | extend gradually | template/attributes | P1 |
| Warehouse operating policies | ✓ | | | ✓ | extend later | | Foundation |
| Flat exact locations | ✓ | | | ✓ | ✓ | | P1 |
| Hierarchical location tree | | ✓ labels only | ✓ | | ✓ | | P1 |
| Location barcode/capabilities/sequence | | | ✓ | | ✓ | | P1 |
| Unassigned stock work queue | ✓ | | | ✓ | migrate to explicit location | | P2 |
| Putaway and internal relocation | ✓ | | | ✓ | route through movement engine | | P2 |
| Warehouse ProductStock/AVCO | ✓ | | | ✓ | reconcile/projection contract | | Foundation |
| Location balance | ✓ | | | preserve data | evolve | future dimensions | P3 |
| Immutable warehouse valuation movements | ✓ | | | ✓ | add stable links | | Foundation |
| Unified source→destination movement ledger | | partial across two ledgers | ✓ | existing history | | ✓ | P2 |
| Transfer approval/history/cancellation | ✓ | | | ✓ | emit movements | | P2 |
| Transit inventory | | | ✓ | transfer workflow | ✓ | transit locations | P2 |
| Receipt/delivery steps | | | ✓ | Purchase/Sale docs | extend | StockMovement routes | P3 |
| Reservations and available quantity | | | ✓ | | | ✓ | P4 |
| Incoming/outgoing/forecast quantity | | | ✓ | | | ✓ | P4/P7 |
| Inventory adjustment workflow | immediate adjustment only | ✓ | approval/count docs | reason/history | refactor gradually | new document | P4 |
| Cycle counting | | | ✓ | | | ✓ | P5 |
| Lots/batches | | | ✓ | | | ✓ | P5 |
| Serial numbers | | | ✓ | | | ✓ | P5 |
| Expiration/FEFO | warehouse enum only | ✓ | operational engine | keep policy | extend | lot dates/alerts | P5 |
| FIFO/LIFO/closest allocation | policy placeholders | ✓ | allocator | keep costing separate | extend | allocation service | P6 |
| Putaway rules | manual putaway | ✓ | rules/suggestions | ✓ | extend | rule aggregate | P6 |
| Capacity weight/volume/package | quantity only | ✓ | ✓ | quantity capacity | extend | storage categories | P6 |
| Quantity-splitting putaway | | | ✓ | | | optimizer later | P8 |
| Replenishment/min-max/safety stock | branch permission only | ✓ | domain/workflow | access policy | extend | rules/suggestions | P7 |
| Packages/pallets | | | ✓ | UOM separation | | ✓ | P8 |
| Ownership/consignment | | | ✓ | | | ✓ | P8 |
| Barcode execution | product search only | ✓ | scan workflow | product barcode | extend | scan commands | P7 |
| Advanced picking/waves/clusters | | | ✓ | | | ✓ | P9 |
| Landed costs | | | ✓ | AVCO engine | extend | allocation docs | P9 |
| Inventory-at-date/trace reports | movement history only | ✓ | dimensional reports | ✓ | extend | projections/reports | P3+ |
| Concurrency/idempotency | Serializable + rowversion | ✓ | reservations/movement keys/tests | ✓ | strengthen | idempotency records | Every phase |
| Bilingual UI/permissions/audit | ✓ foundation | ✓ WMS gaps | | ✓ | extend per slice | new permission keys | Every phase |

## 4. Authoritative data model

### During transition

| Concern | Authority | Projection/supporting data |
|---|---|---|
| Warehouse quantity and AVCO | `ProductStock` | `StockTransaction` snapshots |
| Exact physical allocation | `ProductLocationStock` plus derived Unassigned difference | `LocationMovement` history |
| Business approval | owning business aggregate, especially `StockTransfer` | history records |
| Accounting value | posted journals and ProductStock valuation snapshots | reports |

### Target

| Concern | Authority | Projection/supporting data |
|---|---|---|
| Business intent/approval | Purchase, Sale, StockTransfer, Return, InventoryAdjustment, etc. | document histories |
| Physical history | posted immutable `StockMovement` | document-to-movement links |
| Current dimensional stock | `InventoryBalance`, updated in the same transaction as movement posting | rebuild/reconciliation from posted movement ledger |
| Warehouse AVCO/value | retained `ProductStock` | retained/enriched valuation records and journals |
| Availability | InventoryBalance on hand minus active reservations | query projections for incoming/outgoing/forecast |

No phase may permit two independently editable authorities for the same dimension. Projections must be transactionally updated and periodically reconciled.

## 5. Proposed changes

### 5.1 Location hierarchy and capabilities

Current: `StorageLocation` is warehouse-owned with Code, Zone, Aisle, Rack, Level, Bin, Type, Status and MaximumQuantity.

Problem: labels cannot express a configurable tree, scanning order or operation capabilities; hard-coded levels do not fit every business.

Proposed: extend `StorageLocation` in place with optional ParentLocationId, display Name, Barcode, Sequence and explicit reservable/pickable/receivable/shippable flags. Retain existing structured labels as optional searchable metadata during migration. Add cycle prevention and same-warehouse parent validation.

Why: this preserves IDs and foreign keys while allowing Warehouse → Zone → Aisle → Rack → Shelf or simpler layouts.

Existing Feature Impact: location search, putaway, relocation, transfers, capacity and warehouse screens.

Preservation Strategy: existing locations become root children and keep their codes/types/capacities. No row is deleted or renumbered.

Migration: nullable columns only; backfill Name from Code, Sequence from stable Id order, capability defaults from current Type.

Dependencies: none beyond current location model.

Risk: cycles, invalid cross-warehouse parents and ambiguous capabilities.

Decision: **Extend**.

### 5.2 Explicit operational/default locations

Current: Unassigned Stock is `ProductStock.Quantity - sum(ProductLocationStock.Quantity)`.

Problem: an implicit remainder cannot participate cleanly in source/destination movements, reservations, scanning or historical location reports.

Proposed: create protected default operational locations per structured warehouse (at minimum Unassigned/Stock; later Receiving, Shipping, Returns, Quarantine and Transit according to policy). Migrate the existing remainder into the default internal balance only after reconciliation.

Why: every physical quantity needs an address without losing the useful Unassigned work queue.

Existing Feature Impact: purchases, unassigned screen, transfers without exact destinations, sales and returns.

Preservation Strategy: the Unassigned screen remains, but reads the protected Unassigned location rather than a difference after cutover.

Migration: preflight report; reject cutover where assigned totals exceed ProductStock; insert default locations and balance rows for the verified difference.

Dependencies: hierarchical location foundation and reconciliation command.

Risk: current inconsistent allocated totals could make backfill unsafe.

Decision: **Replace Gradually** (derived remainder → explicit protected location).

### 5.3 Unified physical StockMovement ledger

Current: `StockTransaction` records warehouse quantity/value effects; `LocationMovement` records only Putaway/Relocation; business services write them directly.

Problem: no single trace answers source, destination, document, initiator, approver/poster and physical status for all inventory changes.

Proposed: add a posted/draft/cancelled `StockMovement` aggregate with product, stock-unit snapshot, source/destination location, quantity, stable ReferenceType + ReferenceId, movement kind, timestamps/users and idempotency key. Pilot it on Putaway/Relocation, then transfers, then receipts/deliveries/returns/adjustments. `StockTransaction` continues as the valuation snapshot created when the movement affects warehouse quantity/value.

Why: separates physical execution from valuation and business approval without discarding either existing ledger.

Existing Feature Impact: every inventory-changing application service.

Preservation Strategy: initially dual-write new movements and existing records inside one UnitOfWork; compare outputs before switching reads. Existing rows remain immutable history.

Migration: no fabricated source/destination for old StockTransactions. Mark legacy provenance explicitly; optionally link confidently matched references.

Dependencies: location capability model and stable source-reference conventions.

Risk: duplicate movement generation, dual-write drift and ambiguous legacy references.

Decision: **New Feature beside existing ledgers, then Refactor gradually**.

### 5.4 Business documents remain separate

Current: StockTransfer contains the strongest approval workflow; purchases, sales and returns own their business rules.

Problem: collapsing documents into movements would lose intent, approvals, pricing, tax and reversal semantics.

Proposed: documents authorize and generate movements through an idempotent application-layer posting orchestrator. Approved StockTransfer generates outbound/transit/inbound movements according to route; cancellation generates compensating movements and never deletes history.

Why: physical facts and business decisions change for different reasons and require different permissions.

Existing Feature Impact: StockTransfer posting/cancellation and future receipt/delivery workflows.

Preservation Strategy: retain every current state, actor, timestamp, reason and history record.

Migration: no state conversion; movement links are added only for newly posted documents, with optional safe legacy linkage later.

Dependencies: StockMovement and idempotency contract.

Risk: document status and movement status diverge if not committed atomically.

Decision: **Keep and Integrate**.

### 5.5 InventoryBalance projection

Current: ProductLocationStock represents Product + exact location quantity; ProductStock represents Product + Warehouse quantity/value.

Problem: future lot/serial/package/owner/reservation dimensions cannot fit the current key, while full-history aggregation is too expensive.

Proposed: introduce `InventoryBalance` only when the movement engine is stable. Start with Product + Location and nullable future dimensions; add OnHand and Reserved with Available computed as OnHand - Reserved. Unique dimensional indexes must use normalized sentinel keys or filtered strategies compatible with SQL Server null semantics.

Why: fast operational queries and safe reservations without making a projection an independent source of truth.

Existing Feature Impact: ProductLocationStock, Unassigned Stock, location search, stock reports and capacity checks.

Preservation Strategy: backfill one balance per current ProductLocationStock plus explicit Unassigned; dual-read reconciliation before cutover. ProductStock remains AVCO authority.

Migration: additive table, repeatable backfill, count/value reconciliation, feature-flagged reads, rollback by returning reads to existing tables.

Dependencies: StockMovement and explicit default locations.

Risk: projection drift and dimensional unique-index errors.

Decision: **New projection, Replace ProductLocationStock gradually; Keep ProductStock**.

### 5.6 Reservations and availability

Current: stock is checked only when a sale/transfer posts; no Reserved or Available quantity exists.

Problem: parallel orders can compete until posting, and future picking cannot secure stock.

Proposed: add immutable/releasable reservation allocations linked to business demand and balance dimensions. Reserve inside Serializable transactions with rowversion/atomic conditional update; default policy forbids over-reservation. Define OnHand, Reserved, Available, Incoming, Outgoing and Forecasted explicitly.

Why: reliable fulfillment and a foundation for picking/forecasting.

Existing Feature Impact: sales, transfers, POS, availability displays and cancellation.

Preservation Strategy: documents without reservations continue direct posting during a controlled compatibility window; each workflow migrates end-to-end.

Migration: no historic reservations. Activate per workflow only after balance reconciliation.

Dependencies: InventoryBalance and movement demand/status semantics.

Risk: leaked reservations, deadlocks and double fulfillment.

Decision: **New Feature**.

### 5.7 Inventory adjustment and cycle count documents

Current: reason-required AdjustmentIn/AdjustmentOut movements are immediate; there is no counted quantity, approval or blind count.

Problem: direct adjustment lacks count lifecycle, segregation of duties and discrepancy review.

Proposed: preserve emergency manual adjustment under a restricted permission, and add InventoryAdjustment Draft → Submitted → Approved → Posted/Cancelled with system/count/difference snapshots. Later add CycleCountPlan and assignments.

Why: financially sensitive discrepancies need authorization and traceability.

Existing Feature Impact: StockTransactionService and adjustment UI.

Preservation Strategy: existing adjustment history remains valid; new documents emit compensating StockMovements and valuation transactions.

Migration: additive documents; no conversion of legacy adjustments.

Dependencies: movement engine and balances; cycle scheduling follows adjustment workflow.

Risk: users bypassing the document via old permission.

Decision: **Extend with a new workflow; restrict legacy path gradually**.

### 5.8 Lots, serials and expiration

Current: products have no tracking policy or traceable stock identity.

Problem: FEFO, recalls, serial lifecycle and expiration controls are impossible.

Proposed: add product tracking policy (None/Lot/Serial), Lot/Serial master records, dimensional movement/balance allocations and expiration/best-before/removal/alert dates. Serial-tracked allocations enforce quantity one and tenant-wide product+serial uniqueness. FEFO is a removal policy, not a costing method.

Why: food, medicine and durable-goods traceability.

Existing Feature Impact: receipts, sales, transfers, returns, counts, recipes and reporting.

Preservation Strategy: existing products default to None and existing balances remain untracked; opt-in requires a controlled opening allocation of current quantity.

Migration: additive tables/nullable dimensions; no automatic invented lot numbers without explicit administrator confirmation.

Dependencies: StockMovement, InventoryBalance and reservation allocations.

Risk: enabling tracking on existing non-zero stock without allocation; serial duplication.

Decision: **New Feature**.

### 5.9 Removal allocation and putaway rules

Current: warehouse stores a picking-strategy enum; putaway is manual and capacity is quantity-only.

Problem: no allocator selects stock or suggests destinations.

Proposed: build strategy services over balances: FIFO, FEFO, closest sequence and configurable future policies. Add simple priority-based PutawayRule by product/category/warehouse/storage category, then weight/volume/package capacity and multi-location splitting later.

Why: automation without mixing removal choice with AVCO/FIFO accounting valuation.

Existing Feature Impact: picking, transfers, sales allocation and Unassigned Stock.

Preservation Strategy: manual selection remains a permitted override with audit reason; LocationPriority remains default compatibility behavior.

Migration: rules are opt-in; existing capacities retained as quantity constraints.

Dependencies: hierarchy, balances, reservations and lot dates for FEFO.

Risk: non-deterministic allocation and performance under contention.

Decision: **Extend**.

### 5.10 Replenishment and forecasting

Current: branch warehouse access has a replenishment permission flag, but no rule or suggestion engine.

Problem: stock shortages are reactive and future supply/demand is invisible.

Proposed: define ReplenishmentRule with min/max, reorder point/quantity, safety stock, lead time and route. Forecast is a query projection: OnHand + confirmed Incoming - confirmed Outgoing, with Reserved shown separately. Suggestions create draft purchase/transfer/manufacturing intent, never stock directly.

Why: controlled planning without treating forecast as physical quantity.

Existing Feature Impact: purchasing, transfers, future manufacturing and dashboards.

Preservation Strategy: no automatic orders initially; users approve suggestions.

Migration: additive rules only.

Dependencies: reservation and incoming/outgoing movement states.

Risk: unclear demand-status inclusion and duplicate suggestions.

Decision: **New Feature**.

### 5.11 Barcode, packages and advanced picking

Current: product barcode search exists; no scan-session, package or pick-batch model exists.

Problem: warehouse execution requires keyboard-heavy ERP forms and cannot track containers.

Proposed: after movement foundations stabilize, add scan commands for Location → Product → Lot/Serial → Quantity → Confirm, then package/pallet hierarchy and single/batch/wave/cluster picking documents.

Why: operational speed and accuracy.

Existing Feature Impact: product lookup, locations, transfers, receipts and delivery.

Preservation Strategy: existing forms remain available; barcode is another command interface over the same Application services.

Migration: additive barcode uniqueness checks and package tables; do not confuse packages with UOM.

Dependencies: movement, balances, reservations and tracking dimensions.

Risk: duplicate barcodes, partial scan sessions and offline retries.

Decision: **New Feature, deferred**.

### 5.12 Valuation and landed costs

Current: AVCO per Product + Warehouse, valuation snapshots and journal posting exist.

Problem: no Standard/FIFO costing, valuation-entry abstraction or landed-cost allocation.

Proposed: keep AVCO as the active method while physical WMS stabilizes. Later introduce immutable valuation records linked to StockMovement and allocate landed costs by value/quantity/weight/volume. Standard/FIFO costing requires a separate accounting ADR and migration plan.

Why: physical movement must remain independent from costing while traceable to it.

Existing Feature Impact: ProductStock, StockTransaction, purchase/return/transfer costing and journals.

Preservation Strategy: no recalculation of historic AVCO; new valuation methods are opt-in from a controlled effective date.

Migration: additive valuation records and opening layers; never rewrite posted journals silently.

Dependencies: stable StockMovement identity and accounting policy decisions.

Risk: financial misstatement and irreconcilable historic layers.

Decision: **Keep AVCO; New/Extend later**.

### 5.13 Product logistics, flexible attributes and variants

Current: `Product` is the stocked/sellable identity referenced by recipes, purchases, sales, balances and movements. It has classification, managed stock unit, prices, barcode/code and channel behavior, but no category, physical dimensions, tracking policy or configurable business attributes.

Problem: warehouses need weight, length/width/height, volume, storage handling and lot/serial policy. Different businesses also need different descriptive or variant attributes—such as clothing color/size, food allergens/storage temperature, electronics memory/voltage or automotive compatibility—without adding a nullable database column for every industry.

Proposed: preserve `Product` as the concrete stockable SKU/variant identity. Add a logistics foundation containing ProductCategory, net/gross weight where justified, package-independent base dimensions, calculated/overridden volume, storage/handling flags and tracking policy. Add metadata-driven attribute definitions with typed values (text, number, boolean, date, option and measured value), category applicability and validation. Later add an optional ProductTemplate that groups concrete Product rows into variants; variant-defining attributes create distinct Product SKUs, barcodes, prices and balances, while descriptive attributes do not.

Why: WMS capacity, putaway, replenishment, lots/serials and barcode workflows require structured product facts, while a flexible catalogue must support multiple industries without an ever-growing Product table.

Existing Feature Impact: Product create/edit/index, product search/import, recipes, purchase/sale selectors, POS assortment, measurement units, stock balances, capacity and future lot/serial rules.

Preservation Strategy: every existing Product remains a valid independent SKU with the same Id and relationships. ProductTemplate is optional; no existing ProductId is replaced. Existing products receive safe defaults: no tracking, no special handling and unknown physical dimensions. A template groups products but never owns inventory itself.

Migration: first add nullable logistics fields/category relationship and explicit tracking default. Add attribute tables separately. If templates are introduced, create or assign a default one-to-one template only when needed; do not change historic line or movement ProductIds.

Dependencies: managed measurement units already exist. Logistics metadata should precede advanced capacity/putaway and lot/serial work. Template/variant UI must precede any bulk variant generation.

Risk: variant explosion, duplicate barcodes/codes, confusing descriptive attributes with stock dimensions, changing tracking on non-zero stock, and unit mismatch for weight/dimensions.

Decision: **Extend Product and add metadata; preserve Product as stocked SKU; introduce ProductTemplate gradually**.

## 6. Safe data migration strategy

The current local database contains demo/test data only. Therefore, implementation may use a controlled clean reseed instead of preserving inconsistent demo history when that is the safer verification route. The default engineering path remains additive and reversible so it is suitable for future production databases.

1. Run a preflight reconciliation per tenant/Product/Warehouse:
   - ProductStock quantity.
   - Sum ProductLocationStock.
   - derived Unassigned.
   - latest StockTransaction QuantityAfter/InventoryValueAfter.
   - negative or over-allocated exceptions.
2. Do not cut over a warehouse with unexplained differences.
3. Add nullable/additive schema first; deploy code capable of reading old data.
4. Create protected default locations idempotently for each warehouse.
5. Backfill explicit Unassigned balances from verified differences.
6. Introduce StockMovement dual-write behind a per-tenant/per-workflow feature flag.
7. Compare old and new ledgers/projections continuously.
8. Switch reads only after totals match and critical workflows pass SQL integration tests.
9. Keep old tables and references through at least one stable release; deprecation requires a separate ADR and backup/rollback rehearsal.
10. Never invent lot, serial, owner or package history for legacy quantities. Require an opening allocation document where traceability begins.
11. If a demo-data reset is selected, record its scope, take any required development backup, recreate the supported seed/onboarding baseline and rerun reconciliation and SQL integration tests before continuing.

Rollback is application-level first: disable the new read path and continue using preserved ProductStock/ProductLocationStock/StockTransaction data. Schema removal is not part of an ordinary rollback.

## 7. Concurrency and integrity requirements

- Continue Serializable transactions for multi-row inventory workflows, but add narrow indexes to prevent excessive range locking.
- Every balance dimension carries rowversion or uses a guarded atomic update.
- Movement posting uses a unique tenant idempotency key derived from source type/id/line/action.
- Database constraints enforce positive posted quantities, valid source/destination difference, tenant-consistent relationships and serial uniqueness.
- Reservation create/release/consume occurs in the same transaction as the affected balance update.
- Document state transition and generated movements commit together.
- Cancellation creates compensating movements; it does not mutate posted movement facts.
- Add SQL-backed parallel tests for last-unit sale, competing reservation, transfer post/cancel and duplicate retry.

## 8. Permission plan

Permissions remain tenant role permissions; role names are never hard-coded. Add only with the corresponding vertical slice:

- `Inventory.Locations.View/Create/Edit`
- `Inventory.Movements.View/Post/Reverse`
- `Inventory.Reservations.View/Manage/Override`
- `Inventory.Adjustments.View/Create/Approve/Post`
- `Inventory.Counts.View/Assign/Post`
- `Inventory.Lots.View/Manage`
- `Inventory.Serials.View/Manage`
- `Inventory.Replenishment.View/Manage`
- `Inventory.Barcode.Execute`

Data scope by branch/warehouse/POS must be enforced in Application queries and mutations, not only navigation.

## 9. Reporting model

Build reports incrementally from authoritative ledgers and projections:

- Phase 0–2: reconciliation, on hand by warehouse/location, movement trace, unassigned/over-allocated exceptions.
- Phase 3–4: available/reserved/incoming/outgoing/forecast and inventory-at-date.
- Phase 5: lot/serial traceability, recall and expiration.
- Phase 6–7: putaway efficiency, replenishment, slow/dead stock, empty locations and utilization.
- Phase 8–9: package/pick productivity, valuation layers and landed-cost audit.

Inventory-at-date must derive from immutable posted movements/snapshots, never from today's balance alone.

## 10. Implementation roadmap and vertical slices

### WMS-000 — Reconciliation baseline and contracts (complete)

- [x] Document and implement read-only inventory reconciliation across warehouse totals, location allocations and latest movement snapshots.
- [x] Add SQL integration fixtures for two tenants and concurrent last-unit issue.
- [x] Define stable movement reference/idempotency conventions and feature-flag strategy.
- [x] No balance mutation or cutover.

Definition of Done: bilingual Admin report, permission, indexed repository queries, tests, docs and zero unexplained differences before the next phase.

### WMS-005 — Product logistics foundation (complete; development migration applied)

- [x] Product categories and structured logistics metadata required by warehouse rules.
- [x] Weight/dimensions/derived volume with explicit managed Mass/Length units and safe nullable defaults.
- [x] Storage/handling traits and None/Lot/Serial tracking policy, without enabling tracked stock allocation yet.
- [x] Prevent tracking-policy activation on non-zero stock until the WMS-070 opening-allocation workflow exists.
- [x] Apply `20260929220416_AddProductLogisticsFoundation` to the named development database.
- [ ] Complete an authenticated UI smoke test with the next WMS UI verification pass.

### WMS-010 — Hierarchical locations

Status: implementation and additive migration complete; migration applied to `AHMAD/MiniStoreDb`; authenticated UI smoke remains.

- [x] Parent tree, name, barcode, sequence and capabilities.
- [x] Parent-indented UI, warehouse-filtered parent selection and cycle/same-warehouse validation.
- [x] Backfill existing location metadata without ID changes.
- [x] Enforce receive/pick capabilities in putaway, relocation and transfer workflows.

### WMS-015 — Flexible attributes and product variants

- [x] Typed attribute definitions, options, category applicability and definition validation (WMS-015A).
- [x] Typed product value assignment and required/category/option validation (WMS-015B).
- Separate descriptive attributes from variant-defining attributes.
- [x] Optional ProductTemplate groups existing concrete Product SKUs with duplicate-safe canonical signatures; Product remains the inventory identity (WMS-015C).
- [x] Controlled preview and explicit creation of missing selection-based combinations, with a server-enforced 50-variant limit and safe source-SKU cloning (WMS-015D).
- Automatic generation never copies barcode, stock, recipes or history; prepared-to-order and non-selection variants remain manual.

### WMS-020 — Physical movement kernel

- [x] WMS-020A StockMovement aggregate, posted status and tenant-unique idempotency.
- [x] Pilot Putaway and Relocation end-to-end with atomic dual-write and read-only linkage status.
- [x] Preserve LocationMovement history without historic backfill or read cutover.
- [ ] Extend the kernel to transfer/transit through WMS-030 after pilot verification.

### WMS-030 — Transfer and transit integration

- [x] Approved StockTransfer generates Planned outbound/transit/inbound movements per line.
- [x] Existing atomic posting marks stages Posted; cancellation marks linked stages Reversed.
- [x] Preserve current workflow, permissions, valuation and cancellation behavior with explicit transit visibility.
- [x] No historic transfer state is migrated automatically; compatibility fallbacks preserve old records.
- [ ] Separate transit balances and partial dispatch/receipt remain deferred until explicit balances and reservations exist.

### WMS-040 — Explicit balances and availability

- [x] Protected virtual Unassigned/default position with tenant-safe filtered uniqueness.
- [x] Transactionally maintained InventoryBalance projection and guarded migration from verified existing balances.
- [x] OnHand/Reserved/Available definitions, rowversion and warehouse/location indexes.
- [x] Direct SQL reconciliation confirms projection totals equal ProductStock in the development database.
- [x] Disposable SQL integration proves same-transaction projection synchronization and concurrent last-unit protection.
- [ ] Reserved mutation and allocation ownership begin in WMS-050; legacy read cutover remains deferred.

### WMS-050 — Reservations and fulfillment allocation

- [x] Tenant-owned reservation aggregate with Active/Consumed/Released lifecycle and source idempotency.
- [x] Approved transfer reservations, compatibility creation at posting and atomic consumption.
- [x] Exact reservable-location or protected Unassigned allocation with over-reservation rejection.
- [x] Read-only bilingual history, permission and concurrency integration coverage.
- [ ] Staged sale-order reservations wait for a sale-order lifecycle; immediate POS/invoice sales issue stock atomically.

### WMS-060 — Inventory adjustments and cycle counting

- [x] Draft/count/approve/post/cancel workflow with separation of counter and approver.
- [x] Optional blind count and frozen expected dimensional quantities.
- [x] Stale-count and active-reservation posting guards.
- [x] Atomic ProductStock/location update plus valued StockTransaction and compensating StockMovement.
- [x] Bilingual UI, centralized numbering, permissions and SQL integration coverage.
- [ ] Cycle schedules and assignments follow only after the posting workflow is observed in use.

### WMS-070 — Lots, serials and expiration

- [x] WMS-070A tracked dimensional balances, immutable trace history and manufacture/expiration metadata.
- [x] Serial-one database constraint and tenant-product serial uniqueness; lots may span locations.
- [x] Atomic opening allocation that must exactly cover all current dimensional stock before policy activation.
- [x] Bilingual tracking/expiry report, permissions, migration and SQL integration coverage.
- [x] WMS-070B tracked-operation cutover for purchase receipts, FEFO sale/recipe issues, transfer/cancellation and explicit identifier-aware sales/purchase returns.
- [ ] FEFO consumption belongs to WMS-080 after tracked document integration is complete.

### WMS-080 — Removal and putaway strategies

- [x] WMS-080A deterministic tracked-stock removal applies each warehouse's FIFO, FEFO, location-priority or minimize-locations strategy to sales, recipe consumption and transfer allocation.
- [x] WMS-080B1 wholesale sale manual lot/serial override with exact-quantity validation and persisted selection-strategy audit.
- [ ] Extend manual override to POS/transfers, then add untracked exact-location allocation and simple putaway rules.
- Storage categories/capacity dimensions follow measured need.

### WMS-090 — Replenishment, forecasting and alerts

- Min/max/safety stock/lead time/routes, reviewed suggestions and forecast projection.

### WMS-100 — Barcode execution and packages

- Shared scan commands, protected retry/idempotency and package hierarchy.

### WMS-110 — Advanced picking

- Single then batch; wave/cluster only after operational metrics justify them.

### WMS-120 — Valuation expansion and landed costs

- Movement-linked valuation records, landed-cost documents and later optional costing methods under a separate accounting ADR.

## 11. Explicit non-goals for the first implementation slice

- No replacement or deletion of ProductStock, ProductLocationStock, StockTransaction, LocationMovement or StockTransfer.
- No destructive quantity backfill.
- No lots, reservations, packages or advanced picking before movement/balance consistency.
- No automatic order creation from replenishment.
- No change from AVCO.
- No direct EF access from controllers.
- No WMS feature considered complete without Domain/Application/Infrastructure/database/UI/permissions/tests/docs as applicable.

## 12. Stop conditions

Stop implementation and record a decision before proceeding if:

- assigned location quantity exceeds ProductStock and has no explainable transaction;
- a proposed migration needs to delete or rewrite posted history;
- a new projection cannot be deterministically reconciled;
- document and movement status cannot be committed atomically;
- an existing warehouse workflow would lose actions, actors, reasons or cancellation trace;
- multiple costing policies are mixed without an accounting ADR.

## 13. File change map

This is a planning inventory, not authorization to create every file at once. Each phase adds only its vertical-slice files.

### WMS-000 implemented slice — reconciliation baseline

Files to add:

```text
MiniStore.Application/Dtos/Inventory/Reconciliation/InventoryReconciliationDtos.cs
MiniStore.Application/Services/Inventory/InventoryReconciliationService.cs
MiniStore.Domain/Interfaces/Inventory/IInventoryReconciliationRepository.cs
MiniStore.Infrastructure/Repositories/Inventory/InventoryReconciliationRepository.cs
MiniStore.Web/Controllers/Inventory/InventoryReconciliationController.cs
MiniStore.Web/Views/InventoryReconciliation/Index.cshtml
docs/decisions/2026-09-30-inventory-reconciliation-contract.md
tests/InventorySqlIntegration/InventorySqlIntegration.csproj
tests/InventorySqlIntegration/Program.cs
```

Files to modify:

```text
MiniStore.Application/Permissions/PermissionDefinitions.cs
MiniStore.Infrastructure/Persistence/PermissionSeeder.cs (review; it normally discovers definitions)
MiniStore.Web/Navigation/NavigationDefinitions.cs
MiniStore.Web/Program.cs
MiniStore.Web/Resources/SharedResource.ar.resx
tests/SecurityRegression/Program.cs
docs/modules/inventory.md
docs/services/sales-and-inventory-services.md
docs/05_PERMISSIONS.md
docs/07_TESTING.md
docs/screens/screen-map.md
docs/controllers/mvc-controllers.md
docs/01_CODEBASE_MAP.md
docs/TODO.md
docs/ARABIC_SHARED_ROADMAP.md
docs/history/AI_WORKLOG.md
```

No migration is expected for this read-only slice unless measured query plans justify an index.

### WMS-010 — hierarchical locations

Files to add:

```text
MiniStore.Application/Dtos/Inventory/WarehouseLocations/LocationTreeDtos.cs
MiniStore.Application/Services/Inventory/LocationTreeService.cs (only if StorageLocationService would become unfocused)
docs/decisions/<date>-hierarchical-storage-locations.md
MiniStore.Infrastructure/Migrations/<timestamp>_AddLocationHierarchy.cs
```

Files to modify:

```text
MiniStore.Domain/Entities/Inventory/StorageLocation.cs
MiniStore.Domain/Interfaces/Inventory/IStorageLocationRepository.cs
MiniStore.Application/Services/Inventory/StorageLocationService.cs
MiniStore.Application/Dtos/Inventory/Warehouses/CreateStorageLocationDto.cs
MiniStore.Infrastructure/Repositories/Inventory/StorageLocationRepository.cs
MiniStore.Infrastructure/Persistence/Configurations/Inventory/StorageLocationConfiguration.cs
MiniStore.Infrastructure/Migrations/AppDbContextModelSnapshot.cs
MiniStore.Web/Controllers/Inventory/WarehouseLocationsController.cs
MiniStore.Web/Views/WarehouseLocations/Index.cshtml
MiniStore.Web/Views/WarehouseLocations/Create.cshtml
MiniStore.Application/Permissions/PermissionDefinitions.cs
MiniStore.Web/Resources/SharedResource.ar.resx
tests/SecurityRegression/Program.cs
relevant inventory/database/controller/screen/permission docs
```

### WMS-005/WMS-015 — product logistics, attributes and variants

Files likely to add:

```text
MiniStore.Domain/Entities/Catalog/ProductCategory.cs
MiniStore.Domain/Entities/Catalog/ProductTemplate.cs
MiniStore.Domain/Entities/Catalog/ProductAttributeDefinition.cs
MiniStore.Domain/Entities/Catalog/ProductAttributeOption.cs
MiniStore.Domain/Entities/Catalog/ProductAttributeValue.cs
MiniStore.Domain/Entities/Catalog/ProductTrackingPolicy.cs
MiniStore.Domain/Interfaces/Catalog/IProductAttributeRepository.cs
MiniStore.Application/Dtos/Catalog/ProductAttributes/*
MiniStore.Application/Services/Catalog/ProductAttributeService.cs
MiniStore.Infrastructure/Repositories/Catalog/ProductAttributeRepository.cs
MiniStore.Infrastructure/Persistence/Configurations/Catalog/ProductAttribute*.cs
MiniStore.Web/Controllers/Catalog/ProductAttributesController.cs
MiniStore.Web/Views/ProductAttributes/*
docs/decisions/<date>-product-logistics-and-variant-model.md
```

Files to modify:

```text
MiniStore.Domain/Entities/Catalog/Product.cs
MiniStore.Application/Dtos/Catalog/Products/CreateProductDto.cs
MiniStore.Application/Dtos/Catalog/Products/UpdateProductDto.cs
MiniStore.Application/Dtos/Catalog/Products/ProductDto.cs
MiniStore.Application/Services/Catalog/ProductService.cs
MiniStore.Domain/Interfaces/Catalog/IProductRepository.cs
MiniStore.Infrastructure/Repositories/Catalog/ProductRepository.cs
MiniStore.Infrastructure/Persistence/Configurations/Catalog/ProductConfiguration.cs
MiniStore.Web/Controllers/Catalog/ProductsController.cs
MiniStore.Web/Views/Products/{Create,Edit,Index}.cshtml
MiniStore.Infrastructure/Persistence/{AppDbContext,TenantIsolationModel}.cs
MiniStore.Web/Program.cs
MiniStore.Web/Resources/SharedResource.ar.resx
tests/SecurityRegression/Program.cs
product/inventory/database/permission/screen/roadmap docs
```

This phase must not replace ProductId in stock, recipe, purchase, sale or history tables. Concrete Product rows remain the inventory SKUs.

### WMS-020 — physical movement kernel

Files to add:

```text
MiniStore.Domain/Entities/Inventory/StockMovement.cs
MiniStore.Domain/Entities/Inventory/StockMovementStatus.cs
MiniStore.Domain/Entities/Inventory/StockMovementType.cs
MiniStore.Domain/Interfaces/Inventory/IStockMovementRepository.cs
MiniStore.Application/Dtos/Inventory/StockMovements/StockMovementDtos.cs
MiniStore.Application/Services/Inventory/StockMovementService.cs
MiniStore.Infrastructure/Repositories/Inventory/StockMovementRepository.cs
MiniStore.Infrastructure/Persistence/Configurations/Inventory/StockMovementConfiguration.cs
MiniStore.Infrastructure/Migrations/<timestamp>_AddStockMovementKernel.cs
MiniStore.Web/Controllers/Inventory/StockMovementsController.cs
MiniStore.Web/Views/StockMovements/Index.cshtml
docs/decisions/<date>-physical-stock-movement-ledger.md
```

Files to modify first for the pilot:

```text
MiniStore.Application/Services/Inventory/UnassignedStockService.cs
MiniStore.Application/Services/Inventory/LocationMovementService.cs
MiniStore.Infrastructure/Persistence/AppDbContext.cs
MiniStore.Infrastructure/Persistence/TenantIsolationModel.cs
MiniStore.Web/Program.cs
MiniStore.Application/Permissions/PermissionDefinitions.cs
MiniStore.Web/Navigation/NavigationDefinitions.cs
MiniStore.Web/Resources/SharedResource.ar.resx
tests/SecurityRegression/Program.cs
inventory/database/testing/permission/roadmap/worklog docs
```

`LocationMovement` and its table are preserved. The pilot dual-writes and reconciles before any read cutover.

### WMS-030 — transfers and transit

Files likely to add:

```text
MiniStore.Application/Services/Inventory/TransferMovementPlanner.cs
MiniStore.Application/Dtos/Inventory/StockTransfers/TransitStatusDto.cs
docs/decisions/<date>-transfer-transit-movements.md
```

Files to modify:

```text
MiniStore.Domain/Entities/Inventory/StockTransfer.cs
MiniStore.Domain/Entities/Inventory/StockTransferItem.cs
MiniStore.Application/Services/Inventory/StockTransferService.cs
MiniStore.Domain/Interfaces/Inventory/IStockTransferRepository.cs
MiniStore.Infrastructure/Repositories/Inventory/StockTransferRepository.cs
MiniStore.Infrastructure/Persistence/Configurations/Inventory/StockTransfer*.cs
MiniStore.Web/Controllers/Inventory/StockTransfersController.cs
MiniStore.Web/Views/StockTransfers/Details.cshtml
tests and stock-transfer/inventory docs
```

The existing workflow and all actor/reason history remain unchanged unless a separately documented conflict appears.

### WMS-040 — explicit dimensional balances

Files to add:

```text
MiniStore.Domain/Entities/Inventory/InventoryBalance.cs
MiniStore.Domain/Interfaces/Inventory/IInventoryBalanceRepository.cs
MiniStore.Application/Services/Inventory/InventoryBalanceService.cs
MiniStore.Infrastructure/Repositories/Inventory/InventoryBalanceRepository.cs
MiniStore.Infrastructure/Persistence/Configurations/Inventory/InventoryBalanceConfiguration.cs
MiniStore.Infrastructure/Migrations/<timestamp>_AddInventoryBalancesAndDefaultLocations.cs
docs/decisions/<date>-inventory-balance-projection.md
```

Files to modify:

```text
MiniStore.Domain/Entities/Inventory/ProductStock.cs (contract/reconciliation only; keep AVCO)
MiniStore.Domain/Entities/Inventory/ProductLocationStock.cs (compatibility/read cutover)
MiniStore.Application/Services/Inventory/UnassignedStockService.cs
MiniStore.Application/Services/Inventory/ProductStockService.cs
MiniStore.Application/Services/Inventory/LocationMovementService.cs
MiniStore.Application/Services/Inventory/StockTransferService.cs
purchase/sale/return services only when their workflow migrates
AppDbContext, TenantIsolationModel, Program, tests and docs
```

### WMS-050 and later

Create feature folders only in their phase:

```text
Domain/Entities/Inventory/Reservations/*
Application/Dtos/Inventory/Reservations/*
Application/Services/Inventory/ReservationService.cs
Infrastructure/.../Reservation*.cs

Domain/Entities/Inventory/Adjustments/*
Application/.../InventoryAdjustment*.cs
Web/Controllers/Inventory/InventoryAdjustmentsController.cs
Web/Views/InventoryAdjustments/*

Domain/Entities/Inventory/Tracking/{InventoryLot,InventorySerial}.cs
Application/.../Lots/* and Serials/*
Infrastructure/.../InventoryLot* and InventorySerial*

Domain/Entities/Inventory/Replenishment/*
Application/.../Replenishment/*
Web/.../Replenishment/*
```

Exact names and aggregate boundaries are confirmed by the ADR at the start of each phase.

### Existing files explicitly preserved

Do not delete or bulk-rewrite these during early WMS phases:

```text
MiniStore.Domain/Entities/Inventory/ProductStock.cs
MiniStore.Domain/Entities/Inventory/ProductLocationStock.cs
MiniStore.Domain/Entities/Inventory/StockTransaction.cs
MiniStore.Domain/Entities/Inventory/LocationMovement.cs
MiniStore.Domain/Entities/Inventory/StockTransfer*.cs
all existing inventory migrations
all posted purchase/sale/return/transfer records and histories
```

Potential deprecation is limited to old read/write paths after proven cutover. It is never a data-deletion shortcut.
