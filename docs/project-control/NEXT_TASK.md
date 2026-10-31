# المهمة التالية الوحيدة / Next Task

## `MOD-030E2E` — سجل حركات المواقع / Stock Movements Read-only Cutover

> الحالة: APPROVED

اكتمل `MOD-030E2D`: أصبحت صفحة InventoryTracking Index ‏AngularDefault مع بقاء كل عمليات الكتابة خارج التحويل.
نجح hosted ‏15/15 والقطع والرجوع على 43 مسارًا. أصبحت 14 سطحًا مفعلة، ويبقى 68 سطحًا mapped يعمل MVC افتراضيًا.

### المتطلب السابق

- StockMovements سطح قراءة فقط وله Angular route وعقد API محمي بـ`StockMovements.View`.
- Controller يملك Index GET واحدًا؛ لا توجد mutation ضمن هذا السطح.

### الهدف بعد الموافقة

- إضافة خريطة GET الدقيقة لـStockMovements Index وتفعيل السطح AngularDefault.
- توسيع مشغل القطع ليطلب `StockMovements.View` ويثبت query-string preservation.
- تشغيل hosted acceptance والقطع والـprocess-local MVC rollback.
- إبقاء Controllers وRazor كاملة كرجوع.

### القبول

- Index المصرح يعيد التوجيه إلى `/inventory/location-movements` ويحافظ على query string.
- rollback يعيد المسار إلى MVC 200، ولا توجد mutation ضمن السطح.
- البوابة الكاملة تبقى خضراء دون حذف ملفات أو تغيير قاعدة البيانات.
