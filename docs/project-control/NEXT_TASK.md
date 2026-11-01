# المهمة التالية الوحيدة / Next Task

## `MOD-030E4N` — عكس مستندات الشراء / Purchase Document Reversals Cutover

> الحالة: APPROVED

اكتمل `MOD-030E4M`: أصبحت SupplierCreditNotes السطح 44 بعد hosted ‏15/15 والقطع والرجوع 91/91. لم يوجد إشعار محفوظ لاختبار Details حياً. بقي 38 سطحًا MVC.

### النطاق المحدود

- PurchaseDocumentReversals Index GET إلى `/purchases/reversals`.
- Details GET ذو المعرّف الموجب إلى `/purchases/reversals/{id}`.
- Create GET المركب وCreate POST يبقيان خارج الدفعة.

### القبول

- كل GET مصرح يصل إلى مسار Angular الدقيق ويحافظ على query.
- كل mutation وأي action غير Index/Create/Details GET يبقى خارج presentation cutover.
- hosted acceptance والقطع والرجوع والبوابة الكاملة تبقى خضراء بلا تغيير قاعدة بيانات.
