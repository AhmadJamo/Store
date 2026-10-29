# المهمة التالية الوحيدة / Next Task

## `MOD-027E` — تدقيق المنصة والاحتفاظ / Platform audit and retention

> الحالة: APPROVED — NEXT INCREMENT ضمن موجة SaaS

اكتمل `MOD-027A..D` حتى عمليات إدارة المنصة.

### الهدف

- نقل قراءة وتصفية وتصدير أحداث تدقيق المنصة.
- نقل حالة الاحتفاظ ونقاط التثبيت والحجوزات القانونية وإطلاقها.
- الحفاظ على سياسات AuditView/Export/Sensitive/Retention المنفصلة.

### القبول

- تغطية كل عمليات `PlatformAuditEventsController` و`PlatformAuditRetentionController`.
- كل mutation يحتفظ بالسياسة وantiforgery والتدقيق والتنقيح الحالي.
- لا تكشف الواجهة أدلة حساسة دون سياسة Sensitive.
- العربية/الإنجليزية وRTL والوصولية وبوابة القبول كاملة، مع بقاء MVC افتراضيًا.
