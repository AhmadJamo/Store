# المهمة التالية الوحيدة / Next Task

## `MOD-030E5N` — الواجهة العامة والتسعير / Public Landing and Pricing Cutover

> الحالة: APPROVED

اكتمل `MOD-030E5M`: أصبح Subscription السطح 71 بعد hosted ‏15/15 والقطع/الرجوع 129/129 والبوابة
1,776 بلا إنشاء checkout أو تغيير اشتراك. بقي 11 سطحًا MVC.

### النطاق المحدود

- تحويل Public Index وPricing GET المجهولين إلى `/app/welcome` و`/app/pricing`.
- إبقاء التسجيل واختيار الخطة وكل عمليات SaaS خارج تحويل العرض.

### القبول

- GET العام يصل إلى Angular مع حفظ الاستعلام، والرجوع إلى MVC مثبت بلا تسجيل أو تغيير خطة أو اشتراك.
