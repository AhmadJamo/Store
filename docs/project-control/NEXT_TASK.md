# المهمة التالية الوحيدة / Next Task

## `MOD-020B` — تحرير هوية المنشأة / Organization identity editing

> الحالة: APPROVED — NEXT INCREMENT ضمن موجة نقل الإعدادات

### آخر شريحة مثبتة

اكتملت `MOD-020A`: تفضيلات التشغيل أصبحت قابلة للتعديل من Angular عبر الخدمة الحالية مع rowversion والتدقيق
ومسح cache اللغة. نجحت 1113 فحوص backend و64/64 Angular، وبقي Settings على MVC افتراضياً.

### الهدف التالي

- نقل حقول هوية المنشأة النصية ثنائية اللغة إلى نموذج Angular عبر DTO والخدمة الحاليين.
- الحفاظ على rowversion والتدقيق الدلالي والتحقق الخادمي وعزل الشركة.
- دعم loading/validation/concurrency/success وdirty-state مع ترجمة عربية كاملة.
- إبقاء رفع/حذف الشعار والحقول المخصصة على MVC إذا لم يمكن إغلاقها بأمان داخل نفس الشريحة الصغيرة.

### حدود التنفيذ

- لا EF/entity مباشرة، ولا إعادة تنفيذ قواعد الهوية في المتصفح.
- لا كشف قيم الهوية الحساسة في سجل التدقيق أو logs.
- لا تفعيل AngularDefault ولا إزالة MVC.
- لا توسيع الشريحة إلى Document Designs أو بقية صفحات Settings.

### بوابة القبول

- API محمي بـ`Administration.Access` وantiforgery ويعيد DTO محدثاً/تعارضاً واضحاً.
- النموذج يحافظ على المدخلات عند الخطأ ويعتمد rowversion الجديد بعد النجاح.
- Release وSecurityRegression وAngular tests/build والوصولية والميزانية تنجح.
- تحديث registry والتوثيق وworklog.
