# المهمة التالية الوحيدة / Next Task

## `MOD-030E5W` — مسار Home التوافقي / Home Compatibility Redirect Cutover

> الحالة: APPROVED

اكتمل `MOD-030E5V`: أصبح DocumentDesigns السطح 80 بعد hosted ‏15/15 والقطع/الرجوع 139/139 والبوابة
1,836 دون إنشاء أو تعديل تصميم. بقي سطحان MVC.

### النطاق المحدود

- تحويل Home Index GET التوافقي إلى جذر `/app` مع الحفاظ على query string.
- إبقاء قرار الإعداد الموجّه مقابل Dashboard داخل الخدمة/المتحكم الحالي كمسار rollback، دون تغيير بيانات أو صلاحيات.

### القبول

- مسار Home المصرّح يعيد التوجيه إلى هدف Angular الدقيق، والـ override المحلي يعيده إلى سلوك MVC.
- فحوص الحماية تثبت التطابق وتمنع أي توسيع إلى أوامر غير GET.
- hosted وcutover وrollback والبناء والبوابة النهائية خضراء.

### خارج النطاق

- تغيير منطق اكتمال الإعداد الموجّه أو Dashboard.
- تحويل أمر تغيير اللغة POST إلى presentation cutover.
- حذف أي Controller أو Razor view أو مسار rollback.
