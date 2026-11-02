# المهمة التالية الوحيدة / Next Task

## `MOD-030E4R` — بيانات شراء المورد / Supplier Purchasing Cutover

> الحالة: APPROVED

اكتمل `MOD-030E4Q`: أصبحت PurchaseAgreements السطح 48 بعد hosted ‏15/15 والقطع والرجوع 99/99 والبوابة 1,610. اختُبر Create بحدث توريد مؤهل، ولم توجد اتفاقية محفوظة لاختبار Details حياً. بقي 34 سطحًا MVC.

### النطاق المحدود

- SupplierPurchasing Index GET إلى `/purchases/supplier-terms` مع supplierId الاختياري.
- PriceExceptions GET إلى `/purchases/supplier-terms/exceptions` مع المرشحات.
- PriceBreaks/PriceHistory GET للمعرّفات الموجبة إلى مسارات Angular الدقيقة.
- Export/preview/import/save/status actions تبقى خارج التحويل.

### القبول

- كل GET مصرح يصل إلى مسار Angular الدقيق ويحافظ على query.
- كل mutation وأي action غير Index/Create/Details GET يبقى خارج presentation cutover.
- hosted acceptance والقطع والرجوع والبوابة الكاملة تبقى خضراء بلا تغيير قاعدة بيانات.
