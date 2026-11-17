# تقرير التحقق والاختبارات — V3.0

## Baseline معتمد عند بدء Recovery

| البوابة | النتيجة | الملاحظة |
|---|---|---|
| Release Web build | Passed | صفر أخطاء/تحذيرات في ERP-010 |
| SecurityRegression | 1,649 Passed | آخر تشغيل قبل REC-000 |
| Angular unit | 250/250 Passed | 120 test files |
| Angular production build | Passed with known warning | initial chunk +24.88 kB |
| Edge acceptance | 15/15 Passed | ERP-009 scope؛ ليس كل النظام |

## حدود الدليل

- لا يوجد بعد E2E شامل للفروع create/edit/deactivate؛ REC-001 يجب أن يضيفه.
- أضيف في REC-002 اختبار SQL موثق لحذف المستودع المرتبط وغير المستخدم.
- قبول Edge السابق خاص بنطاق POS/retired routes ولا يثبت 196 مسارًا.
- `dotnet build MiniStore.sln` غير صالح كدليل لأنه لا يحتوي مشاريع؛ استخدم `MiniStore.slnx` أو csproj الفعلية.

## TASK-REC-001 — 2026-11-17

| البوابة | النتيجة |
|---|---|
| Web Release build | Passed — 0 warnings / 0 errors |
| SecurityRegression | Passed — 1,650 checks |
| Angular unit | Passed — 250/250 in 120 files |
| Angular production build | Passed — known initial chunk warning unchanged |
| Lazy chunk policy | Passed — 81,895 / 81,920 bytes |
| Edge end-to-end | Not claimed — shared acceptance stopped at stale `#preview-title` selector |

تغطي اختبارات العميل عقود POST/PUT/status، وتغطي SecurityRegression وجود الصلاحيات وantiforgery على أوامر الفروع. ما زال تحقق المتصفح الكامل create/reload/edit/status مطلوبًا عند إصلاح fixture القبول العام.
## TASK-REC-002 — 2026-11-17

| البوابة | النتيجة |
|---|---|
| Web Release build | Passed — 0 warnings / 0 errors |
| InventorySqlIntegration | Passed — referenced warehouse and stock preserved; unused warehouse removed |
| Angular unit | Passed — 251/251 in 120 files |
| SecurityRegression | Passed — 1,650 checks |

اختبار SQL استخدم migrations والعلاقات الحقيقية، وليس mock. لا يوجد تغيير schema أو migration في هذه المرحلة.
## TASK-REC-003 — 2026-11-17

| البوابة | النتيجة |
|---|---|
| Route/function generator | Passed — 196 routes |
| P1 unresolved wiring | 0 |
| P2 manual authorization review | 7 |
| Critical modules | Accounting, Sales, Inventory present with 0 P1 |
| `Test-RouteFunctionCoverage.ps1` | Passed |

هذه بوابة wiring/evidence وليست بديلًا عن اختبارات قواعد العمل أو E2E لكل عملية.
## TASK-REC-004A — 2026-11-17

| البوابة | النتيجة |
|---|---|
| Angular unit | 256/256 Passed — 122 files |
| Production build | Passed |
| Initial bundle | 340,416 / 358,400 bytes |
| Largest lazy chunk | 81,895 / 81,920 bytes |

السياسة المختبرة: conflict mutation يظهر كتـوستر؛ field validation و401 لا يتكرران في التوستر؛ الطابور محدود ويزول تلقائيًا.
## TASK-REC-004B1 — 2026-11-17

| الفحص | النتيجة |
|---|---|
| Angular unit | 258/258، 122 ملفًا |
| Security/inventory regression | 1,652 ناجحًا |
| Solution Release | ناجح، 0 تحذير/0 خطأ قبل قفل الخادم العامل لملفات Release |
| Angular production | ناجح |
| Bundle gate | initial 340,433/358,400؛ lazy 81,895/81,920 |

ثبتت الاختبارات أن 409 غير المرمز يصنف `conflict`، وأن `concurrency` لا يستخدم إلا إذا أرسله الخادم صراحة، وأن 400 يبقى `validation`.