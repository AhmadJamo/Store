# المهمة التالية الوحيدة / Next Task

## `MOD-030E3D` — سجل تدقيق الشركة / Tenant Audit Events Cutover

> الحالة: APPROVED

اكتمل `MOD-030E3C`: أصبح Warehouses Index السطح السادس والعشرين AngularDefault بعد hosted ‏15/15 والقطع والرجوع
56/56. بقي 56 سطحًا mapped يعمل MVC افتراضيًا.

### النطاق المحدود

- AuditEvents الخاص بالشركة فقط؛ لا يشمل PlatformAuditEvents ذي مصادقة المنصة المنفصلة.
- يضاف Index GET إلى `/audit-events` مع حفظ query.
- Export POST وأي retention mutation تبقى مستقلة بصلاحياتها وantiforgery وخارج كتالوج العرض.

### الهدف

- تدقيق أن Angular يعرض سجل الشركة tenant-scoped بالفلاتر والأدلة الحالية دون mutation.
- تحويل Index فقط وتوسيع مشغل cutover المصرح.
- إثبات AngularDefault وprocess-local MVC rollback مع إبقاء Controller وRazor كاملين.

### القبول

- AuditEvents Index المصرح يصل إلى `/audit-events` ويحافظ على query.
- Export وكل non-GET/HEAD يبقى خارج presentation cutover.
- hosted acceptance والقطع والرجوع والبوابة الكاملة تبقى خضراء بلا تغيير قاعدة بيانات.
