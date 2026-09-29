# Product logistics and tracking-policy foundation

Date: 2026-09-30  
Status: Accepted

## Context

Warehouse capacity, putaway compatibility and later lot/serial traceability require product metadata beyond the commercial ProductType and stock unit. Different industries also need categories without replacing the Product identity used by purchases, sales, recipes and inventory history.

## Decision

Product remains the concrete stockable SKU and keeps its existing Id. Add an optional tenant-owned ProductCategory, nullable net/gross weight with an explicit Mass unit, nullable length/width/height with one explicit Length unit, handling flags and a None/Lot/Serial tracking policy. Volume is derived from the three dimensions rather than stored as independent editable truth.

MeasurementDimension gains Length with millimeter as its base; managed defaults are mm, cm, m and inch. Existing products backfill to no category, unknown measurements, no handling requirements and TrackingPolicy.None. Category codes are language-neutral and tenant-unique.

Changing a product from None to Lot or Serial is rejected while any warehouse ProductStock balance is non-zero. WMS-005 stores the policy but does not fabricate lots/serials or claim traceability execution; receipt/allocation execution arrives in WMS-070. ProductTemplate and flexible typed attributes remain WMS-015.

## Consequences

Future capacity and putaway rules can convert physical values through managed units, and business catalogues can organize products without adding industry-specific nullable columns. Tracked products with opening balances require the later opening-allocation workflow. Product IDs and all existing document/history relationships remain unchanged.
