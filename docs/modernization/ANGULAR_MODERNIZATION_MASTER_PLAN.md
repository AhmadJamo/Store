# خطة تحديث MiniStore إلى Angular وASP.NET Core API

> Status: APPROVED FOR INCREMENTAL EXECUTION  
> Approved: 2026-10-24  
> Backend branch: `codex/angular-modernization-foundation`  
> Frontend workspace: `C:\Users\ahja\source\repos\MiniStore\MiniStore Front-End`

## الهدف

تحديث طبقة العرض تدريجيًا من Razor MVC إلى Angular مع ASP.NET Core API، مع إبقاء MVC عاملًا إلى أن تثبت مطابقة الوظائف والأمان. لا تعيد الخطة كتابة Domain أو Application services أو الحسابات المحاسبية والمخزنية.

## الحدود الثابتة

- الـbackend هو المرجع الوحيد لقواعد الأعمال والصلاحيات والحسابات والترقيم والمعاملات والتزامن والتدقيق.
- لا تُعرض EF entities عبر API؛ تستخدم DTOs صريحة.
- لا تغيير لقاعدة البيانات أو business rules دون موافقة مستقلة.
- لا إزالة لمسار MVC قبل اكتمال parity وخطة rollback معتمدة.
- Angular لا يخزن long-lived credentials أو financial payloads حساسة في browser storage.
- reuse قبل الإنشاء، واستخراج المكونات المشتركة بعد ظهور استخدام حقيقي.

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
| 4 | sidebar، tabs، history مستقل، restoration وdirty guard | workspace unit/E2E tests |
| 5 | Customers vertical slice مع بقاء MVC | API/UI parity وconcurrency |
| 6 | shared forms/tables مستخرجة من استخدامين فعليين | لا generic CRUD engine |
| 7 | نقل الوحدات تدريجيًا حسب المخاطر | موافقة لكل milestone |
| 8 | Command Center، Quick Preview، favorites وsnapshots | authorization لكل نتيجة |
| 9 | smart forms، saved filters، keyboard entry وdraft policy | لا posting تلقائي |
| 10 | dashboard/density/focus/layout preferences | reset وسلوك responsive |
| 11 | duplicate/conflict/data-quality/error recovery | read-only أو قواعد معتمدة |
| 12 | document journey، related records وExplain This Number | backend authoritative evidence |
| 13 | action/exception center وcontextual help | permission-aware tasks |
| 14 | split workspace وrole profiles | لا permission grants من profiles |
| 15 | group/drill-down/period comparison/export preview | bounded server queries |
| 16 | background job center، scheduled reports وSLA | موافقة scheduler/retention |
| 17 | report builder، delegation وfinancial preview | موافقات مستقلة |
| 18 | security/performance/accessibility/compatibility acceptance | evidence كاملة |
| 19 | MVC retirement assessment | explicit approval فقط |

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
- MVC يبقى هو المسار التشغيلي حتى اعتماد parity.
- يمكن تعطيل Angular route/proxy دون تغيير Domain أو database.
- أي schema change مستقبلي يحتاج migration إضافية وخطة rollback مستقلة.

## المخاطر

- ازدواجية مؤقتة بين MVC وAngular.
- CSRF/CORS إن فُصلت النطاقات؛ same-origin هو الافتراضي.
- تضخم API إن تم نسخ ViewModels دون تصميم contracts.
- overengineering في workspace/shared components.
- نقص E2E authenticated journeys حاليًا.
- لا production topology أو CI pipeline مثبتين بعد.
