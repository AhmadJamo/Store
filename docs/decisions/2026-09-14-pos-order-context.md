# Persist POS order context on sales
> Status: Accepted | Date: 2026-09-14

## Context
Restaurant, cafe and quick-service operators need order facts such as dine-in/takeaway, table or pickup reference, guest count and preparation notes. Keeping these values only in browser state would lose them after checkout and prevent later kitchen, service and audit workflows.

## Decision
Store the selected `PosOrderType`, service reference and guest count on `Sale`, and store preparation notes on `SaleItem`. Keep enabled/default order types and capture switches on `PosTerminalSettings`. The browser adapts to those settings for usability, and SaleService loads the terminal setting and enforces it again before creating the sale.

Historical and non-POS sales keep nullable context. Legacy POS without configured terminals remains limited to Walk-in behavior. Order types are a normal enum on Sale; enabled terminal choices use a flags enum so combinations remain compact and validated.

## Consequences
- Completed invoices retain the operational context needed for future kitchen, table and delivery workflows.
- A manipulated form cannot submit a disabled order type or terminal-disabled notes/guest count.
- Business presets configure useful defaults while each terminal remains customizable.
- Table occupancy, kitchen ticket status, modifiers and delivery lifecycle need separate entities rather than additional text fields on Sale.
