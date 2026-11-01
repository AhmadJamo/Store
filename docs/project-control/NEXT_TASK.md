# المهمة التالية الوحيدة / Next Task

## `MOD-030E4J` — استلامات البضاعة / Goods Receipts Cutover

> الحالة: APPROVED

اكتمل `MOD-030E4I`: أصبحت PurchaseOrders السطح 40 في AngularDefault بعد hosted ‏15/15 والقطع والرجوع 80/80
للقائمة وتفاصيل أمر شراء موجود دون إنشاء بيانات. بقي 42 سطحًا mapped يعمل MVC افتراضيًا.

### النطاق المحدود

- GoodsReceipts Index GET إلى `/purchases/receipts` مع الحفاظ على البحث.
- Create GET ذو `purchaseOrderId` الموجب إلى `/purchases/receipts/new`.
- Details/Scan GET ذوا المعرّف الموجب إلى `/purchases/receipts/{id}` و`/purchases/receipts/{id}/scan`.
- كل receipt/scan/inspection/posting/reversal mutations تبقى خارج presentation cutover.

### القبول

- كل GET مصرح يصل إلى مسار Angular الدقيق ويحافظ على query.
- كل mutation وأي action غير Index/Create/Details/Scan GET يبقى خارج presentation cutover.
- hosted acceptance والقطع والرجوع والبوابة الكاملة تبقى خضراء بلا تغيير قاعدة بيانات.
