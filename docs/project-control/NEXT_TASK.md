# المهمة التالية الوحيدة / Next Task

## `MOD-010` — Responsive layout and accessibility preferences

> الحالة: APPROVED — CONTINUOUS EXECUTION

### آخر شريحة منجزة: MOD-009

اكتملت إنتاجية النماذج والقوائم المشتركة: اختصار حفظ آمن، أخطاء قابلة للتركيز، saved filters allowlisted
وسياسة تمنع حفظ business drafts محلياً. نجح Angular build و36/36 tests، وبقيت 1081 backend checks ناجحة.

### الهدف التالي

إكمال تفضيلات التخطيط وإمكانية الوصول عبر Angular workspace:

- تفضيلات sidebar/layout responsive ومحدودة القيم؛
- density موحدة تؤثر فعلياً في الجداول والنماذج وCommand Center؛
- focus-visible وskip navigation وإدارة focus عند التنقل/الحوارات؛
- احترام reduced-motion وforced-colors والتكبير والشاشات الصغيرة؛
- reset مركزي واضح لكل تفضيلات العرض غير الحساسة؛
- فحوصات accessibility مركزة وRTL/LTR.

### حدود التنفيذ

- لا تخزين بيانات أعمال أو صلاحيات ضمن layout preferences.
- لا profile يمنح permission أو يغير business workflow.
- لا قاعدة بيانات أو endpoint جديد في هذه المرحلة.
- MVC يبقى عاملاً، وCSS في ملفات SCSS منفصلة فقط.

### بوابة القبول

- التفضيلات versioned/allowlisted/resettable وتتعافى من storage تالف.
- كل المسارات الأساسية قابلة للوحة المفاتيح مع focus مرئي ومنطقي.
- لا overflow غير مقصود عند 320px أو تكبير 200% في shell والصفحات المنقولة.
- reduced-motion/forced-colors/RTL/LTR تعمل دون فقد معنى أو إجراء.
- Angular/Web build والاختبارات والانحدار تبقى ناجحة.
- Angular/Web build والاختبارات والانحدار تبقى ناجحة.
- التعقيد: عالٍ.
