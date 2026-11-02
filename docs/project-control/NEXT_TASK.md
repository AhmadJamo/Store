# المهمة التالية الوحيدة / Next Task

## `MOD-030E4T` — تكاملات الموردين / Supplier Integrations Cutover

> الحالة: APPROVED

اكتمل `MOD-030E4S`: أصبحت SupplierCommunications السطح 50 بعد hosted ‏15/15 والقطع والرجوع 101/101 والبوابة 1,626. لم يوجد اتصال محفوظ لاختبار Details حياً. بقي 32 سطحًا MVC.

### النطاق المحدود

- SupplierIntegrations Index GET إلى `/purchases/integrations`.
- Submission GET ذو المعرّف الموجب إلى `/purchases/integrations/submissions/{id}`.
- UBL وDownloadSubmission GET يبقيان تنزيل ملف مباشر خارج presentation cutover.
- credential/portal/outbox/retry POST actions تبقى خارج التحويل.

### القبول

- كل GET مصرح يصل إلى مسار Angular الدقيق ويحافظ على query.
- كل mutation وأي action غير Index/Create/Details GET يبقى خارج presentation cutover.
- hosted acceptance والقطع والرجوع والبوابة الكاملة تبقى خضراء بلا تغيير قاعدة بيانات.
