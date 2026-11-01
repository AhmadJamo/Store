# المهمة التالية الوحيدة / Next Task

## `MOD-030E3A` — أسطح الكتالوج الرئيسية المشتركة / Shared Catalog Masters Cutover

> الحالة: APPROVED

اكتمل `MOD-030E2G`: أصبح Inventory Replenishment السطح العشرين AngularDefault بعد hosted ‏15/15 والقطع والرجوع
49/49. بقي 62 سطحًا mapped يعمل MVC افتراضيًا.

### النطاق المحدود

- Branches وPaymentMethods وTaxRates فقط؛ الأسطح الثلاثة تشترك في Angular workspace `/products/catalog-masters`.
- يقتصر كتالوج العرض على Index GET لكل سطح مع حفظ query.
- Create/update/delete تبقى أوامر API/MVC مستقلة بصلاحياتها وantiforgery، ولا تدخل كتالوج العرض.

### الهدف

- تدقيق أن Angular workspace يعرض الأنواع الثلاثة ويطبق صلاحيات كل master بدقة.
- إضافة خرائط Index GET الثلاث وتفعيل الأسطح AngularDefault كدفعة تشغيلية واحدة.
- توسيع مشغل cutover المصرح، ثم إثبات AngularDefault وprocess-local MVC rollback.
- إبقاء Controllers وRazor كاملة كمسار رجوع.

### القبول

- كل Index مصرح يصل إلى `/products/catalog-masters` ويحافظ على query.
- أي action غير Index وأي طلب غير GET/HEAD يبقى خارج presentation cutover.
- hosted acceptance والقطع والرجوع والبوابة الكاملة تبقى خضراء بلا تغيير قاعدة بيانات.
