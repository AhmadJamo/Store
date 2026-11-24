# 13 — المشاكل والفجوات

> الحالة: Active issue register
> آخر تحديث: 2026-11-24
> المبدأ: المشكلة لا تغلق برسالة Success؛ يلزم دليل مناسب لنوعها

| ID | المكان | السبب المثبت/المرجح | التأثير | الخطورة | طريقة الإصلاح | الحالة | دليل التحقق |
|---|---|---|---|---|---|---|---|
| FRD-ISS-001 | Native visual QA | computer-use runtime تعذر، لكن بوابة Playwright الرسمية تعمل | لا تأثير متبقٍ على القبول البصري | Medium | استخدام AngularBrowserAcceptance مع مراجعة الصور | Verified | Playwright 6/6 + screenshots 2026-11-24 |
| FRD-ISS-002 | Angular initial shared chunk | chunk بحجم 204.88 kB يتجاوز warning 180 kB | أداء التحميل الأولي قد يتدهور مع التوسع | Medium | تحليل imports/code splitting قبل rollout | Open | production build 2026-11-24 |
| FRD-ISS-003 | `app-shell.scss` | pilot shell styles بحجم 8.43 kB تتجاوز warning 6 kB | بقي 1.57 kB قبل error budget 10 kB | Medium | استخراج mobile/navigation primitives وحذف legacy selectors | Open | production build 2026-11-24 |
| FRD-ISS-004 | Shared UI adoption | FilterBar/Summary/Form/Detail/Workflow adoption أقل بكثير من PageHeader/Feedback | تفاوت كبير بين 121 صفحة | High | pilot ثم migration بالموجات؛ يمنع bulk restyle | In Progress | UI consistency inventory |
| FRD-ISS-005 | Navigation registry | registry لا يحمل App/Archetype/Mobile priority كعقد صريح بعد | shell قد يستنتج السياق بطريقة هشة | High | توسيع metadata مع generated coverage tests | Planned | `06-NAVIGATION-AND-ROUTES.md` |
| FRD-ISS-006 | Visual identity | بعض feature SCSS يستخدم namespaces وألوان fallback قديمة | dark/RTL/theme consistency غير مضمونة | Medium | token inventory وإزالة القيم المباشرة تدريجياً | Open | repository scan + design contract |
| FRD-ISS-007 | Odoo 20 research | صفحات 20.0 أعادت Timeout أثناء البحث | لا توجد مقارنة موثقة مع 20.0 | Low | إعادة التحقق لاحقاً؛ Odoo 19 هو المرجع الحالي | Documented | `02-ODOO-RESEARCH.md` |
| FRD-ISS-008 | Chatter/Activities/shared views/drag-drop/offline POS | لا توجد عقود عامة مثبتة لكل capability | تنفيذ بصري سيخلق وظائف وهمية أو غير آمنة | High | إبقاؤها Conditional/Excluded حتى API + permission + tests | Controlled | `03-ODOO-UI-FEATURE-MATRIX.md` |
| FRD-ISS-009 | Route coverage | 7 gaps مصنفة P2 في coverage الحالي | بعض المسارات تحتاج تعميق evidence قبل migration | Medium | إغلاق كل gap عند وصول موجته، دون تعطيل P0 pilot | Open | route/function coverage report |
| FRD-ISS-010 | Solution entry files | `MiniStore.sln` فارغ و`MiniStore.slnx` هو الحل الفعلي | أدوات/مطورون قد يشغلون ملفاً خاطئاً | Low | توثيق entry الصحيح وعدم حذف أي ملف بلا قرار | Documented | `01-CURRENT-SYSTEM-AUDIT.md` |
| FRD-ISS-011 | Generated concept images | mockups ليست متصلة بالـAPI وقد تحتوي قيماً تجريبية | خطر معاملتها كمتطلبات نهائية أو دليل تنفيذ | Medium | فهرس usage constraints ومقارنة كل عنصر بالmatrix | Controlled | `mockups/README.md` |

## تعريف الحالات

- Open: مثبتة ولم يبدأ إصلاحها.
- Planned: لها شريحة محددة لم تبدأ.
- In Progress: إصلاح محدود جارٍ مع دليل جزئي.
- Controlled: الخطر محكوم بقرار يمنع التنفيذ الخاطئ.
- Documented: قيد معروف لا يمنع المسار الحالي.
- Verified: أصلحت واختبرت بدليل مسجل؛ لا تستخدم بلا evidence.

## أولويات الإصلاح الحالية

1. FRD-ISS-005 ضمن Shell/Products pilot.
2. FRD-ISS-004 وFRD-ISS-006 عبر primitives حقيقية، لا إعادة كتابة شاملة.
3. FRD-ISS-001 مغلقة؛ استخدم نفس Playwright gate لكل وحدة.
4. FRD-ISS-002 وFRD-ISS-003 قبل أن تتجاوز error budgets.
5. FRD-ISS-009 حسب موجات الوحدة وبحماية route coverage.
