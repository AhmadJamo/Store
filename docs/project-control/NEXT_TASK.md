# المهمة التالية الوحيدة / Next Task

## `MOD-030E4E` — قوالب الشراء / Purchase Templates Cutover

> الحالة: APPROVED

اكتمل `MOD-030E4D`: أصبح PurchaseApprovalRules السطح 35 في AngularDefault بعد hosted ‏15/15 والقطع والرجوع
69/69. بقي 47 سطحًا mapped يعمل MVC افتراضيًا.

### النطاق المحدود

- PurchaseTemplates Index GET إلى `/purchases/templates`.
- الحفاظ على query والاعتماد على `PurchaseTemplates.View`.
- CreateFromOrder وGenerate وSetActive تبقى بصلاحياتها وantiforgery وخارج كتالوج العرض.

### القبول

- كل GET مصرح يصل إلى مسار Angular الدقيق ويحافظ على query.
- كل mutation وأي action غير Index يبقى خارج presentation cutover.
- hosted acceptance والقطع والرجوع والبوابة الكاملة تبقى خضراء بلا تغيير قاعدة بيانات.
