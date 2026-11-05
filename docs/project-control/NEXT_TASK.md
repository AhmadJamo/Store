# المهمة التالية الوحيدة / Next Task

## `MOD-030F` — جرد جاهزية تقاعد MVC / Physical-Retirement Readiness Inventory

> الحالة: APPROVED — NON-DESTRUCTIVE INVENTORY ONLY

اكتملت `UX-001..008` ضمن نطاق العرض، وأصبحت كل أسطح العرض المصنفة AngularDefault مع بقاء MVC كمسار رجوع.
هذه المهمة تحصر الاعتماديات قبل أي اقتراح حذف؛ لا تعطي موافقة لحذف ملف أو إزالة route أو mutation أو download.

### النطاق المحدود

- جرد Views/controllers/actions والروابط والتنزيلات والطباعة وPOST commands التي ما زالت تعتمد على MVC.
- فصل أسطح العرض القابلة للتقاعد عن mutation-only compatibility والرجوع التشغيلي.
- تقسيم المرشحين إلى دفعات صغيرة قابلة للرجوع مع متطلبات اختبار ونشر واضحة.
- تسجيل الموانع والاعتماديات دون تعديل سلوك الأعمال أو العرض الافتراضي.

### القبول

- كل مرشح مرتبط بمصدره، مساره، بديله Angular/API، اعتمادياته وخطة رجوعه.
- لا يوجد حذف أو تعطيل أو تغيير route في هذه المهمة.
- تحديث خارطة الطريق والحالة والمخاطر بنتيجة قابلة لإعادة الجرد.

### خارج النطاق

- حذف Razor views أو controllers أو localization أو static assets.
- إزالة MVC services/mutations/downloads/print endpoints.
- تغيير API/DTO/schema/permission/business rule.
- إعلان retirement أو production readiness دون موافقة منفصلة لكل دفعة.
