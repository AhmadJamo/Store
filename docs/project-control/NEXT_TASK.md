# المهمة التالية الوحيدة / Next Task

## `MOD-030E4X` — عروض أسعار المبيعات / Sales Quotations Cutover

> الحالة: APPROVED

اكتمل `MOD-030E4W`: أصبح SalesPriceLists السطح 55 بعد hosted ‏15/15 ومجموعة القطع/الرجوع المصرح بها 104/104 والبوابة 1,654. لم تحمل الجلسة SalesPricing.View للمسار الحي ولم تتغير بيانات التسعير. بقي 27 سطحًا MVC.

### النطاق المحدود

- SalesQuotations Index وCreate وDetails ذوا المعرّف الموجب إلى `/sales/quotations` و`/sales/quotations/new` و`/sales/quotations/{id}`.
- الحفاظ على status/search وtemplateId في query.
- DownloadPdf وكل Create/Send/Accept/Reject/Expire/Cancel POST تبقى خارج التحويل.

### القبول

- كل GET مصرح يصل إلى مسار Angular الدقيق ويحافظ على query والمعرّف الموجب.
- PDF وكل mutation تبقى خارج presentation cutover بصلاحياتها وantiforgery.
- hosted acceptance والقطع والرجوع والبوابة الكاملة تبقى خضراء بلا إنشاء أو تغيير عرض سعر.
