# المهمة التالية الوحيدة / Next Task

## `MOD-030E4U` — بوابة المورد العامة / Supplier Portal Cutover

> الحالة: APPROVED

اكتمل `MOD-030E4T`: أصبحت SupplierIntegrations السطح 51 بعد hosted ‏15/15 والقطع والرجوع 102/102 والبوابة 1,632. لم توجد Submission محفوظة لاختبار التفاصيل حياً. بقي 31 سطحًا MVC.

### النطاق المحدود

- SupplierPortal Index GET ذو token آمن إلى `/supplier-portal/{token}`.
- الحفاظ على AllowAnonymous وrate limiting وno-store/no-referrer/noindex وسجل الرفض الأمني.
- Quotation/ConfirmOrder/DeliveryNotice POST تبقى خارج التحويل.

### القبول

- كل GET مصرح يصل إلى مسار Angular الدقيق ويحافظ على query.
- كل mutation وأي action غير Index/Create/Details GET يبقى خارج presentation cutover.
- hosted acceptance والقطع والرجوع والبوابة الكاملة تبقى خضراء بلا تغيير قاعدة بيانات.
