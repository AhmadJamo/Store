# Managed measurement units and immutable conversion meaning

## Decision
Store measurement units per tenant with a stable code, display name/symbol, Count/Mass/Volume dimension, positive factor to the dimension base and explicit target precision. Piece, gram and milliliter are the base meanings. Built-in units cannot be deactivated, and units are never hard-deleted through the application.

Conversion is allowed only inside the same dimension and uses `quantity × source factor ÷ target factor`, rounded once to target precision. Custom definitions are created as new records; future product and recipe links will snapshot the conversion meaning needed for historical transactions rather than reinterpret old recipes after a unit change.

## Consequences
The managed catalogue can grow without adding enum members. The legacy enum remains temporarily for product and recipe compatibility until their FK/backfill migration is completed. Reports and import/export must use stable unit codes, not translated names.
