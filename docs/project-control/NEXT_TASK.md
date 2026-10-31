# المهمة التالية الوحيدة / Next Task

## `MOD-030E2B` — دورة تسويات المخزون / Inventory Adjustment Lifecycle Cutover

> الحالة: APPROVED

اكتمل `MOD-030E2A`: أصبحت شاشتا سجل الحركات وأرصدة المنتجات تعملان AngularDefault، ونجح hosted ‏15/15
والقطع والرجوع على 38 مسارًا. أصبحت 11 سطحًا مفعلة، ويبقى 71 سطحًا mapped يعمل MVC افتراضيًا.

### المتطلب السابق

- InventoryAdjustments لديه Angular list/create/details parity وعقود API مستقلة لكل دورة العمل.
- التحويل يقتصر على Index/Create/Details GET؛ العد والاعتماد والترحيل والإلغاء تبقى API mutations مستقلة ومحمية.

### الهدف بعد الموافقة

- إضافة خرائط GET الدقيقة لقائمة/إنشاء/تفاصيل تسوية المخزون وتفعيل السطح AngularDefault.
- توسيع مشغل القطع ليطلب صلاحيات View/Create ويشتق معرف تسوية موجبًا من API read-only.
- تشغيل hosted acceptance والقطع والـprocess-local MVC rollback.
- إبقاء Controllers وRazor كاملة كرجوع.

### القبول

- كل GET مصرح يعيد التوجيه إلى `/inventory/adjustments` أو `/new` أو `/:id` ويحافظ على query string والمعرف.
- rollback يعيد المسارات الثلاثة إلى MVC 200.
- البوابة الكاملة تبقى خضراء دون حذف ملفات أو تغيير قاعدة البيانات.
