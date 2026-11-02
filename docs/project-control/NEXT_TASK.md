# المهمة التالية الوحيدة / Next Task

## `MOD-030E4W` — تسعير المبيعات / Sales Pricing Cutover

> الحالة: APPROVED

اكتمل `MOD-030E4V`: أصبحت Purchases وPurchaseReturns السطحين 53 و54 بعد hosted ‏15/15 والقطع والرجوع 104/104 والبوابة 1,650. لم توجد عينة تاريخية لاختبار التفاصيل أو نموذج المرتجع حياً، ولم يُنشأ مستند. بقي 28 سطحًا MVC.

### النطاق المحدود

- SalesPriceLists Index GET إلى `/sales/pricing` مع الحفاظ على `id` المحدد وأي query آمن.
- Create وSetListActive وAddRule وSetRuleActive وEvaluate POST تبقى خارج التحويل.
- تبقى صلاحيات View/Manage وantiforgery وقواعد التسعير في Application كما هي.

### القبول

- Index GET المصرح يصل إلى مسار Angular الدقيق ويحافظ على query.
- كل mutation والتقييم POST يبقى خارج presentation cutover.
- hosted acceptance والقطع والرجوع والبوابة الكاملة تبقى خضراء بلا إنشاء أو تغيير قائمة/قاعدة أسعار.
