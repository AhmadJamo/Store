# المهمة التالية الوحيدة / Next Task

## `MOD-030E4H` — عروض الموردين / Supplier Quotations Cutover

> الحالة: APPROVED

اكتمل `MOD-030E4G`: أصبح PurchaseSourcing السطح 38 في AngularDefault بعد hosted ‏15/15 والقطع والرجوع 76/76
بما فيها Details وPrint لحدث توريد موجود دون إنشاء بيانات. بقي 44 سطحًا mapped يعمل MVC افتراضيًا.

### النطاق المحدود

- SupplierQuotations Create GET ذو `sourcingEventId` الموجب إلى `/purchases/sourcing/{id}/quotation`.
- SupplierQuotations Comparison GET ذو `sourcingEventId` الموجب إلى `/purchases/sourcing/{id}/comparison`.
- Create POST وAward POST وكل mutations تبقى خارج presentation cutover.

### القبول

- كل GET مصرح يصل إلى مسار Angular الدقيق ويحافظ على query.
- كل mutation وأي action غير Create/Comparison GET يبقى خارج presentation cutover.
- hosted acceptance والقطع والرجوع والبوابة الكاملة تبقى خضراء بلا تغيير قاعدة بيانات.
