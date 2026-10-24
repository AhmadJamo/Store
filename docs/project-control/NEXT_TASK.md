# المهمة التالية الوحيدة / Next Task

## `MOD-004` — Reusable workspace engine

> الحالة: PROPOSED — AWAITING EXPLICIT APPROVAL

### آخر شريحة منجزة: MOD-003

أُنشئ نظام تصميم semantic يدعم system/light/dark وcomfortable/compact وRTL/LTR، مع حفظ تفضيلات العرض غير
الحساسة وreset وحالات وصولية وصفحة معاينة ثنائية اللغة. نجح Angular build و6/6 tests، ونجح 1072 backend checks،
وبقي production npm audit دون ثغرات. تمت مراجعة الواجهة العربية RTL بصريًا ومن خلال accessibility tree.

### الهدف المقترح التالي

إنشاء محرك Workspace مركزي قابل لإعادة الاستخدام لكل الوحدات:

- navigation registry مبني على metadata ومحكوم بصلاحيات session؛
- تبويبات قابلة للفتح والتنشيط والإغلاق ومنع التكرار؛
- tab identity مستقرة تفصل route عن query/context؛
- تاريخ Back/Forward مستقل لكل تبويب؛
- close، close others، reopen recently closed وحدود قصوى قابلة للتهيئة؛
- حفظ آمن لحالة التنقل غير الحساسة واستعادتها بعد refresh؛
- dirty-state guard يمنع فقدان إدخال غير محفوظ؛
- keyboard navigation وARIA للتبويبات والقوائم؛
- اختبارات unit للحالة والاستعادة والتكرار والحماية.

### حدود التنفيذ

- لا Customer API أو شاشة زبائن بعد.
- لا حفظ payloads أو permissions أو أسرار في browser storage.
- لا منح صلاحية من الواجهة؛ backend يبقى المرجع النهائي.
- لا قاعدة بيانات أو migration.
- لا تغيير MVC أو business rules.

### بوابة القبول

- فتح المسار نفسه لا ينشئ تبويبًا مكررًا دون اختلاف context مقصود.
- refresh يعيد التبويبات الآمنة والمسار النشط فقط.
- dirty tab لا يغلق أو ينتقل دون قرار مستخدم واضح.
- sidebar يخفي غير المسموح بصريًا مع بقاء backend authorization إلزاميًا.
- Angular build/tests وbackend regression تبقى ناجحة.
