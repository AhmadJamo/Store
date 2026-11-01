# المهمة التالية الوحيدة / Next Task

## `MOD-030E4K` — مرتجعات الاستلام / Goods Receipt Returns Cutover

> الحالة: APPROVED

اكتمل `MOD-030E4J`: أصبحت GoodsReceipts السطح 41 في AngularDefault بعد hosted ‏15/15 والقطع والرجوع 84/84
للقائمة والإنشاء والتفاصيل والمسح باستخدام بيانات موجودة دون إنشاء بيانات. بقي 41 سطحًا mapped يعمل MVC افتراضيًا.

### النطاق المحدود

- GoodsReceiptReturns Index GET إلى `/purchases/receipt-returns`.
- Create GET ذو `goodsReceiptId` الموجب إلى `/purchases/receipt-returns/new`.
- Details GET ذو المعرّف الموجب إلى `/purchases/receipt-returns/{id}`.
- Create POST وكل return mutations تبقى خارج presentation cutover.

### القبول

- كل GET مصرح يصل إلى مسار Angular الدقيق ويحافظ على query.
- كل mutation وأي action غير Index/Create/Details GET يبقى خارج presentation cutover.
- hosted acceptance والقطع والرجوع والبوابة الكاملة تبقى خضراء بلا تغيير قاعدة بيانات.
