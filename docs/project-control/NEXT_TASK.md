# المهمة التالية الوحيدة / Next Task

## `MOD-030A` — استضافة Angular وبوابة التحويل النهائي / Angular Hosting and Final Cutover Gate

> الحالة: APPROVED — NEXT INCREMENT ضمن موجة القبول الشامل

اكتمل `MOD-029D`: نجح AngularDefault ثم MVC rollback على 10 مسارات GET مصرح بها. كشف التمرين ترحيل
Analytics delegation المعلق؛ طُبق الترحيل 136 الإضافي وأصبحت قاعدة Demo مطابقة بلا pending migrations.

### الهدف

- استضافة build Angular الإنتاجي فعليًا تحت `/app` مع SPA fallback آمن لا يبتلع API/MVC.
- توسيع cutover catalog من allowlist التجريبية إلى route families المثبتة تدريجيًا، مع بقاء rollback.
- تشغيل قبول مصرح به على Angular المستضاف لا dev server، ثم تحويل الدفعة الأولى فقط.
- عدم حذف MVC حتى يثبت كل surface تحت الاستضافة الإنتاجية ويُسجل rollback لكل دفعة.

### القبول

- `/app` وdeep links تعيد Angular production index/assets بنجاح، بينما `/api` وMVC تبقيان منفصلتين.
- الدفعة الأولى تتحول بـGET allowlist فقط، وتحافظ على query/id والصلاحيات، ويظل rollback قابلًا للتشغيل.
- البوابة الكاملة وMOD-029B..D تنجح على الخادم المستضاف.
- لا حذف Razor/controller قبل إغلاق سجل كل surface وإثبات مسار الرجوع.
