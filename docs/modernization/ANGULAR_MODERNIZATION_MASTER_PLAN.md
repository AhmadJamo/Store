# خطة تحديث MiniStore إلى Angular وASP.NET Core API

> Status: APPROVED FOR INCREMENTAL EXECUTION  
> Approved: 2026-10-24  
> Backend branch: `codex/angular-modernization-foundation`  
> Frontend workspace: `C:\Users\ahja\source\repos\MiniStore\MiniStore Front-End`

## الهدف

تحديث طبقة العرض تدريجيًا من Razor MVC إلى Angular مع ASP.NET Core API، ثم نقل جميع صفحات وإجراءات MVC
حتى تصبح Angular واجهة ERP الوحيدة بعد إثبات المطابقة الكاملة. يبقى MVC عاملًا لكل وحدة إلى أن تثبت مطابقة
وظائفها وأمانها ويُفعّل cutover قابل للرجوع. لا تعيد الخطة كتابة Domain أو Application services أو الحسابات
المحاسبية والمخزنية. التفاصيل الملزمة في `FULL_MVC_TO_ANGULAR_MIGRATION_PLAN.md`.

## الحدود الثابتة

- الـbackend هو المرجع الوحيد لقواعد الأعمال والصلاحيات والحسابات والترقيم والمعاملات والتزامن والتدقيق.
- لا تُعرض EF entities عبر API؛ تستخدم DTOs صريحة.
- لا تغيير لقاعدة البيانات أو business rules دون موافقة مستقلة.
- لا إزالة لمسار MVC قبل اكتمال parity وخطة rollback معتمدة.
- Angular لا يخزن long-lived credentials أو financial payloads حساسة في browser storage.
- reuse قبل الإنشاء، واستخراج المكونات المشتركة بعد ظهور استخدام حقيقي.
- تُنقل وظائف MVC كاملة لكن يُعاد تصميم الشاشات حصريًا حسب خط Angular الذي بنيناه؛ MVC ليس مرجعًا بصريًا
  ولا تُطلب visual parity معه، بل functional parity فقط.
- كل CSS/SCSS منفصل عن HTML، وكل شاشة تجتاز responsive وRTL/LTR وtheme/density وaccessibility acceptance.

## المعمارية المستهدفة

```text
Angular workspace
  -> typed API client
  -> versioned ASP.NET Core API (/api/v1)
  -> existing Application services
  -> Domain
  -> existing repositories + UnitOfWork
  -> EF Core / SQL Server
```

في مرحلة الانتقال يستضيف `MiniStore.Web` كلاً من MVC وAPI لتجنب تكرار المصادقة وtenant middleware وDI. يستخدم التطوير Angular proxy، ويكون النشر النهائي same-origin أو خلف reverse proxy موحد.

## تسلسل التنفيذ

| Phase | الناتج | بوابة القبول |
|---|---|---|
| 0 | repository audit وخريطة المخاطر | مكتمل 2026-10-24 |
| 1 | فرع معزول، وثائق، Angular foundation وبنية build | build/test ناجحان ولا تغيير backend runtime |
| 2 | API conventions، session/CSRF، permissions، ProblemDetails وOpenAPI | مكتمل 2026-10-24؛ build و1072 regression وlive 401 ناجحة |
| 3 | design tokens، RTL/LTR، themes، density وaccessibility | مكتمل 2026-10-24؛ build و6 tests ومراجعة visual/AX ناجحة |
| 4 | sidebar، tabs، history مستقل، restoration وdirty guard | مكتمل 2026-10-24؛ build و12 tests و1072 regression ناجحة |
| 5 | Customers vertical slice مع بقاء MVC | مكتمل 2026-10-24؛ API/UI parity وconcurrency و17 tests و1076 regression ناجحة |
| 6 | shared forms/tables مستخرجة من استخدامين فعليين | مكتمل 2026-10-24؛ أربعة presentation primitives وCustomers/Suppliers دون generic CRUD |
| 7 | نقل الوحدات تدريجيًا حسب المخاطر | بدأ التنفيذ؛ MOD-007 أكمل Suppliers list/details/create/edit مع MVC rollback و26 اختبار Angular و1081 فحص backend |
| 8 | Command Center، Quick Preview، favorites وsnapshots | مكتمل 2026-10-24؛ مزودا Customers/Suppliers وallowlisted preferences و30 اختبار Angular |
| 9 | smart forms، saved filters، keyboard entry وdraft policy | مكتمل 2026-10-24؛ no-local-draft policy وsafe shortcuts/errors/preferences و36 اختبار Angular |
| 10 | dashboard/density/focus/layout preferences | مكتمل 2026-10-24؛ versioned layout وmobile navigation وskip/dialog focus و39 اختبار Angular |
| 11 | duplicate/conflict/data-quality/error recovery | مكتمل 2026-10-24؛ stable codes وrecovery عام و52 اختبار Angular |
| 12 | document journey، related records وExplain This Number | مكتمل 2026-10-24؛ Sales Order/Customer Invoice evidence خادمي وصلاحيات مستقلة |
| 13 | action/exception center وcontextual help | مكتمل 2026-10-24؛ قرارات backend وصلاحيات View/action |
| 14 | split workspace وrole profiles | مكتمل 2026-10-24؛ ملفات allowlisted لا تمنح صلاحيات |
| 15 | group/drill-down/period comparison/export preview | مكتمل 2026-10-24؛ مزودات backend وحفر/معاينة محدودة |
| 16 | background job center، refresh plans وruntime SLA | مكتمل 2026-10-24؛ ثلاثة workers حقيقيين ولا ادعاء scheduled delivery |
| 17 | report builder، delegation وfinancial preview | موافقات مستقلة |
| 18 | بوابة قبول security/performance/accessibility/browser/E2E مشتركة | evidence قابلة للتكرار قبل كل cutover |
| 19 | جرد MVC الكامل وmigration registry وroute cutover controls | لا شاشة أو action خارج السجل |
| 20 | الهوية والمستخدمون والأدوار وonboarding وإعدادات المنشأة | parity + rollback لكل route |
| 21 | الكتالوج والبيانات الأساسية والفروع والمستودعات | parity كاملة |
| 22 | المخزون وWMS | مصالحة كمية/تتبع/تزامن |
| 23 | المشتريات والتكاملات | procure-to-pay كامل |
| 24 | المبيعات الاحترافية والذمم | quote-to-cash كامل |
| 25 | Legacy Sales وPOS | تشغيل وطباعة وإغلاق كامل |
| 26 | المحاسبة والإقفال والتدقيق | مصالحة مالية وأدلة مصدر |
| 27 | SaaS والعام وPlatform owner | فصل auth/tenant/platform محفوظ |
| 28 | المصمم وPDF والملفات والاستيراد/التصدير وكل الصفحات المتبقية | صفر فجوات في registry |
| 29 | قبول شامل وdual-run وتجربة cutover/rollback | كل الوحدات Parity Proven |
| 30 | Angular-only cutover وإزالة MVC تدريجيًا | موافقة صريحة فقط |

## MVP

1. Angular shell ثنائي اللغة.
2. same-origin session bootstrap وCSRF.
3. permission-aware navigation.
4. typed API client وProblemDetails.
5. workspace tabs/history/dirty state.
6. Customers list/details/create/edit.
7. server-side search/sort/pagination.
8. concurrency/error handling.
9. unit/API/Playwright verification.
10. MVC rollback path محفوظ.

## state ownership

- Angular Router: route الحالية وbrowser URL.
- Workspace service: tabs، active tab، history وdirty markers.
- Feature services/signals: UI state المحلية.
- RxJS/HttpClient: asynchronous API flows والإلغاء.
- Backend: business state والpermissions والحسابات.
- لا NgRx قبل إثبات حاجة لا تغطيها signals/services بوضوح.

## الاختبارات المطلوبة

- Backend build وregression قبل وبعد كل API slice.
- API integration للمصادقة والصلاحيات وtenant isolation وvalidation والتزامن.
- Angular unit tests للتوجيه والحالة والترجمة.
- Playwright للتبويبات، Back/Forward، refresh restoration، dirty form، RTL/LTR وpermission changes.
- لا تعد Phase مكتملة إلا بنتائج اختبارات منفذة فعليًا.

## rollback

- نقطة الأساس: commit `fd0deb7082bcc7c8cd5ae19061a33c300d6aad6e` على `master`.
- التطوير على `codex/angular-modernization-foundation`.
- MVC يبقى مسار الرجوع لكل وحدة حتى اعتماد parity ونجاح cutover الخاص بها، ثم يبقى الكود حتى `MOD-030`.
- يمكن تعطيل Angular route/proxy دون تغيير Domain أو database.
- أي schema change مستقبلي يحتاج migration إضافية وخطة rollback مستقلة.

## المخاطر

- ازدواجية مؤقتة طويلة نسبيًا بين MVC وAngular حتى اكتمال موجات `MOD-020..029`.
- CSRF/CORS إن فُصلت النطاقات؛ same-origin هو الافتراضي.
- تضخم API إن تم نسخ ViewModels دون تصميم contracts.
- overengineering في workspace/shared components.
- نقص E2E authenticated journeys حاليًا.
- لا production topology أو CI pipeline مثبتين بعد.
