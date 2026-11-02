# المهمة التالية الوحيدة / Next Task

## `MOD-030E5B` — تحصيلات العملاء / Customer Payments Cutover

> الحالة: APPROVED

اكتمل `MOD-030E5A`: أصبحت CustomerInvoices السطح 59 بعد hosted ‏15/15 والقطع/الرجوع 104/104 والبوابة 1,689. لم تتغير فاتورة أو ذمم أو قيود. بقي 23 سطحًا MVC.

### النطاق المحدود

- CustomerPayments Index وDetails وCreate ذو customerInvoiceId موجب إلى مسارات Angular الدقيقة.
- الحفاظ على search في القائمة.
- Create/Reverse POST تبقى خارج التحويل وتعمل عبر Angular API المحمي.

### القبول

- كل GET مصرح يصل إلى مسار Angular الدقيق ويحافظ على query والمعرّف الموجب.
- كل mutation تبقى خارج presentation cutover بصلاحياتها وantiforgery.
- hosted acceptance والقطع والرجوع والبوابة الكاملة تبقى خضراء بلا إنشاء أو عكس تحصيل/قيد.
