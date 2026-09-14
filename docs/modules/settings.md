# Settings
> Status: PARTIALLY IMPLEMENTED | Last reviewed: 2026-09-13

`SettingsController` is restricted to Identity `Admin` role (not defined Settings.* permissions). General, Discount and Inventory settings use singleton keys and rowversions. Inventory settings define safe defaults for new warehouse type, control mode, picking strategy, POS eligibility, location-capacity enforcement and optional exact transfer-location requirements. Effective operational policy can then be adjusted per warehouse. Invoice settings generate channel-specific invoice numbers and have rowversion but no singleton database guarantee. Document number settings are not exposed through SettingsController.

Sources: setting entities, DTOs, services, repository interfaces/implementations/configurations, `SettingsController`, settings views. Changing settings affects sales and transfer workflows.

Inventory Access configures branch warehouse operation permissions/priorities, creates POS terminals and assigns each terminal a default and prioritized alternatives.
