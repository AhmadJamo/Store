# المهمة التالية الوحيدة / Next Task

## `MOD-030E5X` — تصنيف حد اللغة / Language Mutation Boundary Classification

> الحالة: APPROVED

اكتمل `MOD-030E5W`: أصبح Home السطح 81 بعد hosted ‏15/15 والقطع/الرجوع 140/140 والبوابة 1,840 دون
تغيير حالة أو بيانات. لم يبق إلا Language Set، وهو POST mutation بلا GET أو Razor view.

### النطاق المحدود

- تصنيف Language Set POST كحد MVC compatibility دائم خارج presentation cutover.
- جعل سجل الهجرة والمصفوفة يميزان mutation-only exception عن سطح عرض MVC متبقٍ.
- إثبات بقاء الأمر POST + antiforgery وأنه لا يدخل كتالوج GET/HEAD أو AngularDefault.

### القبول

- لا يبقى أي سطح عرض GET على MVC افتراضيًا.
- يظهر Language بوضوح كاستثناء mutation-only لا كفجوة عرض.
- فحوص الحماية والمولّد والبناء وبوابة Angular خضراء.

### خارج النطاق

- تغيير cookie الثقافة أو التطبيع أو returnUrl المحلي.
- تحويل POST إلى redirect عرض أو نقله إلى المتصفح.
- حذف Controller أو antiforgery أو أي مسار rollback.
