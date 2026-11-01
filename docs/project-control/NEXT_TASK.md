# المهمة التالية الوحيدة / Next Task

## `MOD-030E3F` — قوالب المنتجات والوصفات / Product Templates and Recipes Cutover

> الحالة: APPROVED

اكتمل `MOD-030E3E`: أصبحت Users وRoles السطحين 28 و29 في AngularDefault بعد hosted ‏15/15 والقطع والرجوع 62/62.
بقي 53 سطحًا mapped يعمل MVC افتراضيًا.

### النطاق المحدود

- ProductTemplates Index GET إلى `/products/templates`.
- Recipes Index GET إلى `/products/recipes` وEdit GET ذو `productId` الموجب إلى `/products/recipes/{id}`.
- Assign/Create/Preview/Generate وقواعد حفظ الوصفة تبقى مستقلة بصلاحياتها وantiforgery وخارج كتالوج العرض.

### القبول

- كل GET مصرح يصل إلى مسار Angular الدقيق ويحافظ على query والمعرّف الموجب.
- كل mutation أو معرّف غير موجب يبقى خارج presentation cutover.
- hosted acceptance والقطع والرجوع والبوابة الكاملة تبقى خضراء بلا تغيير قاعدة بيانات.
