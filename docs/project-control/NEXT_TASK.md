# المهمة التالية الوحيدة / Next Task

## FRD-F — Products List/Form/Record Pilot

> الحالة: IN PROGRESS — بدأ بعد إغلاق FRD-E في 2026-11-25
>
> الهدف التالي هو أول تحويل كامل لنمط صفحات فعلي، وليس تجميلاً عاماً: Products List ثم Create/Edit ثم Record/Details، مع الحفاظ على CRUD والبحث بالاسم والباركود والصلاحيات وURLs وAPI الحالية.

ابدأ بجرد ملفات Products الفعلية ومساراتها واختباراتها وعقود API، ثم طابقها مع `05-DESIGN-SYSTEM.md` و`07-MODULE-MIGRATION-MATRIX.md`. بدأ List بتجميع وجهات الكتالوج وإزالة النص غير المترجم. التالي: تحسين control bar وحالات filtered-empty وكثافة الجدول باستعمال PageHeader/FilterBar/Summary/DataTable/Feedback الحالية، ثم توثيق parity قبل Form/Record. لا تغيّر قواعد المنتج أو DTOs أو API ما لم يكشف الجرد ضرورة موثقة. أثبت EN/AR وRTL/LTR وdesktop/mobile والبحث بالاسم والباركود والصلاحيات والحالات loading/empty/error قبل الانتقال إلى Form/Record.

## FRD-E — إعادة بناء Application Shell

> الحالة: COMPLETE — أُغلقت في 2026-11-25
>
> اكتملت App metadata وApp Switcher وcurrent-app navigation واستخراج CSS وSection/Archetype/Mobile priority وcontext hierarchy وقائمة account/branch الموحدة. بقيت URLs وWorkspace/dirty/permission authority كما هي، ونجحت البوابة النهائية بـ270 اختباراً وPlaywright 7/7.

## FRD-D — نظام التصميم الوظيفي

> الحالة: COMPLETE — أُغلق في 2026-11-24

ثُبت عقد `05-DESIGN-SYSTEM.md` ومعاينة List/Record والحالات. نجح 260 اختباراً والبناء وفحوص 138 قالباً و121 صفحة، وPlaywright 6/6 للاتجاه والثيم والكثافة والكيبورد وDesktop/390px. الأدلة في `docs/frontend-redesign/acceptance/design-system-2026-11-24/`.
## FRD-C — معمارية UX والتنقل

> الحالة: COMPLETE — أُغلق في 2026-11-22

وُثقت Information Architecture وأنماط الصفحات والتنقل والصلاحيات والاستثناءات في `04-UX-ARCHITECTURE.md` و`06-NAVIGATION-AND-ROUTES.md`، واعتمد القرار في ADR `2026-11-22-odoo-inspired-frontend-ux-architecture.md`.
## FRD-B — Odoo Research والتفصيل المقارن

> الحالة: COMPLETE — أُغلق في 2026-11-22

وُثقت المراجع الرسمية والقرارات المقارنة في `docs/frontend-redesign/02-ODOO-RESEARCH.md` و`docs/frontend-redesign/03-ODOO-UI-FEATURE-MATRIX.md`. Odoo 19.0 هو المرجع المتحقق؛ لا يُدعى توثيق 20.0 بسبب Timeout أثناء البحث.
## UX-V4-001 — إعادة بناء الـShell وشاشة التطبيقات

> الحالة: IN PROGRESS — موافقة مالك المنتج 2026-11-22
>
> اكتملت الدفعة الثانية: نُقلت الصفحات المفتوحة إلى قائمة عمل سياقية محسوبة العدد بدل صف تبويبات دائم.

نفّذ المرحلة الأولى من `docs/modernization/UI_UX_V4_REBUILD_PLAN.md`: استبدل طبقات التنقل الأربع بشريط علوي نحيف،
تنقل وحدات مضغوط وشاشة تطبيقات مصفاة بالصلاحيات. حافظ على Router وworkspace history وdirty guards واختيار الفرع والجلسة
والبحث والتفضيلات. لا تغيّر API أو schema أو قواعد الأعمال. أغلق المهمة فقط بعد Angular tests وproduction build وتحديث
أدلة التوثيق.


## TASK-ERP-ROADMAP-001 — تدقيق نواقص P0 واختيار أول شريحة وظيفية

> الحالة: DEFERRED BY PRODUCT OWNER — مؤجل حتى إغلاق بوابة UX-V4-001

راجع القدرات الجزئية ذات الأولوية P0 في `ERP_MASTER_ROADMAP.md` مقابل الكود الفعلي، وحدد أول شريحة ERP/Angular رأسية محدودة بمعيار قبول واختبارات واضحة. لا يبدأ تنفيذ الشريحة قبل تسجيلها هنا. الأولوية لوظائف المال والمخزون والصلاحيات، وليس لاستكمال REC-004.

## TASK-REC-004 — توحيد ProblemDetails وتعارضات الأعمال

> الحالة: DEFERRED — REC-004A وREC-004B1 وREC-004B2 وREC-004B3 وREC-004B4 وREC-004B5 وREC-004B6 وREC-004B7 وREC-004B8 وREC-004B9 وREC-004B10 مكتملة

اجرد أنماط معالجة الأخطاء في Internal API، وحدد InvalidOperationException التي تمثل validation أو duplicate أو not-found أو conflict، ثم وحّد الاستجابة 400/404/409/500 مع correlationId ورسالة محلية قابلة للفهم وحفظ نموذج Angular. ابدأ بالمسارات عالية المخاطر التي كشفتها مصفوفة REC-003، ولا تغيّر قواعد العمل نفسها.

## المنجز في REC-004B1

- أضيف كود conflict الثابت لتعارضات الأعمال 409 غير المرتبطة بـrowversion.
- بقيت concurrency حصرية للتعارض المتزامن الصريح، وبقي validation على 400.
- التالي: جرد بقية InvalidOperationException وتحويل الحالات المؤكدة إلى أنواع/استجابات صريحة دون مطابقة رسائل هشة.


## المنجز في REC-004B2

- تأسيس أنواع Application صريحة دون HTTP أو مطابقة رسائل.
- ترحيل القيود اليدوية وأوامر البيع إلى 400/403/404/409 الدلالية.
- التالي: دفعة Inventory/Purchasing مؤكدة ثم قياس ما تبقى من catches الملتبسة.


## المنجز في REC-004B3

- ترحيل Stock Transfers وPurchase Requests إلى التصنيف الصريح.
- خط الأساس المتبقي: 70 Controller و959 موضع InvalidOperation قديم مقابل 51 موضع typed.
- التالي: اختيار دفعة وظيفية محدودة جديدة، لا استبدال شامل حسب النص.


## المنجز في REC-004B4

- duplicate للعملاء والموردين أصبح 409 فعليًا، وconcurrency بقي مستقلًا.
- إزالة message matching من Products API وتصنيف أخطاء المنتج داخل Application.
- الجرد الحالي: 952 legacy مقابل 58 typed؛ التالي دفعة masters/workflows محدودة أخرى.


## المنجز في REC-004B5

- ترحيل Categories/Attributes/Templates وإزالة message matching.
- الجرد الحالي 924 legacy مقابل 86 typed.
- التالي: دفعة إعدادات/مستندات محدودة جديدة ثم إعادة تقييم إغلاق REC-004.

## المنجز في REC-004B6

- ترحيل Measurement Units/Tax Rates/Payment Methods/Branches إلى العقد المصنف.
- الجرد الحالي 915 legacy مقابل 97 typed.
- التالي: دفعة تشغيلية محدودة جديدة، مع إبقاء parsing والحقول على 400.

## المنجز في REC-004B7

- ترحيل خمس خدمات إعدادات مترابطة إلى العقد المصنف.
- الجرد الحالي 905 legacy مقابل 107 typed.
- التالي: دفعة تشغيلية محدودة عالية الوضوح، دون استبدال آلي لنصوص الأخطاء.

## المنجز في REC-004B8

- ترحيل دورة تصميمات المستندات كاملة إلى العقد المصنف.
- الجرد الحالي 897 legacy مقابل 115 typed.
- التالي: دفعة وظيفية محدودة من الجرد، دون تغيير قواعد الأعمال.

## المنجز في REC-004B9

- ترحيل دورة الفترات المالية والتحقق من تاريخ الترحيل إلى العقد المصنف.
- الجرد الحالي 889 legacy مقابل 122 typed.
- التالي: دفعة محاسبية/تشغيلية محدودة مع فحص كل مستهلك للبوابة.

## المنجز في REC-004B10

- ترحيل شجرة الحسابات وبوابة الترحيل/العكس المركزية إلى العقد المصنف.
- الجرد الحالي 883 legacy مقابل 127 typed.
- التالي: دفعة محاسبية محدودة أخرى مع تتبع مستهلكي بوابة الترحيل.
