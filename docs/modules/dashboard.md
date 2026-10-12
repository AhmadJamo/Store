# Dashboard application launcher
> Status: IMPLEMENTED | Last reviewed: 2026-10-12

## Purpose and behavior
The authenticated Dashboard is an Odoo-inspired application launcher rather than an empty landing page. It presents Point of Sale, Sales, Purchasing, Inventory, Products, Accounting and Administration as responsive application cards with distinct icons, colors, plain-language descriptions and direct shortcuts to common screens.

The launcher is permission-aware. An application is shown only when at least one of its destinations is authorized, each shortcut is filtered independently through `IPermissionService`, and the card opens the first authorized destination. A client-side localized search filters applications without a server request. The layout supports English and Arabic, including RTL, and remains usable on mobile widths.

## Source files
`Web/Navigation/{DashboardAppDefinition,DashboardDefinitions}.cs`, `Web/Controllers/Home/DashboardController.cs`, `Web/Views/Dashboard/Index.cshtml`, shared localization resources and permission regression checks.

## If this changes
Review application destinations and permissions together, Arabic localization, Dashboard view responsiveness, codebase/screen maps and regression coverage. Never expose a shortcut only because its application card is visible; every destination remains independently permission-filtered and controller authorization remains authoritative.
