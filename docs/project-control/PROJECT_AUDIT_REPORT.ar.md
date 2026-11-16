# تقرير تدقيق المشروع — V3.0

> التاريخ: 2026-11-16  
> الحالة: تدقيق تأسيسي مكتمل؛ التدقيق الوظيفي التفصيلي مستمر حسب سجل المهام  
> مصدر الحقيقة: الكود الحالي ونتائج الاختبارات، ثم الوثائق كمرجع للقرارات

## نطاق التدقيق المنجز

تم فحص حالة Git وآخر 20 كومتًا، ملفات الحل والمشاريع، بنية Domain/Application/Infrastructure/Web، Angular Router، مصادر الفروع والمستودعات، الصلاحيات، ProblemDetails، الاختبارات، Migrations ومركز التحكم القائم. يحتوي المستودع على 329 ملف Markdown داخل `docs`، و196 مسار Angular مصرّحًا، و148 Migration مصدرية، وأربعة مشاريع إنتاجية وأربعة مشاريع اختبار مستقلة.

هذا التقرير لا يدّعي اختبار كل عملية تجارية بعد؛ جرد الوظائف العابر للطبقات سيستمر كوحدات قابلة للتحقق في TASK-REC-002 وما بعده.

## المعمارية الفعلية

- Modular monolith طبقي: `MiniStore.Domain`, `MiniStore.Application`, `MiniStore.Infrastructure`, `MiniStore.Web`.
- Angular 22 هو سطح الأعمال العادي تحت `/app`؛ ASP.NET Core/.NET 10 هو مضيف same-origin للهوية وAPI وحدود التشغيل المحتفظ بها.
- EF Core 10 وSQL Server مع UnitOfWork ومستودعات؛ لا يجوز تجاوز Application layer.
- `MiniStore.slnx` يحتوي المشاريع الفعلية. `MiniStore.sln` لا يحتوي مشاريع، ولذلك استخدامه يعطي نتيجة مضللة `No projects found`.
- الاختبارات القابلة للتشغيل: SecurityRegression، InventorySqlIntegration، PerformanceSmoke، AngularBrowserAcceptance، و250 اختبار Angular.

## حالة Frontend

- 196 تعريف مسار Angular، lazy-loaded ضمن shell محمي.
- بنية features واسعة مع Design System، workspace tabs، localization عربي/إنجليزي، RTL، وحالات loading/error/empty مشتركة.
- صفحة المستودعات تملك نموذج إنشاء/تعديل فعليًا وتحافظ على المدخلات عند فشل الطلب.
- صفحة الفروع الحالية تعرض الملخص التشغيلي وإدارة منح الوصول، لكنها لا تقدم إنشاء/تعديل/تعطيل واضحًا.

## حالة Backend ومعالجة الأخطاء

- `/api/v1` يستخدم `InternalApiExceptionMiddleware`, `ProblemDetails`, code وcorrelationId.
- Validation responses موحدة عبر `InternalApiProblemFactory`.
- توجد فجوة تصنيف: `InvalidOperationException` غير المعالج داخل Controller يقع حاليًا في 500 بالميدلوير العام، بينما بعض Controllers تمسكه محليًا. يلزم تدقيق اتساق قبل تغيير شامل.

## الفروع

- `Branch` يدعم Code/Name/IsActive وSalesRevenueAccountId، لكن لا يملك سلوك تعديل أو تعطيل.
- `BranchService.CreateAsync` موجود، وPOST موجود فقط في `CatalogMastersApiController` تحت `Administration.Access`.
- صفحة `/branches` لا تملك زر إنشاء أو نموذجًا أو تفاصيل/تعديل.
- الصلاحيات الحالية: `Branches.View`, `Branches.ViewOperations`, `Branches.ManageAccess`; لا توجد Create/Edit/Deactivate دقيقة.
- العلاقات المتقدمة مثل legal entity/address/timezone ليست مدعومة في النموذج الحالي، ولا يجوز اختراعها قبل قرار نطاق ونموذج بيانات.

## المستودعات

- إنشاء/تعديل/حذف وربط Branch/InventoryAccount موجود عبر `WarehousesApiController`, `WarehouseService` وصفحة `/warehouses`.
- صلاحيات View/Create/Edit/Delete مستقلة.
- تغيير control mode وتعطيل POS محميان بقواعد فعلية.
- خطر يحتاج تحقق P1: `DeleteAsync` يفوض الحذف للمستودع مباشرة؛ يجب إثبات أن قيود FK/المستودع تمنع حذف مستودع ذي حركات وإرجاع خطأ مفهوم.

## POS

- يوجد POS فعلي، terminal experience، cash sessions، drawer policies، blind close واعتماد الفروقات.
- V3.0 يطلب نطاقًا أوسع بكثير (Studio، policy engine، offline outbox، device management، reporting). هذه ليست مكتملة بمجرد إغلاق خطة ERP السابقة 10/10.
- إعادة بناء POS من الصفر مؤجلة حتى استقرار P0 وإثبات فجوات الوظائف الحالية؛ لن نحذف الواجهة القائمة قبل تكافؤ مثبت.

## التوثيق

- مركز تحكم قائم بالفعل، لذلك أضيفت ملفات V3.0 دون استبدال السجل التاريخي.
- `docs/00_PROJECT_CONTEXT.md` يحمل في بدايته حالات ERP قديمة (مثل 6/10) ثم لاحقًا إغلاق 10/10؛ هذا تعارض توثيقي مثبت.
- إعلان 10/10 يعني اكتمال roadmap المحدود السابق، وليس اكتمال Enterprise ERP وفق V3.0.

## المخاطر ذات الأولوية

1. P1 — إدارة الفروع غير مكتملة وغير قابلة للاكتشاف من صفحة الفروع.
2. P1 — سلامة حذف المستودع ورسالة الفشل تحتاج اختبار تكامل.
3. P2 — استخدام `MiniStore.sln` يعطي نجاحًا مضللًا بلا مشاريع.
4. P2 — حالات قديمة متعارضة في وثائق السياق.
5. P2 — اتساق تحويل business conflicts إلى 409 ورسائل field-level يحتاج جردًا.
6. P2 — 196 مسارًا لا يعني 196 workflow متحققًا؛ يلزم functional inventory تدريجي.

## التوصية التنفيذية

REC-001 يكمل إدارة الفروع ضمن الموديل الحالي فقط: CRUD الآمن المتاح، صلاحيات دقيقة، واجهة واضحة في `/branches`، ورسائل أخطاء واختبارات. بعده REC-002 يثبت سلامة حذف المستودعات ويغلق فجوة UX إن وجدت. أي توسيع Branch إلى legal entity/address/timezone يحتاج ADR ومخطط بيانات مستقل.
