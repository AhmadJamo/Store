# سجل مهام استعادة المشروع

| ID | المهمة | الوحدة | المصدر | الأولوية | الحالة | الاعتماديات |
|---|---|---|---|---|---|---|
| TASK-REC-000 | التدقيق التأسيسي ومركز التحكم V3.0 | Cross-cutting | V3.0 + كود | P0 | Completed | — |
| TASK-REC-001 | إدارة الفروع من صفحة الفروع | Branches | V3.0 + فجوة مثبتة | P0 | Completed | REC-000 |
| TASK-REC-002 | إثبات سلامة حذف المستودع وUX الإنشاء | Warehouses | تدقيق كود | P0 | Completed | REC-000 |
| TASK-REC-003 | جرد 196 مسارًا ووظائفها عبر الطبقات | All | V3.0 | P0 | Completed | REC-000 |
| TASK-REC-004 | اتساق ProblemDetails وتصنيف business conflict | Cross-cutting | تدقيق كود | P0 | Ready | REC-003 |
| TASK-POS-001 | تدقيق فجوات POS مقابل V3.0 | POS | V3.0 | P1 | Planned | REC-001..004 |
| TASK-POS-002 | قرار معمارية Offline | POS | V3.0 | P3 | Deferred | POS-001 + ADR |
| TASK-REP-001 | تدقيق semantic reporting coverage | Reporting | V3.0 | P4 | Planned | REC-003 |

## TASK-REC-000

- الوصف: إنشاء baseline مصدره الكود وفصل roadmap القديم عن برنامج Recovery الجديد.
- الملفات: تقارير project-control العربية الجديدة.
- القبول: جرد المشاريع والمسارات والتوثيق، مخاطر مثبتة، سجل أخطاء ومهمة تالية.
- النتيجة: Completed؛ لا تعديل runtime.

## TASK-REC-001

- الوحدة: Branch Management.
- الوصف: نقل إنشاء الفرع إلى تجربة قابلة للاكتشاف في `/branches`، إضافة تعديل/تعطيل ضمن نموذج Branch الحالي، وفصل Create/Edit permissions عن Administration.Access.
- الملفات المتوقعة: Branch entity/service/repository DTO/API/permissions/Angular branches/tests/docs.
- القبول: create ثم reload، edit، deactivate/activate، duplicate validation، unauthorized/forbidden، عدم فقد قيم النموذج، عربي/إنجليزي وRTL.
- الاختبارات: Release، SecurityRegression، Angular unit/build، Edge authenticated.
- النتيجة: اكتملت واجهات الإنشاء والتعديل والتفعيل/التعطيل والصلاحيات المنفصلة والتدقيق؛ التحقق المتصفحي الآلي العام بقي خارج دليل القبول بسبب عطل selector سابق غير متعلق بالشاشة.
- ملاحظة: legal entity/address/timezone خارج هذه الشريحة حتى ADR منفصل.

## TASK-REC-002

- النتيجة: قيود SQL كانت تمنع حذف المستودع المرتبط لكنها كانت تتسرب كخطأ خادم؛ أصبح المستودع البرمجي يحول رفض FK إلى تعارض أعمال معروف وAPI يعيده 409 مترجمًا.
- اختبار قاعدة حقيقية: المستودع المرتبط بـ ProductStock/StockTransaction بقي مع بياناته، بينما حُذف مستودع غير مستخدم.
- UX: شاشة `/warehouses` كانت تحتوي أصلًا create/edit وربط الفرع والحساب بصلاحيات مستقلة؛ لم تُعد كتابتها. أضيف اختبار يثبت بقاء الصف ورسالة الخطأ عند رفض الحذف.
- التحقق: Release clean، SQL integration passed، Angular 251/251، SecurityRegression 1,650.
## TASK-REC-003

- المخرج: `ROUTE_FUNCTION_COVERAGE.ar.md` مولد من الكود ويربط 196 مسار شاشة بالمكوّن وعقود API والـController وخدمة Application والصلاحيات واختبار Angular.
- النتيجة: 0 فجوات P1 في الربط؛ 182 مسارًا بدليل عابر للطبقات مباشر، وحدود عامة/عرضية معروفة، و7 حالات P2 ذات تفويض authenticated أو API-specific تحتاج مراجعة يدوية مستقلة.
- الوحدات الحرجة: Accounting (11)، Sales (37)، Inventory (22) دون P1.
- البوابة: `Test-RouteFunctionCoverage.ps1` تعيد التوليد وتفشل عند أي P1 أو غياب وحدة حرجة.

## TASK-REC-004A

- الحالة: Completed.
- النطاق: توستر لأخطاء mutation غير الحقلية فقط؛ لا تغيير لتصنيف الخادم.
- الدليل: 256 Angular tests، production build، initial/lazy budgets.
- المتبقي: REC-004B جرد وتوحيد 400/404/409/500 وأكواد ProblemDetails.
## TASK-REC-004B1

- الحالة: Completed — 2026-11-17.
- النتيجة: كود `conflict` ثابت لتعارضات الأعمال 409؛ `concurrency` محصور بتعارض rowversion الصريح؛ validation بقي 400.
- النطاق: حذف المنتج/المستودع وتعارضات الموردين المعروفة، مع تصنيف Angular واختبارات منع الانحدار.
- المتبقي: استكمال جرد InvalidOperationException وتصنيف الحالات المؤكدة فقط دون مطابقة نصوص هشة.

## TASK-REC-004B2

- الحالة: Completed — 2026-11-17.
- النطاق: typed Application problems + manual journals + sales orders.
- التالي: Inventory/Purchasing cohort.


## TASK-REC-004B3

- الحالة: Completed — 2026-11-17.
- النطاق: Stock Transfers + Purchase Requests typed ProblemDetails.
- التالي: REC-004B4 من الجرد المقاس.


## TASK-REC-004B4

- الحالة: Completed — 2026-11-17.
- النطاق: Customers + Suppliers + Products typed duplicate/concurrency/conflict.


## TASK-REC-004B5

- الحالة: Completed — 2026-11-17.
- النطاق: Product Categories + Attributes + Templates.

## TASK-REC-004B6

- الحالة: Completed — 2026-11-17.
- النطاق: Measurement Units + Tax Rates + Payment Methods + Branches.
- التالي: اختيار دفعة مترابطة جديدة من الجرد ثم إعادة تقييم إغلاق REC-004.

## TASK-REC-004B7

- الحالة: Completed — 2026-11-17.
- النطاق: General + Purchase Sourcing + POS Experience + Exchange Rates + Accounting Settings.
- التالي: دفعة وظائف تشغيلية محدودة ثم مراجعة نطاق REC-004 المتبقي.
