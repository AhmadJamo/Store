# المهمة التالية الوحيدة / Next Task

## `MOD-030E4D` — قواعد اعتماد الشراء / Purchase Approval Rules Cutover

> الحالة: APPROVED

اكتمل `MOD-030E4C`: أصبح PurchaseReplenishment السطح 34 في AngularDefault بعد hosted ‏15/15 والقطع والرجوع
68/68. بقي 48 سطحًا mapped يعمل MVC افتراضيًا.

### النطاق المحدود

- PurchaseApprovalRules Index GET إلى `/purchases/approval-rules`.
- الحفاظ على query والاعتماد على `PurchaseApprovalRules.View`.
- إنشاء القاعدة وتغيير حالتها يبقيان بصلاحية Manage وantiforgery وخارج كتالوج العرض.

### القبول

- كل GET مصرح يصل إلى مسار Angular الدقيق ويحافظ على query.
- كل mutation وأي action غير Index يبقى خارج presentation cutover.
- hosted acceptance والقطع والرجوع والبوابة الكاملة تبقى خضراء بلا تغيير قاعدة بيانات.
