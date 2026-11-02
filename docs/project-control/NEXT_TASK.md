# المهمة التالية الوحيدة / Next Task

## `MOD-030E4V` — المشتريات المباشرة التاريخية ومرتجعاتها / Legacy Purchases and Returns Cutover

> الحالة: APPROVED

اكتمل `MOD-030E4U`: أصبحت SupplierPortal السطح 52 بعد hosted ‏15/15 والقطع والرجوع 102/102 وفحص ترويسات الخصوصية والبوابة 1,637. لم يُنشأ رابط بوابة أو submission، وبقي 30 سطحًا MVC.

### النطاق المحدود

- Purchases Index وDetails ذوا المعرّف الموجب إلى `/purchases/legacy` و`/purchases/legacy/{id}`.
- PurchaseReturns Index وDetails ذوا المعرّف الموجب وCreate ذو `purchaseId` الموجب إلى مسارات Angular التاريخية الدقيقة.
- إنشاء الشراء المباشر القديم يبقى متقاعدًا، وPurchase Post وPurchaseReturn Create POST يبقيان خارج التحويل.

### القبول

- كل GET مصرح يصل إلى مسار Angular الدقيق ويحافظ على المعرّفات والاستعلام.
- المعرّفات غير الموجبة، إنشاء شراء قديم وكل mutation تبقى خارج presentation cutover.
- hosted acceptance والقطع والرجوع والبوابة الكاملة تبقى خضراء بلا إنشاء مستندات شراء أو مخزون أو محاسبة.
