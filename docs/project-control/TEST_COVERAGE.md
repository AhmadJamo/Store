# تغطية الاختبارات / Test Coverage

## آخر خط أساس منفذ

أحدث بوابة كاملة قبل `MOD-029B` في 2026-10-30: Release نجح بلا تحذيرات أو أخطاء، وSecurityRegression نجح في
1,345 فحصًا، وAngular نجح في 214/214 اختبارًا عبر 106 ملفات. نجح فحص الوصول الثابت لـ121 قالبًا، ونجحت ميزانية
الإنتاج عند 326,764/358,400 بايت أولية و81,880/81,920 لأكبر lazy chunk. تشغيل Demo المصرح به نجح 9/9 بلا
تخطٍ للهوية/المالية/المخزون، و100/100 أداء عند p95 14.24 ms وp99 16.79 ms.

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
