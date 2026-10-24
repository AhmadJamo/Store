# المهمة التالية الوحيدة / Next Task

## `MOD-002` — Versioned internal API foundation

> الحالة: PROPOSED — AWAITING EXPLICIT APPROVAL

### آخر شريحة منجزة: MOD-001

أُنشئ فرع `codex/angular-modernization-foundation` ومشروع Angular 22 مستقل في `MiniStore Front-End` مع
standalone components وstrict TypeScript وlazy routing وSCSS design tokens وواجهة responsive عربية RTL وإنجليزية LTR.
أُضيف عقد حالة Workspace صغير قائم على Signals وdevelopment proxy محدود إلى `/api` و`/Account`.

نجح Angular production build بحزمة أولية 229.81 kB raw، ونجح 1/1 frontend test، كما نجح backend Release build
دون warnings/errors ونجح 1066 فحص انحدار. لم يتغير MVC أوdatabase schema أوbusiness rules.

### الهدف المقترح التالي

إضافة أساس API داخلي versioned داخل `MiniStore.Web` لإعادة استخدام المصادقة والصلاحيات وعزل الشركات الحالي:

- `/api/v1/session` يعرض هوية المستخدم والشركة واللغة والصلاحيات اللازمة للواجهة دون بيانات حساسة.
- CSRF contract آمن لطلبات Angular mutation مع cookies same-origin.
- `ProblemDetails` موحد مع validation errors وcorrelation ID دون كشف الاستثناءات.
- OpenAPI للـinternal API فقط مع عقود DTO صريحة.
- conventions للpagination/sorting/filtering وcancellation.
- integration/regression tests للمصادقة و401/403 وtenant isolation وantiforgery.

### حدود التنفيذ

- لا Customer API أونقل شاشة قبل قبول foundation.
- لا تغيير لقواعد Identity أوpermissions أوtenant middleware.
- لا database migration.
- لا JWT أوtoken في localStorage.
- لا إزالة أوتعديل لمسارات MVC الحالية.

### بوابة القبول

- frontend يستطيع قراءة session contract عبر dev proxy.
- mutation probe محمي من CSRF ويُختبر دون تغيير business data.
- كل endpoint يفرض backend authorization.
- OpenAPI وProblemDetails موثقان ومختبران.
- backend وfrontend builds/tests تظل ناجحة.
