# المهمة التالية الوحيدة / Next Task

## `MOD-030E2D` — التتبع والتشغيل / Inventory Tracking Cutover

> الحالة: APPROVED

اكتمل `MOD-030E2C`: أصبحت قائمة/إنشاء/تعديل/تفاصيل تحويلات المخزون AngularDefault، ونجح hosted ‏15/15 والقطع
والرجوع على 42 مسارًا. أصبحت 13 سطحًا مفعلة، ويبقى 69 سطحًا mapped يعمل MVC افتراضيًا.

### المتطلب السابق

- InventoryTracking لديه Angular workspace وعقد API يقرأ نفس Application projection المستخدم في MVC.
- التحويل يقتصر على Index GET؛ open/quarantine/release/create-recall/close-recall/communication تبقى mutations مستقلة ومحمية.

### الهدف بعد الموافقة

- إضافة خريطة GET الدقيقة لـInventoryTracking Index وتفعيل السطح AngularDefault.
- توسيع مشغل القطع ليطلب `InventoryTracking.View` ويثبت صفحة التتبع المصرح بها دون تنفيذ أي mutation.
- تشغيل hosted acceptance والقطع والـprocess-local MVC rollback.
- إبقاء Controllers وRazor كاملة كرجوع.

### القبول

- Index المصرح يعيد التوجيه إلى `/inventory/tracking` ويحافظ على query string إن وجد.
- rollback يعيد المسار إلى MVC 200 دون تنفيذ open/quarantine/release/recall mutation.
- البوابة الكاملة تبقى خضراء دون حذف ملفات أو تغيير قاعدة البيانات.
