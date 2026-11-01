# المهمة التالية الوحيدة / Next Task

## `MOD-030E4F` — طلبات الشراء / Purchase Requests Cutover

> الحالة: APPROVED

اكتمل `MOD-030E4E`: أصبح PurchaseTemplates السطح 36 في AngularDefault بعد hosted ‏15/15 والقطع والرجوع
70/70. بقي 46 سطحًا mapped يعمل MVC افتراضيًا.

### النطاق المحدود

- PurchaseRequests Index GET إلى `/purchases/requests` وCreate GET إلى `/purchases/requests/new`.
- Details GET ذو المعرّف الموجب إلى `/purchases/requests/{id}` مع الحفاظ على query.
- Create POST وSubmit/Cancel/Approve/Reject تبقى بصلاحياتها وantiforgery وخارج كتالوج العرض.

### القبول

- كل GET مصرح يصل إلى مسار Angular الدقيق ويحافظ على query.
- كل mutation وأي action غير Index يبقى خارج presentation cutover.
- hosted acceptance والقطع والرجوع والبوابة الكاملة تبقى خضراء بلا تغيير قاعدة بيانات.
