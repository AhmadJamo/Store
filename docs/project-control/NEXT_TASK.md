# المهمة التالية الوحيدة / Next Task

## `MOD-030E2C` — دورة تحويلات المخزون / Stock Transfer Lifecycle Cutover

> الحالة: APPROVED

اكتمل `MOD-030E2B`: أصبحت قائمة/إنشاء/تفاصيل تسويات المخزون AngularDefault، ونجح hosted ‏15/15 والقطع والرجوع
على 40 مسارًا. أصبحت 12 سطحًا مفعلة، ويبقى 70 سطحًا mapped يعمل MVC افتراضيًا.

### المتطلب السابق

- StockTransfers لديه Angular list/create/edit/details parity وعقود API مستقلة لكل دورة العمل.
- التحويل يقتصر على Index/Create/Edit/Details GET؛ submit/approve/reject/post/cancel تبقى mutations مستقلة ومحمية.

### الهدف بعد الموافقة

- إضافة خرائط GET الدقيقة لقائمة/إنشاء/تعديل/تفاصيل تحويل المخزون وتفعيل السطح AngularDefault.
- توسيع مشغل القطع ليطلب صلاحيات View/Create/Edit ويشتق معرف تحويل موجبًا من API read-only.
- تشغيل hosted acceptance والقطع والـprocess-local MVC rollback.
- إبقاء Controllers وRazor كاملة كرجوع.

### القبول

- كل GET مصرح يعيد التوجيه إلى `/inventory/transfers` أو `/new` أو `/:id/edit` أو `/:id` ويحافظ على المعرف.
- rollback يعيد المسارات المتاحة إلى MVC 200 دون تنفيذ lifecycle mutation.
- البوابة الكاملة تبقى خضراء دون حذف ملفات أو تغيير قاعدة البيانات.
