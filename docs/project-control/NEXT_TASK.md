# المهمة التالية الوحيدة / Next Task

## `MOD-030E5M` — اشتراك الشركة / Tenant Subscription Cutover

> الحالة: APPROVED

اكتمل `MOD-030E5L`: أصبحت SupplierPayments السطح 70 بعد hosted ‏15/15 والقطع/الرجوع 128/128 والبوابة
1,773 بلا إنشاء دفعة أو تغيير قيد. بقي 12 سطحًا MVC.

### النطاق المحدود

- تحويل Subscription Index GET الآمن إلى `/app/subscription`.
- إبقاء checkout POST والتسعير والعروض والتفعيل تحت خدمات SaaS الحالية وخارج تحويل العرض.

### القبول

- GET المصرح يصل إلى Angular مع حفظ الاستعلام، والرجوع إلى MVC مثبت بلا إنشاء checkout أو تغيير اشتراك.
