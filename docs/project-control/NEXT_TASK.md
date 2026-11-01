# المهمة التالية الوحيدة / Next Task

## `MOD-030E4C` — تزويد المشتريات / Purchase Replenishment Cutover

> الحالة: APPROVED

اكتمل `MOD-030E4B`: أصبح PurchaseAccountingReconciliation السطح 33 في AngularDefault بعد hosted ‏15/15 والقطع
والرجوع 67/67. بقي 49 سطحًا mapped يعمل MVC افتراضيًا.

### النطاق المحدود

- PurchaseReplenishment Index GET إلى `/purchases/replenishment`.
- الحفاظ على query والاعتماد على `PurchaseReplenishment.View`.
- حفظ الإعدادات والتشغيل اليدوي يبقيان بصلاحياتهما وantiforgery وخارج كتالوج العرض.

### القبول

- كل GET مصرح يصل إلى مسار Angular الدقيق ويحافظ على query.
- كل mutation وأي action غير Index يبقى خارج presentation cutover.
- hosted acceptance والقطع والرجوع والبوابة الكاملة تبقى خضراء بلا تغيير قاعدة بيانات.
