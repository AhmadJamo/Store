# المهمة التالية الوحيدة / Next Task

## `MOD-030E2F` — عمليات المستودع / Warehouse Operations Cohort

> الحالة: APPROVED

اكتمل `MOD-030E2E`: أصبح StockMovements Index القراءة فقط AngularDefault على workspace حركات المواقع المشترك.
نجح hosted ‏15/15 والقطع والرجوع على 44 مسارًا. أصبحت 15 سطحًا مفعلة، ويبقى 67 سطحًا mapped يعمل MVC افتراضيًا.

### المتطلب السابق

- WarehouseLocations وUnassignedStock وLocationMovements وInventoryScanning تملك Angular workspaces وعقود API مستقلة.
- التحويل يقتصر على GET presentation؛ create/update/assign/move/scan/count تبقى mutations مستقلة ومحمية.

### الهدف بعد الموافقة

- إضافة خرائط GET الدقيقة للأسطح الأربعة وتفعيلها كدفعة واحدة محدودة.
- توسيع مشغل القطع بصلاحيات القراءة والإنشاء/التعديل اللازمة لمسارات GET فقط، مع اشتقاق أي معرف حي من API read-only.
- تشغيل hosted acceptance والقطع والـprocess-local MVC rollback.
- إبقاء Controllers وRazor كاملة كرجوع.

### القبول

- كل GET مصرح يصل إلى workspace المستودع الصحيح ويحافظ على query أو المعرف عند الحاجة.
- rollback يعيد المسارات إلى MVC دون تنفيذ أي mutation.
- البوابة الكاملة تبقى خضراء دون حذف ملفات أو تغيير قاعدة البيانات.
