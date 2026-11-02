# المهمة التالية الوحيدة / Next Task

## `MOD-030E4Q` — اتفاقيات الشراء / Purchase Agreements Cutover

> الحالة: APPROVED

اكتمل `MOD-030E4P`: أصبحت PurchaseServiceAcceptances السطح 47 بعد hosted ‏15/15 والقطع والرجوع 97/97 والبوابة 1,601. لم يوجد قبول خدمة محفوظ لاختبار Details حياً. بقي 35 سطحًا MVC.

### النطاق المحدود

- PurchaseAgreements Index GET إلى `/purchases/agreements`.
- Create GET ذو `sourcingEventId` الموجب إلى `/purchases/agreements/new`.
- Details GET ذو المعرّف الموجب إلى `/purchases/agreements/{id}`.
- Create/Activate/Cancel/Release POST تبقى خارج التحويل.

### القبول

- كل GET مصرح يصل إلى مسار Angular الدقيق ويحافظ على query.
- كل mutation وأي action غير Index/Create/Details GET يبقى خارج presentation cutover.
- hosted acceptance والقطع والرجوع والبوابة الكاملة تبقى خضراء بلا تغيير قاعدة بيانات.
