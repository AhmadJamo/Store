# المهمة التالية الوحيدة / Next Task

## `MOD-030E3C` — قائمة المستودعات / Warehouses Index Cutover

> الحالة: APPROVED

اكتمل `MOD-030E3B`: أصبحت ProductCategories وProductAttributes السطحين 24 و25 في AngularDefault بعد hosted ‏15/15
والقطع والرجوع 55/55. بقي 57 سطحًا mapped يعمل MVC افتراضيًا.

### النطاق المحدود

- Warehouses فقط.
- يضاف Index GET إلى `/warehouses` مع حفظ query.
- Create/Edit GET تبقى على MVC لأن مساحة Angular الحالية تدير النموذج داخل الصفحة ولا تستهلك URL intent مستقلًا.
- Create/Edit/Delete POST تبقى أوامر MVC/API مستقلة بصلاحياتها وantiforgery ولا تدخل كتالوج العرض.

### الهدف

- تدقيق أن مساحة Angular تعرض المستودعات بالأسماء والخيارات الحالية وتحترم صلاحيات View/Create/Edit/Delete.
- تحويل Index فقط وتوسيع مشغل cutover المصرح.
- إثبات AngularDefault وprocess-local MVC rollback مع إبقاء Controller وRazor كاملين.

### القبول

- Warehouses Index المصرح يصل إلى `/warehouses` ويحافظ على query.
- Create/Edit deep links وكل write وغير GET/HEAD تبقى خارج presentation cutover.
- hosted acceptance والقطع والرجوع والبوابة الكاملة تبقى خضراء بلا تغيير قاعدة بيانات.
