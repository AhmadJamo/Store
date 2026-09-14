# Configurable POS experience per terminal
> Status: Accepted | Date: 2026-09-14

## Context
One company may operate retail, grocery, cafe, restaurant or quick-service terminals. Even terminals inside one branch may need different product density, touch behavior and visible information. A single company-wide POS layout cannot represent those operating contexts accurately.

## Decision
Persist one optional `PosTerminalSettings` row per `PosTerminal` with a shared key and rowversion. Business profiles provide initial presentation presets; administrators may then save validated terminal-specific overrides. The POS loads all terminal runtime settings through the Application service and applies the selected terminal's experience in the browser.

Settings in this phase cover presentation and operator ergonomics only. Sale pricing, warehouse authorization, stock validation and accounting remain enforced by their existing server-side services. Restaurant tables, kitchen routing, modifiers, scale input and other sector workflows will use separate domain features rather than boolean appearance flags.

## Consequences
- Different terminals and branches can use different layouts without duplicating the POS screen.
- Presets make setup fast while custom values support mixed concepts.
- Rowversion prevents an administrator from silently overwriting a concurrent settings update.
- New terminals work immediately with an in-memory Retail default until settings are saved.
- Sector-specific operating workflows still require explicit domain and application designs.
