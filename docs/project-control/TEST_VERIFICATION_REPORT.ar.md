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
## TASK-REC-004B2 — 2026-11-17

| الفحص | النتيجة |
|---|---|
| Release isolated build | ناجح، 0 تحذير/0 خطأ مع بقاء الخادم العامل |
| Security/inventory regression | 1,656 ناجحًا؛ يغطي 404 وconflict وduplicate وconcurrency وcorrelationId |
| Angular unit | 258/258، 122 ملفًا |
| Production + bundle | ناجح؛ 340,433 initial و81,895 lazy ضمن الحدود |
## TASK-REC-004B3 — 2026-11-17

| الفحص | النتيجة |
|---|---|
| Release isolated build | ناجح، 0 تحذير/0 خطأ |
| Regression | 1,658 ناجحًا مع حارسي Purchase Requests وStock Transfers |
| Angular unit | 258/258، 122 ملفًا |
| Production + bundle | ناجح؛ 340,433 initial و81,895 lazy ضمن الحدود |
| Remaining classification baseline | 70 API controllers؛ 959 legacy throw sites؛ 51 typed sites |
## TASK-REC-004B4 — 2026-11-17

| الفحص | النتيجة |
|---|---|
| Isolated Release regression | 1,661 ناجحًا |
| Contract guards | وراثة duplicate/concurrency ومنع message matching للمنتجات ناجحة |
| Angular unit | 258/258، 122 ملفًا |
| Production + bundle | ناجح؛ 340,433 initial و81,895 lazy |
| Application classification | 952 legacy؛ 58 typed |
## TASK-REC-004B5 — 2026-11-17

| الفحص | النتيجة |
|---|---|
| Release | ناجح، 0 تحذير/0 خطأ |
| Regression | 1,662 ناجحًا |
| Catalog message-matching guard | ناجح على Categories/Attributes/Templates |
| Angular | 258/258 |
| Production + bundle | 340,433 initial؛ 81,895 lazy ضمن الحدود |
| Classification | 924 legacy؛ 86 typed |

## TASK-REC-004B6 — 2026-11-17

| الفحص | النتيجة |
|---|---|
| Release | ناجح، 0 تحذير/0 خطأ |
| Regression | 1,664 ناجحًا |
| Typed API/service guards | ناجحة للخدمات الأربع والـAPIs الثلاثة |
| Angular | 258/258، 122 ملفًا |
| Production + bundle | 340,433 initial؛ 81,895 lazy ضمن الحدود |
| Classification | 915 legacy؛ 97 typed |

## TASK-REC-004B7 — 2026-11-17

| الفحص | النتيجة |
|---|---|
| Release | ناجح، 0 تحذير/0 خطأ |
| Regression | 1,665 ناجحًا |
| Settings service guard | ناجح للخدمات الخمس |
| Angular | 258/258، 122 ملفًا |
| Production + bundle | 340,433 initial؛ 81,895 lazy ضمن الحدود |
| Classification | 905 legacy؛ 107 typed |

## TASK-REC-004B8 — 2026-11-17

| الفحص | النتيجة |
|---|---|
| Release | ناجح، 0 تحذير/0 خطأ |
| Regression | 1,666 ناجحًا |
| Document-design guards | خدمة وAPI ناجحان |
| Angular | 258/258، 122 ملفًا |
| Production + bundle | 340,433 initial؛ 81,895 lazy ضمن الحدود |
| Classification | 897 legacy؛ 115 typed |
