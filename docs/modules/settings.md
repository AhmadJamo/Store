# Settings
> Status: PARTIALLY IMPLEMENTED | Last reviewed: 2026-09-13

`SettingsController` is restricted to Identity `Admin` role (not defined Settings.* permissions). General settings have singleton key and rowversion. Discount settings similarly use a singleton key and enforce discount choices/limits. Invoice settings generate channel-specific invoice numbers and have rowversion but no singleton database guarantee. Document number settings are not exposed through SettingsController.

Sources: setting entities, DTOs, services, repository interfaces/implementations/configurations, `SettingsController`, settings views. Changing settings affects sales and transfer workflows.
