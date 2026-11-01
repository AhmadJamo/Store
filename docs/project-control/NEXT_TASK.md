# المهمة التالية الوحيدة / Next Task

## `MOD-030E2G` — تخطيط إعادة التزويد / Inventory Replenishment Parity and Cutover

> الحالة: APPROVED

اكتمل `MOD-030E2F`: أصبحت أربعة أسطح Warehouse Operations ‏AngularDefault في دفعة واحدة، مع بقاء deep links غير
المدعومة وعمليات الكتابة على MVC/API. نجح hosted ‏15/15 والقطع والرجوع على 48 مسارًا. أصبحت 19 سطحًا مفعلة،
ويبقى 63 سطحًا mapped يعمل MVC افتراضيًا. أُغلقت فجوة Inventory Replenishment الوظيفية بالمقابل الصحيح.

### المتطلب السابق

- كشف التدقيق أن `/purchases/replenishment` يخص أتمتة المشتريات وصلاحيات `PurchaseReplenishment.*`، وليس شاشة
  المخزون المبنية على `ReplenishmentService` وصلاحيات `Inventory.Replenishment.*`.
- بُني API وAngular workspace الصحيحان على `/inventory/replenishment`، ونجحت اختبارات الوظيفة والصلاحيات.

### الهدف بعد الموافقة

- إضافة خريطة GET الدقيقة لـReplenishment Index إلى `/inventory/replenishment` وتفعيل السطح AngularDefault بعد إثبات parity.
- توسيع مشغل القطع بصلاحية `Inventory.Replenishment.View` وإثبات query preservation.
- تشغيل hosted acceptance والقطع والـprocess-local MVC rollback.
- إبقاء Controllers وRazor كاملة كرجوع.

### القبول

- اختبارات API وAngular تثبت القراءة والأوامر والصلاحيات وحالات الخطأ دون نقل قواعد العمل إلى الواجهة.
- Index المصرح يصل إلى `/inventory/replenishment` ويحافظ على query.
- rollback يعيد المسار إلى MVC دون تنفيذ أي mutation.
- البوابة الكاملة تبقى خضراء دون حذف ملفات أو تغيير قاعدة البيانات.
