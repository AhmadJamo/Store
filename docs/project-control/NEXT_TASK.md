# المهمة التالية الوحيدة / Next Task

## `MOD-011` — Data quality and recovery presentation

> الحالة: APPROVED — CONTINUOUS EXECUTION

### آخر شريحة منجزة: MOD-010

اكتملت تفضيلات theme/density/sidebar/content-width والتجاوب وmobile navigation وإدارة focus وforced-colors.
نجح البناء دون تحذيرات و39/39 Angular tests و1081 backend checks، ونجحت مراجعة بصرية عربية.

### الهدف التالي

إنشاء طبقة عرض وتعافٍ عامة لمشاكل جودة البيانات دون اختراع قواعد جديدة:

- تصنيف موحد لـvalidation وduplicate وconcurrency وnot-found وforbidden وtransient؛
- recovery actions واضحة ومسموح بها مثل reload/retry/open source؛
- correlation ID قابل للنسخ للدعم دون كشف stack trace؛
- conflict presentation يحافظ على إدخال المستخدم ويشرح الخطوة التالية؛
- إثبات reusable contract على Customers وSuppliers؛
- عدم عرض زر أو علاج لا تسمح به الصلاحيات الحالية.

### حدود التنفيذ

- backend Problem Details وقواعد Application هي المرجع الوحيد.
- لا duplicate matching أو merge تلقائي من المتصفح.
- لا retry تلقائي لعمليات mutation ولا تجاوز rowversion.
- لا قاعدة بيانات أو endpoint عام جديد دون حاجة مثبتة.
- MVC يبقى عاملاً، وCSS في ملفات SCSS منفصلة فقط.

### بوابة القبول

- الأخطاء المعروفة تُصنف حتمياً وتعرض رسالة وعلاجاً مناسبين.
- validation/concurrency لا يفقدان مدخلات النموذج.
- retry لا يظهر لخطأ دائم، والإجراءات permission-aware.
- correlation ID يظهر دون تفاصيل داخلية، وRTL/LTR/keyboard تعمل.
- Angular/Web build والاختبارات والانحدار تبقى ناجحة.
- التعقيد: عالٍ.
