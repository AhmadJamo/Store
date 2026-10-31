# المهمة التالية الوحيدة / Next Task

## `MOD-030B` — توسيع الدفعة القابلة للرجوع / Expand the Reversible Cutover Cohort

> الحالة: APPROVED — NEXT INCREMENT ضمن موجة القبول الشامل

اكتمل `MOD-030A`: Angular production مستضاف تحت `/app`، نجح deep-link/assets/API separation، ونجح القبول
المصرح 15/15. Customers هو أول AngularDefault cohort، و10 مسارات نجحت في التحقق المختلط.

### الهدف

- تحويل Suppliers وProducts كدفعة ثانية بعد قبول hosted production خاص بهما.
- توسيع allowlist فقط عند الحاجة لمسارات GET المثبتة، مع بقاء mutations على API وMVC rollback.
- تشغيل pilot مختلط ثم MVC rollback للدفعة قبل تثبيت الإعداد.
- عدم تحويل AnalyticsCenter/Settings حتى تتوفر جلسة `Administration.Access` لتغطية Settings.

### القبول

- Suppliers وProducts ينجحان على hosted Angular مع صلاحيات الجلسة والـdeep links.
- mixed pilot يثبت تحويل الدفعة الجديدة وبقاء AnalyticsCenter/Settings على MVC.
- rollback يعيد Customers/Suppliers/Products إلى MVC 200 قبل تثبيت الدفعة.
- البوابة الكاملة تبقى خضراء ولا يحذف أي Razor/controller في هذه الخطوة.
