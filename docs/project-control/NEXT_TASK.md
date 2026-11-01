# المهمة التالية الوحيدة / Next Task

## `MOD-030E4G` — توريد طلبات الشراء / Purchase Sourcing Cutover

> الحالة: APPROVED

اكتمل `MOD-030E4F`: أصبح PurchaseRequests السطح 37 في AngularDefault بعد hosted ‏15/15 والقطع والرجوع 73/73
بما فيها Details لطلب موجود دون إنشاء بيانات. بقي 45 سطحًا mapped يعمل MVC افتراضيًا.

### النطاق المحدود

- PurchaseSourcing Index GET إلى `/purchases/sourcing`.
- Details/Print GET ذوا المعرّف الموجب إلى `/purchases/sourcing/{id}` و`/purchases/sourcing/{id}/print`.
- Create/Direct forms وAttachment download وكل mutations تبقى خارج الدفعة إلى أن يثبت URL intent لكل منها.

### القبول

- كل GET مصرح يصل إلى مسار Angular الدقيق ويحافظ على query.
- كل mutation وأي action غير Index يبقى خارج presentation cutover.
- hosted acceptance والقطع والرجوع والبوابة الكاملة تبقى خضراء بلا تغيير قاعدة بيانات.
