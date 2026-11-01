# المهمة التالية الوحيدة / Next Task

## `MOD-030E4B` — مصالحة محاسبة المشتريات / Purchase Accounting Reconciliation Cutover

> الحالة: APPROVED

اكتمل `MOD-030E4A`: أصبح PurchaseAnalytics السطح 32 في AngularDefault بعد hosted ‏15/15 والقطع والرجوع 66/66.
بقي 50 سطحًا mapped يعمل MVC افتراضيًا.

### النطاق المحدود

- PurchaseAccountingReconciliation Index GET إلى `/purchases/accounting-reconciliation`.
- الحفاظ على `asOfDate` و`exceptionsOnly` والاعتماد على `PurchaseAccountingReconciliation.View`.
- كل تسوية أو ترحيل أو تعديل مالي يبقى خارج هذه الدفعة وخارج كتالوج العرض.

### القبول

- كل GET مصرح يصل إلى مسار Angular الدقيق ويحافظ على query.
- كل mutation وأي action غير Index يبقى خارج presentation cutover.
- hosted acceptance والقطع والرجوع والبوابة الكاملة تبقى خضراء بلا تغيير قاعدة بيانات.
