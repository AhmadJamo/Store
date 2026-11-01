# المهمة التالية الوحيدة / Next Task

## `MOD-030E4M` — إشعارات ائتمان الموردين / Supplier Credit Notes Cutover

> الحالة: APPROVED

اكتمل `MOD-030E4L`: أصبحت VendorBills السطح 43 في AngularDefault بعد hosted ‏15/15 والقطع والرجوع 89/89،
وشمل الاختبار فاتورة موجودة للقراءة فقط. بقي 39 سطحًا mapped يعمل MVC افتراضيًا.

### النطاق المحدود

- SupplierCreditNotes Index GET إلى `/purchases/credit-notes`.
- Create GET ذو `vendorBillId` الموجب إلى `/purchases/credit-notes/new`.
- Details GET ذو المعرّف الموجب إلى `/purchases/credit-notes/{id}`.
- Create POST وكل credit-note mutations تبقى خارج presentation cutover.

### القبول

- كل GET مصرح يصل إلى مسار Angular الدقيق ويحافظ على query.
- كل mutation وأي action غير Index/Create/Details GET يبقى خارج presentation cutover.
- hosted acceptance والقطع والرجوع والبوابة الكاملة تبقى خضراء بلا تغيير قاعدة بيانات.
