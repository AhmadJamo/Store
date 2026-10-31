# المهمة التالية الوحيدة / Next Task

## `MOD-030E2A` — سجل وأرصدة المخزون / Inventory Ledger and Stock Master Cutover

> الحالة: APPROVED

اكتمل `MOD-030E1`: أصبحت أربع شاشات مخزون read-only تعمل AngularDefault، ونجح hosted ‏15/15 والقطع والرجوع
على 33 مسارًا. أصبحت تسعة أسطح مفعلة، ويبقى 73 سطحًا mapped يعمل MVC افتراضيًا.

### المتطلب السابق

- StockTransactions وProductStocks لديهما Angular parity كامل وعقود API مستقلة.
- التحويل يقتصر على Index/Create/Edit GET؛ كل إنشاء/تعديل/حذف يبقى عبر API أو MVC rollback المحمي.

### الهدف بعد الموافقة

- إضافة خرائط GET الدقيقة لسجل المخزون وأرصدة المنتج وتفعيل السطحين AngularDefault.
- توسيع مشغل القطع ليطلب صلاحيات View/Create/Edit ويختبر المسارات والقيم الموجبة للمعرفات.
- تشغيل hosted acceptance والقطع والـprocess-local MVC rollback.
- إبقاء Controllers وRazor كاملة كرجوع.

### القبول

- كل GET مصرح يعيد التوجيه إلى هدف Angular المطابق ويحافظ على query string والمعرف.
- rollback يعيد المسارات الأربعة إلى MVC 200.
- البوابة الكاملة تبقى خضراء دون حذف ملفات أو تغيير قاعدة البيانات.
