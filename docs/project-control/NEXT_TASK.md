# المهمة التالية الوحيدة / Next Task

## `MOD-030E4S` — اتصالات الموردين / Supplier Communications Cutover

> الحالة: APPROVED

اكتمل `MOD-030E4R`: أصبحت SupplierPurchasing السطح 49 بعد hosted ‏15/15 والقطع والرجوع 101/101 والبوابة 1,622. لم يوجد صف بيانات شراء مورد لاختبار مسارات الأسعار حياً. بقي 33 سطحًا MVC.

### النطاق المحدود

- SupplierCommunications Details GET ذو المعرّف الموجب إلى `/purchases/communications/{id}`.
- Prepare وRecordActivity POST تبقيان خارج التحويل.
- Attachment GET يبقى تنزيل ملف مباشر خارج presentation cutover.

### القبول

- كل GET مصرح يصل إلى مسار Angular الدقيق ويحافظ على query.
- كل mutation وأي action غير Index/Create/Details GET يبقى خارج presentation cutover.
- hosted acceptance والقطع والرجوع والبوابة الكاملة تبقى خضراء بلا تغيير قاعدة بيانات.
