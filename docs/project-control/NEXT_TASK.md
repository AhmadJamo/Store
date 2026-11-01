# المهمة التالية الوحيدة / Next Task

## `MOD-030E4L` — فواتير الموردين / Vendor Bills Cutover

> الحالة: APPROVED

اكتمل `MOD-030E4K`: أصبحت GoodsReceiptReturns السطح 42 في AngularDefault بعد hosted ‏15/15 والقطع والرجوع 86/86.
غُطيت القائمة والإنشاء باستلام موجود، ولم يوجد مرتجع محفوظ لاختبار Details حياً. بقي 40 سطحًا mapped يعمل MVC افتراضيًا.

### النطاق المحدود

- VendorBills Index GET إلى `/purchases/bills` مع الحفاظ على البحث.
- Create GET ذو `purchaseOrderId` الموجب إلى `/purchases/bills/new`.
- Details GET ذو المعرّف الموجب إلى `/purchases/bills/{id}`.
- المرفقات وكل bill lifecycle/accounting mutations تبقى خارج presentation cutover.

### القبول

- كل GET مصرح يصل إلى مسار Angular الدقيق ويحافظ على query.
- كل mutation وأي action غير Index/Create/Details GET يبقى خارج presentation cutover.
- hosted acceptance والقطع والرجوع والبوابة الكاملة تبقى خضراء بلا تغيير قاعدة بيانات.
