# المهمة التالية الوحيدة / Next Task

## `MOD-030C` — دفعة Analytics وSettings المصرح بها / Authorized Analytics and Settings Cohort

> الحالة: APPROVED — NEXT INCREMENT ضمن التحويل المرحلي القابل للرجوع

اكتمل `MOD-030B`: Customers وSuppliers وProducts تعمل افتراضيًا عبر Angular، ونجح hosted acceptance 15/15،
ثم نجح mixed pilot وMVC rollback على 12 مسارًا مصرحًا دون حذف MVC.

### المتطلب السابق

- توفير Playwright storage-state خارجي ومستثنى من Git لمستخدم يحمل `Administration.Access`، مع صلاحيات Analytics
  اللازمة. لا تُقرأ أو تُطبع بيانات الاعتماد أو محتويات الجلسة.

### الهدف

- إثبات hosted production لـAnalytics Center وSettings، بما في ذلك deep links والقراءات المصرح بها.
- توسيع allowlist فقط لمسارات GET الموجودة والمثبتة بنظير Angular.
- تشغيل mixed pilot للدفعتين ثم process-local MVC rollback قبل تثبيت AngularDefault.
- إبقاء جميع mutations خلف API/antiforgery وإبقاء Razor/controller كمسار رجوع.

### القبول

- جلسة `Administration.Access` تنجح في hosted Angular دون تخطٍ لSettings.
- AnalyticsCenter وSettings يعيدان التوجيه إلى أهداف Angular المطابقة في pilot.
- Customers/Suppliers/Products تبقى AngularDefault، والمسارات غير المحولة تبقى MVC.
- rollback يعيد الأسطح الخمسة إلى MVC 200، والبوابة الكاملة تبقى خضراء.
