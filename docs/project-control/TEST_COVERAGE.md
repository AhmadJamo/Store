# تغطية الاختبارات / Test Coverage

## آخر خط أساس منفذ

أحدث بوابة بعد `MOD-030E2G` في 2026-11-01: Release نجح بلا تحذيرات أو أخطاء، وSecurityRegression نجح في
1,447 فحصًا، وAngular نجح في 218/218 اختبارًا عبر 108 ملفات. نجح فحص الوصول الثابت لـ122 قالبًا، ونجحت ميزانية
الإنتاج عند 327,391/358,400 بايت أولية و81,880/81,920 لأكبر lazy chunk. تشغيل Demo المصرح به نجح 15/15 بلا
فشل أو تخطٍ، ونجح القطع والرجوع على 49 مسارًا مصرحًا.

| الأمر | النتيجة | التاريخ |
|---|---|---|
| Authenticated MOD-029B dual-run gate | 9/9 browser checks, zero skips; culture, financial empty-state and real inventory row reconciled across MVC/API; 100/100 performance requests passed | 2026-10-30 |
| Authenticated MOD-029C high-risk cohorts | 15/15 browser checks, zero skips; permission-allowed GET/JSON reads passed for sales, purchasing, inventory, accounting and administration | 2026-10-30 |
| `dotnet run --project tests/SecurityRegression/SecurityRegression.csproj -c Release --no-build` | Passed 1072 checks (نجح 1072 فحصًا)، وتشمل الآن versioned authenticated session API، حماية mutation بـantiforgery، paging bounds/sort allow-list، واستخدام permission boundary المشترك | 2026-10-24 |
| `dotnet build tests/SecurityRegression/SecurityRegression.csproj -c Release --no-restore` | Passed Web and dependencies, 0 warnings/errors after MOD-002 | 2026-10-24 |
| Live anonymous internal API probes | session، CSRF وOpenAPI أعادت `401 application/problem+json` دون HTML redirects | 2026-10-24 |
| `npm run build` in `MiniStore Front-End` | Passed Angular 22 production build after MOD-003; 256.08 kB raw initial bundle with lazy shell/home/design-system chunks | 2026-10-24 |
| `npm test -- --watch=false` in `MiniStore Front-End` | Passed 4/4 Vitest files and 6/6 tests, including CSRF, theme/density persistence/reset and locale RTL/LTR synchronization | 2026-10-24 |
| `npm audit --omit=dev --json` in `MiniStore Front-End` | 0 production dependency vulnerabilities | 2026-10-24 |
| Live visual/accessibility-tree review at `/design-system` | Arabic RTL shell, labels, native controls and empty/loading/error/status states verified | 2026-10-24 |
| `dotnet build MiniStore.sln -c Release --no-restore` | Passed, 0 warnings/errors after the complete workflow/localization audit | 2026-10-23 |
| `dotnet run --project tests/InventorySqlIntegration/InventorySqlIntegration.csproj -c Release --no-build` | BLOCKED before test execution by Windows SQL SSPI; quotation-template snapshot scenario is compiled but unverified on SQL | 2026-10-22 |
| `pwsh -NoProfile -File scripts/verify-local.ps1` | Build + 777 + SQL + 500-request workload passed | 2026-10-18 |
| `dotnet run --project tests/InventorySqlIntegration/InventorySqlIntegration.csproj -c Release` | Passed full disposable SQL suite, including partial invoices, one-time balanced receivable/revenue posting, branch/warehouse dimensions, exact inverse journal, released eligibility and zero stock/movement mutation | 2026-10-21 |
| Live local historic-credential negative test (اختبار رفض بيانات الدخول التاريخية) | Rejected after remediation (رُفضت بعد المعالجة) | 2026-10-15 |
| Live local replacement-admin positive test (اختبار دخول المدير البديل) | Passed with authorized redirect (نجح مع تحويل مصرح) | 2026-10-15 |
| Encrypted backup verify/isolated restore/DBCC (تحقق النسخة والاستعادة المعزولة وسلامة القاعدة) | Passed with AES-256 and checksum (نجح) | 2026-10-15 |
| Restored ERP reconciliation (مصالحة النظام المستعاد) | 147 tables and 21 business checks; zero mismatches (صفر فروق) | 2026-10-15 |
| EF/database migration reconciliation (مطابقة الترحيلات) | Source/model/local Demo reconciled through migration 136; no pending migrations or model changes | 2026-10-30 |
| MOD-029D live cutover/rollback | 10 authorized allowlisted GET routes redirected to exact Angular targets, then returned MVC 200 after restart; final full gate passed | 2026-10-30 |
| MOD-030A hosted Angular production | `/app` index/deep link 200 no-cache, hashed asset 200 immutable, API remained separate; authenticated hosted run 15/15 and mixed Customers pilot 10 routes passed | 2026-10-30 |
| MOD-030B Suppliers/Products cohort | Hosted deep links passed; authenticated run 15/15 with zero skips; 12-route mixed pilot and 12-route MVC rollback passed; Release clean, 1,349 backend checks, 214/214 Angular tests, 121 templates and bundle budgets passed | 2026-10-31 |
| MOD-030C Analytics cohort | Authenticated hosted run 15/15 with zero skips; Analytics mixed pilot and four-cohort MVC rollback passed across 12 authorized routes; Settings intentionally remained MVC; Release clean, 1,381 backend checks, 214/214 Angular tests, 121 templates and bundle budgets passed | 2026-10-31 |
| MOD-030D Settings cohort | Authorized hosted run 15/15 with zero failures/skips; five-cohort Angular cutover and process-local MVC rollback passed across 29 routes; Release clean, 1,385 backend checks, 214/214 Angular tests, 121 templates and bundle budgets passed | 2026-10-31 |
| MOD-030E1 read-only inventory cohort | Inventory balances, reservations, insights and reconciliation: hosted 15/15; 33-route Angular cutover and MVC rollback; Release clean, 1,394 backend checks, 214/214 Angular tests, 121 templates and bundle budgets passed | 2026-10-31 |
| MOD-030E2A inventory ledger/stock cohort | Hosted 15/15 with rendered Angular/API stock equality; 38-route Angular cutover and MVC rollback; Release clean, 1,406 backend checks, 215/215 Angular tests, 121 templates and bundle budgets passed | 2026-10-31 |
| MOD-030E2B inventory-adjustment cohort | Hosted 15/15; 40-route Angular cutover and MVC rollback; above-Int32 Details route covered without mutating Demo data; Release clean, 1,413 backend checks, 215/215 Angular tests, 121 templates and bundle budgets passed | 2026-11-01 |
| MOD-030E2C stock-transfer cohort | Hosted 15/15; 42-route Angular cutover and MVC rollback; exact Details/Edit routes covered without creating a missing Demo fixture; Release clean, 1,423 backend checks, 215/215 Angular tests, 121 templates and bundle budgets passed | 2026-11-01 |
| MOD-030E2D inventory-tracking cohort | Hosted 15/15; 43-route Angular cutover and MVC rollback; Index-only presentation mapping with every tracking mutation excluded; Release clean, 1,427 backend checks, 215/215 Angular tests, 121 templates and bundle budgets passed | 2026-11-01 |
| MOD-030E2E stock-movements cohort | Hosted 15/15; 44-route Angular cutover and MVC rollback; read-only Index mapped to the shared location-movements workspace; Release clean, 1,430 backend checks, 215/215 Angular tests, 121 templates and bundle budgets passed | 2026-11-01 |
| MOD-030E2F warehouse-operations cohort | Four Index GET surfaces batched; hosted 15/15; 48-route Angular cutover and MVC rollback; unsupported WarehouseLocations Create/Edit deep links retained on MVC; Release clean, 1,440 backend checks, 215/215 Angular tests, 121 templates and bundle budgets passed | 2026-11-01 |
| MOD-030E2G Inventory Replenishment | Dedicated Application-backed API and `/inventory/replenishment` workspace; compound permissions and antiforgery proven; hosted 15/15 and 49-route cutover/MVC rollback passed; Release clean, 1,447 backend checks, 218/218 Angular tests across 108 files, 122 templates and bundle budgets passed | 2026-11-01 |
| MOD-030E3A shared catalog masters | Branches, PaymentMethods and TaxRates exact Index GET actions share `/products/catalog-masters`; writes excluded; hosted 15/15 and 52-route cutover/MVC rollback passed; Release clean, 1,455 backend checks, 218/218 Angular tests across 108 files, 122 templates and bundle budgets passed | 2026-11-01 |
| MOD-030E3B product catalog definitions | ProductCategories Index plus ProductAttributes Index/positive-product Values GET use exact Angular routes; query-bound productId handling and write exclusions proven; hosted 15/15 and 55-route cutover/MVC rollback passed; Release clean, 1,465 backend checks, 218/218 Angular tests across 108 files, 122 templates and bundle budgets passed | 2026-11-01 |
| MOD-030E3C Warehouses Index | Exact Index GET maps to `/warehouses`; unsupported Create/Edit deep links and all writes excluded; hosted 15/15 and 56-route cutover/MVC rollback passed; Release clean, 1,469 backend checks, 218/218 Angular tests across 108 files, 122 templates and bundle budgets passed | 2026-11-01 |
| MOD-030E3D tenant AuditEvents | Exact Index GET maps to `/audit-events`; export and Platform audit excluded; hosted 15/15 and 57-route cutover/MVC rollback passed; Release clean, 1,473 backend checks, 218/218 Angular tests across 108 files, 122 templates and bundle budgets passed | 2026-11-01 |
| MOD-030E3E Users and Roles | Exact Users Index/Create and Roles Index/Create/Edit GET routes; mutations excluded; hosted 15/15 and 62-route cutover/MVC rollback passed; Release clean and 1,485 backend checks passed without test identity creation | 2026-11-01 |
| MOD-030E3F Product Templates and Recipes | Exact ProductTemplates Index and Recipes Index/positive-product Edit GET routes; mutations excluded; hosted 15/15 and 65-route cutover/MVC rollback passed; Release clean and 1,493 backend checks passed without data creation | 2026-11-01 |
| MOD-030E4A Purchase Analytics | Exact read-only Index GET maps to `/purchases/analytics` with query preservation and no purchase commands; hosted 15/15 and 66-route cutover/MVC rollback passed; Release clean and 1,496 backend checks passed | 2026-11-01 |
| MOD-030E4B Purchase Accounting Reconciliation | Exact read-only Index GET maps to `/purchases/accounting-reconciliation` with as-of/exception query preservation and no financial commands; hosted 15/15 and 67-route cutover/MVC rollback passed; Release clean and 1,499 backend checks passed | 2026-11-01 |
| MOD-030E4C Purchase Replenishment | Exact Index GET maps to `/purchases/replenishment`; settings-save/manual-run commands stay independently permissioned and antiforgery protected; hosted 15/15 and 68-route cutover/MVC rollback passed; Release clean and 1,502 backend checks passed | 2026-11-01 |
| MOD-030E4D Purchase Approval Rules | Exact Index GET maps to `/purchases/approval-rules`; create/status commands stay Manage-permissioned and antiforgery protected; hosted 15/15 and 69-route cutover/MVC rollback passed; Release clean and 1,505 backend checks passed | 2026-11-01 |
| MOD-030E4E Purchase Templates | Exact Index GET maps to `/purchases/templates`; create/generate/status commands stay independently permissioned and antiforgery protected; hosted 15/15 and 70-route cutover/MVC rollback passed; Release clean and 1,508 backend checks passed | 2026-11-01 |
| MOD-030E4F Purchase Requests | Exact Index/Create/positive-ID Details GET routes; lifecycle/approval mutations excluded; hosted 15/15 and 73-route cutover/MVC rollback passed including a read-only existing request; Release clean and 1,516 backend checks passed without data creation | 2026-11-01 |
| MOD-030E4G Purchase Sourcing | Exact Index/positive-ID Details/Print GET routes; create/direct forms, attachments and mutations excluded; hosted 15/15 and 76-route cutover/MVC rollback passed including a read-only existing sourcing event; Release clean and 1,524 backend checks passed without data creation | 2026-11-02 |
| MOD-030E4H Supplier Quotations | Exact positive-sourcing-ID Create/Comparison GET routes with query identity consumed into the Angular path; Create/Award POST and mutations excluded; hosted 15/15 and 78-route cutover/MVC rollback passed using an existing sourcing event; Release clean and 1,531 backend checks passed without data creation | 2026-11-02 |
| MOD-030E4I Purchase Orders | Exact Index/positive-ID Details GET routes; create-from-award and lifecycle/fulfillment mutations excluded; hosted 15/15 and 80-route cutover/MVC rollback passed including a read-only existing order; Release clean and 1,537 backend checks passed without data creation | 2026-11-02 |
| MOD-030E4J Goods Receipts | Exact Index/query-bound Create/positive-ID Details/Scan GET routes with positive-order validation; receipt/scan/inspection/posting/reversal mutations excluded; hosted 15/15 and 84-route cutover/MVC rollback passed using existing data; Release clean and 1,549 backend checks passed without data creation | 2026-11-02 |
| MOD-030E4K Goods Receipt Returns | Exact Index/query-bound Create/positive-ID Details GET routes; Create POST and mutations excluded; hosted 15/15 and 86-route cutover/MVC rollback passed with an existing receipt for Create, while no saved return existed for live Details; Release clean and 1,558 backend checks passed without data creation | 2026-11-02 |
| MOD-030E4L Vendor Bills | Exact Index/query-bound Create/positive-ID Details GET routes; attachments, extraction and lifecycle/accounting commands excluded; hosted 15/15 and 89-route cutover/MVC rollback passed including a read-only existing bill; Release clean and 1,568 backend checks passed without data creation | 2026-11-02 |
| Live schema integrity (سلامة المخطط الحي) | 147 tables, 420 trusted FKs, zero disabled constraints/indexes or unsafe tenant relations | 2026-10-15 |
| Web dependency build (بناء الويب واعتماداته) | Release, 4 projects, zero warnings/errors after SAL-120A (نجح) | 2026-10-22 |

اختبار `SQL` (قاعدة البيانات) غطى: إيقاف الكاتب القديم مع استمرار الحدث الدلالي، إعادة تشغيله وإضافة صف مستقبلي واحد فقط، أدوار الكاتب/القارئ/الصيانة، الحجز القانوني، نقطة التثبيت والبيان السابق للحذف، إضافة إلى إنشاء/تغيير صلاحيات/حذف الدور وتغيير إعداد حساس ورجوع آمن، عزل الشركات، المصالحة، الأرصدة، تزامن التزويد، تقسيم الجوائز،
الاستلام، فاتورة المورد، دفع المورد، العكس، المطابقة، GRNI (بضاعة مستلمة غير مفوترة)، المرتجع، التكلفة
الإضافية، التحليل، المسح، الجرد، التتبع، الحجر والاستدعاء، إضافة إلى عزل رمز الزبون، الأرشفة وتعارض rowversion، وحجز/تسليم طلب البيع، والفوترة الجزئية المتعددة، وترحيل الذمة/الإيراد حسب الفرع والمستودع وعكس القيد دون تغيير OnHand.

## التغطية حسب المجال

| المجال | الموجود | الحكم |
|---|---|---|
| Domain invariants (قواعد المجال) | فحوص تنفيذية كثيرة داخل SecurityRegression | قوي لكن غير مفصول إلى وحدات |
| Permissions/Security (الصلاحيات/الأمان) | انعكاس attributes ومسارات وتصعيد وعزل | قوي داخل النطاق |
| SQL concurrency (تزامن قاعدة البيانات) | مخزون/طلب/استلام/فاتورة/دفع/عكس | قوي للمشتريات والمخزون |
| Accounting integrity (سلامة المحاسبة) | توازن ومصالحة وفترات ومفاتيح مصدر | قوي للمسارات المدعومة |
| UI/E2E (واجهة/طرف لطرف) | لا suite آلية | مفقود |
| Performance/load (الأداء/التحميل) | حمل توفر محلي: 500 طلب، p95 16.85ms وp99 28.35ms، صفر فشل | مثبت محليًا؛ حمل الأعمال/الإنتاج متبقٍ |
| Backup/restore (نسخ/استعادة) | نسخة Demo مشفرة، تحقق، استعادة للقراءة فقط، DBCC ومصالحة ثم تنظيف آمن | مثبت محليًا؛ خارج الجهاز مفقود |
| Penetration (اختراق) | لا اختبار مستقل | مفقود |
| Sales professional lifecycle (دورة المبيعات الاحترافية) | SAL-030 quotations and SAL-040 one-time order conversion/copied snapshots/status history implemented; reservation onward planned | قوي للعروض والطلبات؛ واجهة E2E وتشغيل SQL المحلي معلقان |
| Customer master (ملف الزبون) | CUS-010/020/030A/030B: المجال والصلاحيات/antiforgery وعزل SQL وتفرد الكود والافتراضيات والأرشفة وrowversion وسياسة الحساب وإعدادات الدفع/العملة/الضريبة وترحيل Demo | قوي للشرائح المنفذة؛ واجهة E2E ما زالت عامة مفقودة |
| Sales pricing (تسعير المبيعات) | SAL-020B plus SAL-030 snapshot consumption: deterministic rules, managed units, dated currency, inclusive/exclusive tax net/gross, permissions/antiforgery and setup links | قوي للأساس والعروض؛ اختبار واجهة E2E ما زال مفقودًا |
| Audit events (أحداث التدقيق) | 37 عنصر تغطية، دلالة وسجلات مجال، عزل، قراءة/تصدير، احتفاظ، حجز، نقطتا تحويل، أدوار SQL وتجميد قابل للرجوع | قوي لبيئة Demo؛ تشغيل الإنتاج الخارجي متبقٍ |

## قواعد الحالة

- لا تعني نتيجة 777 أن كل ERP مختبر؛ إنها عدد فحوص برنامج مخصص واحد.
- لا يعد build (البناء) اختبار سلوك.
- كل مهمة جديدة تضيف اختبار مجال + قاعدة بيانات/أمان حسب الخطر، وتعيد الاختبارات المناسبة.
- المهمة `COMPLETE` (مكتملة) فقط إذا سجل الأمر والنتيجة والتاريخ؛ التعذر يعني `UNVERIFIED` (غير متحقق)، لا نجاحًا.
