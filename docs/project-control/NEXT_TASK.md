# المهمة التالية الوحيدة / Next Task

## `MOD-030D` — دفعة Settings المصرح بها / Authorized Settings Cohort

> الحالة: APPROVED — BLOCKED ON AUTHORIZED OPERATOR SESSION

اكتمل `MOD-030C`: Customers وSuppliers وProducts وAnalyticsCenter تعمل افتراضيًا عبر Angular، ونجح hosted
acceptance 15/15 ثم pilot وMVC rollback على 12 مسارًا. خرائط GET لـSettings جاهزة، لكن Settings بقيت MVC.

### المتطلب السابق

- التقاط Playwright storage-state خارجي ومستثنى من Git لمستخدم يحمل `Administration.Access`.
- يجب أن ينجح شرط الصلاحية الصريح في المشغل؛ لا يُقبل skip ولا تعديل أدوار/صلاحيات كجزء من الاختبار.

### الهدف

- إثبات hosted production لكل deep links الخاصة بصفحات Settings المربوطة.
- تحويل Settings وحدها إلى AngularDefault بعد نجاح الدليل.
- تشغيل mixed pilot ثم process-local MVC rollback للأسطح الخمسة.
- إبقاء `OrganizationLogo` وكل mutations خارج تحويل GET، وإبقاء Razor/controller كرجوع.

### القبول

- جلسة `Administration.Access` تنجح دون تخطٍ.
- كل مسارات Settings المصرح بها تعيد التوجيه إلى أهداف Angular المطابقة.
- rollback يعيد الأسطح الخمسة إلى MVC 200.
- البوابة الكاملة تبقى خضراء ولا يحذف أي Razor/controller.
