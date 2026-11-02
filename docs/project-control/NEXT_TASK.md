# المهمة التالية الوحيدة / Next Task

## `MOD-030E4O` — التكاليف الإضافية / Landed Costs Cutover

> الحالة: APPROVED

اكتمل `MOD-030E4N`: أصبحت PurchaseDocumentReversals السطح 45 بعد hosted ‏15/15 والقطع والرجوع 92/92. لم يوجد سجل عكس محفوظ لاختبار Details حياً. بقي 37 سطحًا MVC.

### النطاق المحدود

- LandedCosts Index GET إلى `/purchases/landed-costs`.
- Create GET ذو `goodsReceiptId` الموجب إلى `/purchases/landed-costs/new`.
- Details GET ذو المعرّف الموجب إلى `/purchases/landed-costs/{id}`.
- Create POST يبقى خارج التحويل.

### القبول

- كل GET مصرح يصل إلى مسار Angular الدقيق ويحافظ على query.
- كل mutation وأي action غير Index/Create/Details GET يبقى خارج presentation cutover.
- hosted acceptance والقطع والرجوع والبوابة الكاملة تبقى خضراء بلا تغيير قاعدة بيانات.
