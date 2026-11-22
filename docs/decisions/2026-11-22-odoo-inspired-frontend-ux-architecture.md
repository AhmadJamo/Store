# ADR — Odoo-inspired frontend UX architecture

> التاريخ: 2026-11-22
> الحالة: Accepted for phased implementation

## السياق

تملك واجهة Angular الحالية 196 route و121 feature page، مع Navigation Registry وWorkspace state ومكونات مشتركة،
لكن طبقات التنقل والتكوين البصري تراكمت وأصبحت كثيفة وغير متجانسة. طلب مالك المنتج إعادة بناء شاملة مستوحاة من
وضوح Odoo، مع السماح بهدم UI الحالي، دون خسارة الوظائف أو تجاوز العقود والصلاحيات.

بحث Odoo 19 الرسمي أثبت أن القيمة القابلة للنقل هي بنية التطبيق، control areas، أنماط List/Form/Kanban، والبحث
المركب. لا يبرر ذلك نسخ العلامة أو الألوان أو افتراض Chatter/Activities/drag-drop/offline contracts غير موجودة.

## القرار

1. تنظيم الواجهة الداخلية في ثلاث طبقات: Global Shell، Current App Navigation، Page Control Area.
2. عرض 10 تطبيقات أعمال رئيسية في App Launcher، وعدم عرض صفحات CRUD كبلاطات.
3. إبقاء URLs الحالية أثناء migration وإضافة presentation metadata فوق Navigation Registry الحالي.
4. إبقاء WorkspaceService سلطة الاستعادة والdirty/history، وعرضه كقائمة Open Work سياقية لا tabs دائمة.
5. اعتماد archetypes موحدة: List، Record، Form، Kanban، Dashboard، Workspace، Settings.
6. تطبيق Odoo patterns فقط عند وجود API/permission capability حقيقية؛ Chatter وActivities وshared favorites وdrag-drop
   وoffline POS تبقى Conditional أو Excluded حتى وجود عقد موثق.
7. استخدام هوية MiniStore أصلية: Plum للـshell وTeal للتفاعل، مع RTL/LTR وresponsive وaccessibility من أساس التصميم.
8. البدء بـProducts List/Form/Record pilot ثم Sales document workflow pilot قبل rollout على الوحدات.

## النتائج

- يمكن إعادة بناء UI بقوة دون إعادة كتابة business logic أو route contracts.
- يتطلب Navigation Registry توسيع metadata واختبار coverage، لكنه يبقى authority واحداً.
- تبقى بعض الوعود البصرية مشروطة بقدرات backend؛ الواجهة لا تحاكي خصائص غير موجودة.
- لا تحذف الصفحات أو CSS القديمة إلا بعد migration matrix واختبارات parity وvisual acceptance.
- Public/Platform/Portal/Login/print surfaces تعامل كاستثناءات ولا تجبر على ERP shell.

## البدائل المرفوضة

- **نسخ Odoo حرفياً:** يخلق تبعية بصرية/وظيفية لا تطابق عقود MiniStore.
- **تجميل كل صفحة منفردة:** يحافظ على التشتت ويضاعف CSS والمكونات.
- **إعادة تسمية كل route أولاً:** مخاطرة عالية بلا قيمة أعمال مباشرة وتكسر deep links.
- **حذف Workspace state:** يخسر dirty guards والاستعادة المثبتة؛ المطلوب تغيير عرضه لا سلطته.
- **تعميم Kanban/Chatter/drag-drop:** يقدم وظائف وهمية أو غير آمنة دون backend capability.

## أدلة القرار

- `docs/frontend-redesign/01-CURRENT-SYSTEM-AUDIT.md`
- `docs/frontend-redesign/02-ODOO-RESEARCH.md`
- `docs/frontend-redesign/03-ODOO-UI-FEATURE-MATRIX.md`
- `docs/frontend-redesign/04-UX-ARCHITECTURE.md`
- `docs/frontend-redesign/06-NAVIGATION-AND-ROUTES.md`
- `docs/project-control/ROUTE_FUNCTION_COVERAGE.ar.md`
