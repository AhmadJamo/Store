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
- لا يوجد بعد اختبار SQL موثق لحذف مستودع مرتبط؛ REC-002.
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