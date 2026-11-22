# 01 — تدقيق النظام الحالي قبل إعادة بناء الواجهة

> الحالة: مكتمل للبوابة الأولى  
> تاريخ التدقيق: 2026-11-22  
> مرجع Git عند التدقيق: `1356d77`  
> النطاق: Repository وFrontend وطبقات التكامل والاختبارات؛ لا يتضمن حكماً بصرياً نهائياً على Odoo

## 1. الخلاصة التنفيذية

MiniStore نظام ERP قائم واسع الوظائف، وليس نموذجاً أولياً. الواجهة العادية أصبحت Angular، بينما ASP.NET Core يستضيف
عقود `/api/v1` ويحافظ على المصادقة والصلاحيات والسياق. طبقات Domain وApplication وInfrastructure وWeb منفصلة،
وتمنع تعليمات المشروع تجاوز Application layer أو استخدام EF من Controllers.

المشكلة الرئيسية ليست نقص الصفحات، بل غياب لغة عرض وتفاعل واحدة. الجرد الحالي يسجل 196 مسار Angular فريداً و121
صفحة Feature. توجد بنية جيدة للصلاحيات، Router، session، branch context، dirty-state protection، localization،
ProblemDetails والاختبارات، لكن تبني المكونات المشتركة غير متجانس: 110 صفحات تستخدم PageHeader، مقابل 42 جدولاً
مشتركاً، و3 FilterBar، و3 SummaryStrip، و4 WorkflowProgress فقط.

القرار الناتج: إعادة البناء ستكون Presentation Migration فوق العقود الحقيقية، وليست إعادة كتابة ERP أو Backend.

## 2. مصادر الحقيقة المستخدمة

- `docs/00_PROJECT_CONTEXT.md` و`docs/01_CODEBASE_MAP.md` و`docs/02_ARCHITECTURE.md`.
- `docs/project-control/ROUTE_FUNCTION_COVERAGE.ar.md`: المصفوفة التفصيلية لـ196 مساراً.
- `docs/screens/screen-map.md` ووثائق الوحدات والقرارات المعمارية.
- `MiniStore Front-End/src/app/app.routes.ts` وملفات Routes الكسولة.
- `MiniStore Front-End/src/app/workspace/navigation/navigation-registry.service.ts`.
- `MiniStore.Web/Controllers/Api/V1` وطبقة Application.
- نتائج `Test-UiUxConsistency.ps1` و`Test-AngularStaticAccessibility.ps1`.
- Git history حتى Commit `1356d77`.

مصفوفة `ROUTE_FUNCTION_COVERAGE.ar.md` هي الملحق التفصيلي لهذا التقرير: كل صف فيها يسجل Route، Component،
API، Controller، Application Service، Permissions واختبار Angular. لا ننسخ 196 صفاً هنا حتى لا ننشئ مصدر حقيقة
ثانياً قابلاً للتقادم.

## 3. بنية المستودع

| المنطقة | الواقع الحالي | ملاحظة إعادة البناء |
|---|---|---|
| Domain | `MiniStore.Domain` | قواعد الأعمال والحالة؛ خارج نطاق إعادة العرض |
| Application | `MiniStore.Application`، 283 ملف C# | DTOs وخدمات التطبيق والسلطة التشغيلية |
| Infrastructure | `MiniStore.Infrastructure` | EF Core وRepositories وUnitOfWork |
| Web/API | `MiniStore.Web`، 91 ملف API Controller و93 Controller class | المصادقة والصلاحيات وProblemDetails وعقود v1 |
| Angular | `MiniStore Front-End`، Angular 22.2 | هدف إعادة بناء Presentation |
| Tests | Angular + 4 مشاريع تحقق إضافية؛ `MiniStore.slnx` يسجل AngularBrowserAcceptance | بوابات مرحلية لا يجوز تجاوزها |
| Legacy solution | `MiniStore.sln` فارغ؛ `MiniStore.slnx` هو Solution الفعلي | يجب استخدام slnx في الأوامر الجديدة |

## 4. تقنية Frontend الفعلية

| البند | الواقع |
|---|---|
| Angular | 22.2، standalone components، strict TypeScript |
| Build | `@angular/build:application`، production base href = `/app/` |
| State | Angular Signals وRxJS؛ لا يوجد NgRx |
| Change detection | استخدام واسع لـOnPush |
| Routing | Router رئيسي + lazy feature routes + auth/anonymous guards |
| HTTP | Fetch backend مع Interceptors مرتبة: branch context، CSRF، mutation toast، auth failure |
| Session | cookie/session خادمية؛ لا تخزين credentials أو tokens في localStorage |
| Localization | `en-US` و`ar-JO`، Locale يملك `lang/dir` |
| UI preferences | theme/density/sidebar/content width/profile؛ Presentation only |
| Forms | Reactive Forms غالباً، مع dirty guards وحفظ rejected input في المسارات الحديثة |
| Tests | 122 ملف spec و258 اختباراً ناجحاً وقت التدقيق |
| Templates | 138 قالب Angular، منها 121 Feature page |
| Styling | 110 ملف SCSS إضافة إلى `src/styles.scss` |

## 5. حدود Core وWorkspace الموجودة

### Core

- `ApiSessionService`: session وpermissions المرئية.
- `authGuard` و`anonymousOnlyGuard`: حدود المسارات.
- `csrfInterceptor`: حماية الأوامر الداخلية.
- `authFailureInterceptor`: انتهاء الجلسة ومسار الدخول.
- `operationErrorToastInterceptor`: أخطاء mutation غير الحقلية.
- `BranchContextService` وInterceptor: سياق فرع مختار يعاد التحقق منه خادمياً.
- `LocalizationService` و`UiCultureStateService`: اللغة والاتجاه.
- `UiPreferencesService` و`WorkspaceProfileService`: حالة عرض غير سلطوية.

### Workspace

- `NavigationRegistryService`: الوجهات والصلاحيات المرئية.
- `WorkspaceService`: الصفحات المفتوحة، history، recently closed، dirty-state.
- `NavigationGroupStateService`: حالة مجموعات التنقل.
- AppShell الحالي: top bar، module rail، command search، branch/session/preferences، وقائمة عمل سياقية.

هذه الخدمات يجب إعادة استخدامها. لا يُنشأ Router أو permission model أو transaction concept جديد في Design System.

## 6. جرد المسارات والوحدات

المصفوفة التنفيذية الحالية تسجل 196 مساراً، 182 منها لها دليل عابر للطبقات، بلا فجوات P1، و7 حدود P2 تحتاج
مراجعة يدوية. التوزيع الأهم:

| الوحدة | المسارات | الوضع الوظيفي | مشكلة العرض الأساسية | اتجاه V4 |
|---|---:|---|---|---|
| Purchases | 59 | واسع ومتكامل | صفحات كثيرة بتراكيب محلية وكثافة غير موحدة | List/Document/Form patterns |
| Sales | 37 | أوامر وتسعير وفواتير ومدفوعات وPOS | عرض الوثائق والإجراءات غير موحد | Document pattern ثم POS مستقل |
| Inventory | 22 | مخزون وتتبع وتحويل ومسح وتسويات | جداول وأدلة حركة موزعة بصرياً | Enterprise List + evidence panels |
| Settings | 18 | إعدادات وهوية وفترات وصلاحيات وصول | نماذج مختلفة وحمل بصري مرتفع | Settings/Form archetype |
| Accounting | 11 | قيود وتقارير وتسويات وإغلاق | يحتاج كثافة ودقة حالات واضحة | Journal/Report/Document archetypes |
| Products | 11 | master/categories/attributes/templates/recipes | أفضل Pilot للعقد المشترك | أول List/Form/Kanban pilot |
| Customers | 4 | list/details/create/edit | Vertical slice ناضج لكن شكله قديم | Record form + smart links |
| Suppliers | 4 | list/details/create/edit | مشابه للعملاء مع عقود شراء | Record form + purchasing links |
| Roles | 3 | list/create/edit | صلاحيات حساسة تحتاج hierarchy واضح | Permission-aware settings |
| Analytics | 3 | view/builder/access | بنية مختلفة مبررة | Report/Builder archetype |
| Users | 2 | list/create | يحتاج named role selection | Enterprise List + Form |
| Documents | 2 | evidence list/details | يحتاج ترابط مع السجلات | Evidence panel |
| Branches | 1 | CRUD وحالة ومنح | كل الوظائف في صفحة متماسكة وكبيرة | List + Form + related records |
| Warehouses | 1 | CRUD كامل | صفحة واحدة تحمل list/form معاً | فصل List/Form ضمن نفس العقود |
| POS | ضمن Sales | sessions/cash/sales/returns | Workspace تشغيلي مختلف | Touch-first shell مستقل |
| Public/Platform/Portal | حدود مستقلة | دخول وتسجيل ومنصة وبوابة مورد | لا ينبغي فرض ERP shell عليها | استثناءات موثقة |

## 7. جرد أنماط الشاشات الحالية

| النمط | الدليل الحالي | الملاحظة |
|---|---:|---|
| PageHeader | 110 | انتشار جيد لكنه لا يكفي لتوحيد الصفحة |
| DataTable | 42 | غالبية القوائم ما زالت خارج عقد واحد |
| FilterBar | 3 | فجوة كبيرة في البحث والتصفية |
| SummaryStrip | 3 | مستخدم فقط في Pilots |
| FormSection | 3 | النماذج الباقية تملك بنية محلية |
| DetailSection | 3 | تفاصيل السجل غير موحدة |
| WorkflowProgress | 4 | حالات المستندات لا تستخدم لغة واحدة |
| WorkflowFooter | 5 | الإجراءات النهائية موزعة |
| FeedbackState | 111 | تغطية قوية للحالات العامة |
| ApiRecovery | 110 | تغطية جيدة لاسترداد أخطاء القراءة |

كما وجد الجرد السابق 31 قالباً يملك layout محلياً باسم filters و19 قالباً يملك cards محلية. توجد ملفات مصدر كثيرة
مضغوطة في أسطر طويلة، ما يزيد كلفة المراجعة والتعديل ويجب علاجه عند لمس كل Feature، لا بحملة formatting عمياء.

## 8. العقود والسلطات التي لا يجوز نقلها إلى Angular

- الأسعار والضرائب والإجماليات والمخزون والتوافر والتكلفة والحسابات المالية.
- انتقالات workflow وإمكانية تنفيذ command.
- صلاحيات المستخدم والوصول للفرع والشركة.
- rowversion، idempotency، transaction boundaries وUnitOfWork.
- أرقام المستندات، posting، reversal، reservation، delivery وmatching.
- إخفاء UI لا يعتبر تفويضاً؛ كل Endpoint يعيد التحقق خادمياً.

## 9. نقاط القوة التي يجب الحفاظ عليها

1. Route-to-API coverage مولد آلياً ولا توجد فجوات P1.
2. Session وCSRF وProblemDetails وbranch context مركزية.
3. أسماء المنتجات والمستودعات والفروع هي واجهة المستخدم، والـIDs داخلية.
4. العربية والإنجليزية مدعومتان واتجاه الصفحة مركزي.
5. 258 اختبار Angular يعمل مع Build ناجح.
6. lazy loading واسع، وOnPush/Signals موجودان.
7. dirty guards وحفظ rejected input مستخدمان في المسارات الحديثة.
8. طبقات Backend وDDD/UnitOfWork واضحة.

## 10. المخاطر والفجوات

| الخطر | الشدة | الدليل | المعالجة |
|---|---|---|---|
| عدم اتساق Layout بين 121 صفحة | عالٍ | نسب المكونات المشتركة | اعتماد Archetypes قبل rollout |
| إنشاء Generic component ضخم | عالٍ | تنوع Purchases/Sales/Accounting | تجريد بعد مستهلكين حقيقيين |
| إظهار View غير مدعوم بالخادم | عالٍ | تفاوت filtering/grouping endpoints | Feature capability contract |
| تضخم CSS | متوسط/عالٍ | shell style budget warning | Tokens وretire rules لكل Route |
| launcher مزدحم | متوسط | registry يحوي 80 destination | launcher للوحدات، command search للصفحات |
| قبول بصري غير مكتمل | عالٍ | لا يوجد screenshot gate حديث لهذه الدفعة | Desktop/tablet/mobile + RTL/LTR |
| 7 حدود P2 | متوسط | route coverage | مراجعة يدوية قبل migration |
| Solution قديم فارغ | منخفض | `MiniStore.sln` | توثيق واستخدام `MiniStore.slnx` |
| أسطر مصدر مفرطة الطول | متوسط | source scan | تنظيف محلي عند migration |
| Bundle warnings | متوسط | initial script وshell SCSS | قياس وتقليل قبل التعميم |

## 11. استراتيجية إعادة البناء الناتجة

1. تجميد تعميم الـpilot الحالي حتى اكتمال Odoo Research وUX Architecture وDesign System specification.
2. بناء capability map لكل Archetype؛ لا يظهر Filter/Group/View إلا إذا كان Backend يدعمه.
3. اعتماد Products كـList/Form pilot بعد إغلاق design gate.
4. اعتماد Sales Order كـDocument pilot بعد نجاح Products.
5. نقل الوحدات حسب dependency map، مع مقارنة كل Action وPermission وAPI قبل استبدال الصفحة.
6. إبقاء POS كتجربة touch-first مرتبطة بنفس session/permissions/business services.
7. إزالة CSS والمكونات القديمة فقط بعد قبول Route المقابل.

## 12. معيار إغلاق مرحلة Audit

- [x] فحص بنية المستودع وSolution الفعلي.
- [x] تثبيت إصدارات Angular وأدوات البناء.
- [x] جرد Router/Core/Interceptors/Guards/State.
- [x] ربط الجرد بمصفوفة 196 Route.
- [x] توثيق الوحدات والمشكلات والحدود غير القابلة للنقل.
- [x] مراجعة Git history وآخر دفعتين للـUI.
- [x] تحديد مخاطر Design System وMigration.
- [ ] Odoo Research؛ بوابة مستقلة في الوثيقة 02.

## 13. ملاحظة حول الـpilot المنفذ

Commitان سبقا وصول الـMaster Prompt:

- `d47e9c0`: shell/app launcher V4 first slice.
- `1356d77`: contextual open-work menu.

هما قابلان للتراجع ومحافظان على العقود، لكنهما لا يمثلان Design System نهائياً. سيُعاد تقييمهما مقابل البحث الرسمي قبل
تعميم الشكل أو نسخ مكوناته إلى صفحات أخرى.
