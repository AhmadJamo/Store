# المهمة التالية الوحيدة / Next Task

## `MOD-030E4P` — قبول خدمات الشراء / Purchase Service Acceptances Cutover

> الحالة: APPROVED

اكتمل `MOD-030E4O`: أصبحت LandedCosts السطح 46 بعد hosted ‏15/15 والقطع والرجوع 95/95 والبوابة 1,592. بقي 36 سطحًا MVC.

### النطاق المحدود

- PurchaseServiceAcceptances Index GET إلى `/purchases/service-acceptances`.
- Create GET ذو `purchaseOrderId` الموجب إلى `/purchases/service-acceptances/new`.
- Details GET ذو المعرّف الموجب إلى `/purchases/service-acceptances/{id}`.
- Create POST يبقى خارج التحويل.

### القبول

- كل GET مصرح يصل إلى مسار Angular الدقيق ويحافظ على query.
- كل mutation وأي action غير Index/Create/Details GET يبقى خارج presentation cutover.
- hosted acceptance والقطع والرجوع والبوابة الكاملة تبقى خضراء بلا تغيير قاعدة بيانات.
