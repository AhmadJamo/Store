# المهمة التالية الوحيدة / Next Task

## `MOD-030E4A` — تحليلات المشتريات / Purchase Analytics Cutover

> الحالة: APPROVED

اكتمل `MOD-030E3F`: أصبحت ProductTemplates وRecipes السطحين 30 و31 في AngularDefault بعد hosted ‏15/15 والقطع
والرجوع 65/65. بقي 51 سطحًا mapped يعمل MVC افتراضيًا.

### النطاق المحدود

- PurchaseAnalytics Index GET إلى `/purchases/analytics`.
- الحفاظ على فلاتر query والاعتماد على `PurchaseAnalytics.View` دون منح أي صلاحية كتابة.
- كل إنشاء/تعديل/اعتماد/ترحيل لمستندات الشراء يبقى خارج هذه الدفعة وخارج كتالوج العرض.

### القبول

- كل GET مصرح يصل إلى مسار Angular الدقيق ويحافظ على query.
- كل mutation وأي action غير Index يبقى خارج presentation cutover.
- hosted acceptance والقطع والرجوع والبوابة الكاملة تبقى خضراء بلا تغيير قاعدة بيانات.
