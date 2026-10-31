# المهمة التالية الوحيدة / Next Task

## `MOD-030E2G` — تخطيط إعادة التزويد / Replenishment Cutover

> الحالة: APPROVED

اكتمل `MOD-030E2F`: أصبحت أربعة أسطح Warehouse Operations ‏AngularDefault في دفعة واحدة، مع بقاء deep links غير
المدعومة وعمليات الكتابة على MVC/API. نجح hosted ‏15/15 والقطع والرجوع على 48 مسارًا. أصبحت 19 سطحًا مفعلة،
ويبقى 63 سطحًا mapped يعمل MVC افتراضيًا.

### المتطلب السابق

- Replenishment يملك Angular workspace وعقد API يقرأ نفس Application projection المستخدم في MVC.
- التحويل يقتصر على Index GET؛ save/activate/create-transfer/create-purchase-demand تبقى mutations مستقلة ومحمية.

### الهدف بعد الموافقة

- إضافة خريطة GET الدقيقة لـReplenishment Index وتفعيل السطح AngularDefault.
- توسيع مشغل القطع بصلاحية `Inventory.Replenishment.View` وإثبات query preservation.
- تشغيل hosted acceptance والقطع والـprocess-local MVC rollback.
- إبقاء Controllers وRazor كاملة كرجوع.

### القبول

- Index المصرح يصل إلى `/purchases/replenishment` ويحافظ على query.
- rollback يعيد المسار إلى MVC دون تنفيذ أي mutation.
- البوابة الكاملة تبقى خضراء دون حذف ملفات أو تغيير قاعدة البيانات.
