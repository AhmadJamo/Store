# المهمة التالية الوحيدة / Next Task

## `MOD-030E4Z` — تسليمات البيع / Sales Deliveries Cutover

> الحالة: APPROVED

اكتمل `MOD-030E4Y`: أصبحت SalesOrders السطح 57 بعد hosted ‏15/15 ومجموعة القطع/الرجوع 104/104 والبوابة 1,671. لم تحمل الجلسة SalesOrders.View ولم يتغير أمر أو حجز. بقي 25 سطحًا MVC.

### النطاق المحدود

- SalesDeliveries Index وDetails وCreate ذو salesOrderId موجب إلى `/sales/deliveries` و`/sales/deliveries/{id}` و`/sales/deliveries/new?salesOrderId={id}`.
- الحفاظ على search في القائمة.
- Create/Post/Cancel POST تبقى خارج التحويل وتعمل عبر Angular API المحمي.

### القبول

- كل GET مصرح يصل إلى مسار Angular الدقيق ويحافظ على query والمعرّف الموجب.
- كل mutation تبقى خارج presentation cutover بصلاحياتها وantiforgery.
- hosted acceptance والقطع والرجوع والبوابة الكاملة تبقى خضراء بلا إنشاء أو تغيير تسليم.
