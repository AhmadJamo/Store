# 12 — تقدم إعادة بناء Frontend

> آخر تحديث: 2026-11-25
> الحالة العامة: Products, Contacts and Branches/Warehouses verified; Inventory Availability and Reservations migrated, Stock Transactions safety net next
> المصدر: الكود، الاختبارات، وثائق المشروع، والمراجع الرسمية التي تم التحقق منها

## بوابات المراحل

| المرحلة | الحالة | الدليل | الخطوة التالية |
|---|---|---|---|
| A — Repository Audit | Verified | `01-CURRENT-SYSTEM-AUDIT.md` + route coverage | مغلقة |
| B — Odoo Research | Verified | `02-ODOO-RESEARCH.md` + `03-ODOO-UI-FEATURE-MATRIX.md` | مغلقة |
| C — UX Architecture | Verified | `04-UX-ARCHITECTURE.md` + `06-NAVIGATION-AND-ROUTES.md` + route coverage | مغلقة |
| D — Design System | Verified | 260 tests + build + static gates + Playwright 6/6 | مغلقة |
| E — Application Shell | Verified | 10-App metadata + App Switcher + navigation/context/account hierarchy | مغلقة |
| F — Core Views | Verified pilots | Products, Contacts and Branches/Warehouses acceptance records | Inventory wave |
| G — Module Migration | In Progress | Products, Contacts and Branches/Warehouses migrated | Inventory audit then bounded pilot |
| H — POS Reconstruction | Not Started | — | بعد core patterns |
| I — System Quality | In Progress continuously | 295 tests/build/142-template gate + security 1,668 + Playwright 7/7 | authenticated workflow E2E لاحقاً |
| J — Cleanup & Handover | Not Started | — | بعد اكتمال routes |

## تقدم الوحدات

| Module | Research | Design | Implementation | API Integration | Tests | Status |
|---|---|---|---|---|---|---|
| Shell / Launcher | Verified research | Verified | FRD-E complete | Existing contracts | 270 + Playwright 7/7 | Verified |
| Products | Audited | Verified | FRD-F List/Form/Record | Verified contracts/permissions | 274 + security 1,668 + Playwright 7/7 | Migrated |
| Customers / Suppliers | Audited | Verified | FRD-G List/Form/Record | Verified contracts/permissions | 282 + security 1,668 + Playwright 7/7 | Migrated |
| Sales | Not Started | Not Started | Legacy V3 | Verified | Existing specs | Planned |
| Purchases | Not Started | Not Started | Legacy V3 | Verified | Existing specs | Planned |
| Inventory | Audited | Scoped | Availability + Reservations + Locations V4 | Verified services/APIs/permissions | 299 + focused specs + Playwright 7/7 | Stock Transactions safety net next |
| Accounting | Not Started | Not Started | Legacy V3 | Verified | Existing specs | Planned |
| Branches / Warehouses | Audited | Verified | FRD-H Workspace/Master/Locations | Verified contracts/context | 291 + security 1,668 + Playwright 7/7 | Migrated |
| Users / Roles / Settings | Not Started | Not Started | Legacy V3 | Verified | Existing specs | Planned |
| POS | Verified research | Not Started | Existing cohesive workspace | Verified | Existing specs | Planned |
| Public / Platform / Portal | Not Started | Exception review | Existing | Existing boundaries | Existing specs | Planned exceptions |

## التغييرات المثبتة

| Commit | النطاق | الحالة |
|---|---|---|
| d47e9c0 | V4 shell/app-launcher pilot | Passed automated gates |
| 1356d77 | contextual open-work menu | Passed automated gates |
| 03cd6fa | functional design-system slice | 259 tests/build/accessibility/consistency passed |
| c93406e | FRD-E Application Shell close | 270 tests/build/static/Playwright passed |
| 52f749a + 142324a | FRD-F Product Record/Form completion | 274 tests/build/static/Playwright passed |
| 7a9df93 + face230 | FRD-G Customer/Supplier Records completion | 282 tests/build/static/security/Playwright passed |
| df8e96d + 8fc970f + 34e778c | FRD-H Warehouse Master/Locations/Branch Workspace | 291 tests/build/static/security/Playwright passed |

## قيود حالية

- أُغلق Odoo Research على مرجع 19.0 الرسمي؛ تعذّر توثيق 20.0 بسبب Timeout ولا تُنسب إليه خصائص غير متحققة.
- أُغلق Design System وApplication Shell بعد القبول البصري؛ اكتملت App metadata وApp Switcher وcurrent-app navigation وSection/Archetype metadata وقائمة الحساب/الفرع، وأُغلق Shell SCSS warning بالاستخراج المحصور.
- أضيفت أربع concept mockups كاتجاه بصري، وثُبت baseline قبول Design System الفعلي في `acceptance/design-system-2026-11-24/`.
- Products وContacts وBranches/Warehouses هي الوحدات المصنفة Migrated إلى V4؛ يبقى تكامل API حي مصادق عليه بوابة منفصلة عند توفر environment/state خارجيين.
