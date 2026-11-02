# المهمة التالية الوحيدة / Next Task

## `MOD-030E5A` — فواتير العملاء / Customer Invoices Cutover

> الحالة: APPROVED

اكتمل `MOD-030E4Z`: أصبحت SalesDeliveries السطح 58 بعد hosted ‏15/15 والقطع/الرجوع 104/104 والبوابة 1,680. لم تتغير بيانات تسليم أو مخزون أو محاسبة. بقي 24 سطحًا MVC.

### النطاق المحدود

- CustomerInvoices Index وDetails وCreate ذو salesOrderId موجب إلى مسارات Angular الدقيقة.
- الحفاظ على status/search في القائمة.
- Create/Approve/Cancel/Post/Reverse POST تبقى خارج التحويل وتعمل عبر Angular API المحمي.

### القبول

- كل GET مصرح يصل إلى مسار Angular الدقيق ويحافظ على query والمعرّف الموجب.
- كل mutation تبقى خارج presentation cutover بصلاحياتها وantiforgery.
- hosted acceptance والقطع والرجوع والبوابة الكاملة تبقى خضراء بلا إنشاء أو تغيير فاتورة/قيد.
