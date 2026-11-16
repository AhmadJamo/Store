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
