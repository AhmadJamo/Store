# المهمة التالية الوحيدة / Next Task

## `MOD-030E4I` — أوامر الشراء / Purchase Orders Cutover

> الحالة: APPROVED

اكتمل `MOD-030E4H`: أصبحت SupplierQuotations السطح 39 في AngularDefault بعد hosted ‏15/15 والقطع والرجوع 78/78
لمساري Create/Comparison GET باستخدام حدث توريد موجود دون إنشاء بيانات. بقي 43 سطحًا mapped يعمل MVC افتراضيًا.

### النطاق المحدود

- PurchaseOrders Index GET إلى `/purchases/orders` مع الحفاظ على filters.
- PurchaseOrders Details GET ذو المعرّف الموجب إلى `/purchases/orders/{id}`.
- CreateFromAward وكل lifecycle/fulfillment mutations تبقى خارج presentation cutover.

### القبول

- كل GET مصرح يصل إلى مسار Angular الدقيق ويحافظ على query.
- كل mutation وأي action غير Index/Details GET يبقى خارج presentation cutover.
- hosted acceptance والقطع والرجوع والبوابة الكاملة تبقى خضراء بلا تغيير قاعدة بيانات.
