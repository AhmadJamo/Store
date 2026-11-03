# المهمة التالية الوحيدة / Next Task

## `MOD-030E5R` — احتفاظ تدقيق المنصة / Platform Audit Retention Cutover

> الحالة: APPROVED

اكتمل `MOD-030E5Q`: أصبح PlatformAuditEvents السطح 75 بعد hosted ‏15/15 والقطع/الرجوع 132/132
والبوابة 1,806 مع إثبات عزل جلسة الشركة، بلا تصدير أو تغيير تدقيق. بقي 7 أسطح MVC.

### النطاق المحدود

- تحويل PlatformAuditRetention Index GET إلى `/app/platform-retention`.
- إبقاء إنشاء/تحرير الحجز القانوني، نقاط التثبيت وتشغيل الاحتفاظ تحت سياسة Owner وantiforgery وخارج التحويل.

### القبول

- المسار الدقيق وحدود GET وسياسة Owner مثبتة، والرجوع إلى MVC مثبت بلا تغيير حجز أو نقطة تثبيت أو حذف تدقيق.
