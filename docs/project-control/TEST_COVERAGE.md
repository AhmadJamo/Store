# تغطية الاختبارات / Test Coverage

## آخر خط أساس منفذ

| الأمر | النتيجة | التاريخ |
|---|---|---|
| `dotnet run --project tests/SecurityRegression/SecurityRegression.csproj -c Release --no-build` | Passed 798 checks (نجح 798 فحصًا)، تشمل جهات الاتصال والعناوين وصلاحياتها وعزلها | 2026-10-18 |
| `pwsh -NoProfile -File scripts/verify-local.ps1` | Build + 777 + SQL + 500-request workload passed | 2026-10-18 |
| `dotnet run --project tests/InventorySqlIntegration/InventorySqlIntegration.csproj --no-restore` | Passed full migrations, 37-control catalog, legacy freeze/rollback, semantic continuity, SQL roles, legal hold and signed retention evidence (نجح) | 2026-10-18 |
| Live local historic-credential negative test (اختبار رفض بيانات الدخول التاريخية) | Rejected after remediation (رُفضت بعد المعالجة) | 2026-10-15 |
| Live local replacement-admin positive test (اختبار دخول المدير البديل) | Passed with authorized redirect (نجح مع تحويل مصرح) | 2026-10-15 |
| Encrypted backup verify/isolated restore/DBCC (تحقق النسخة والاستعادة المعزولة وسلامة القاعدة) | Passed with AES-256 and checksum (نجح) | 2026-10-15 |
| Restored ERP reconciliation (مصالحة النظام المستعاد) | 147 tables and 21 business checks; zero mismatches (صفر فروق) | 2026-10-15 |
| EF/database migration reconciliation (مطابقة الترحيلات) | P0 baseline 112/112; current Demo 116/116 with no pending model change after CUS-020 | 2026-10-18 |
| Live schema integrity (سلامة المخطط الحي) | 147 tables, 420 trusted FKs, zero disabled constraints/indexes or unsafe tenant relations | 2026-10-15 |
| Web dependency build (بناء الويب واعتماداته) | 4 projects, zero warnings/errors (نجح) | 2026-10-18 |

اختبار `SQL` (قاعدة البيانات) غطى: إيقاف الكاتب القديم مع استمرار الحدث الدلالي، إعادة تشغيله وإضافة صف مستقبلي واحد فقط، أدوار الكاتب/القارئ/الصيانة، الحجز القانوني، نقطة التثبيت والبيان السابق للحذف، إضافة إلى إنشاء/تغيير صلاحيات/حذف الدور وتغيير إعداد حساس ورجوع آمن، عزل الشركات، المصالحة، الأرصدة، تزامن التزويد، تقسيم الجوائز،
الاستلام، فاتورة المورد، دفع المورد، العكس، المطابقة، GRNI (بضاعة مستلمة غير مفوترة)، المرتجع، التكلفة
الإضافية، التحليل، المسح، الجرد، التتبع، الحجر والاستدعاء، إضافة إلى عزل رمز الزبون، الأرشفة وتعارض rowversion.

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
| Sales professional lifecycle (دورة المبيعات الاحترافية) | غير منفذة | غير منطبق الآن |
| Customer master (ملف الزبون) | CUS-010/020: المجال والصلاحيات/antiforgery وعزل SQL وتفرد الكود والافتراضيات والأرشفة وrowversion وترحيل Demo | قوي للشريحتين؛ واجهة E2E ما زالت عامة مفقودة |
| Audit events (أحداث التدقيق) | 37 عنصر تغطية، دلالة وسجلات مجال، عزل، قراءة/تصدير، احتفاظ، حجز، نقطتا تحويل، أدوار SQL وتجميد قابل للرجوع | قوي لبيئة Demo؛ تشغيل الإنتاج الخارجي متبقٍ |

## قواعد الحالة

- لا تعني نتيجة 777 أن كل ERP مختبر؛ إنها عدد فحوص برنامج مخصص واحد.
- لا يعد build (البناء) اختبار سلوك.
- كل مهمة جديدة تضيف اختبار مجال + قاعدة بيانات/أمان حسب الخطر، وتعيد الاختبارات المناسبة.
- المهمة `COMPLETE` (مكتملة) فقط إذا سجل الأمر والنتيجة والتاريخ؛ التعذر يعني `UNVERIFIED` (غير متحقق)، لا نجاحًا.
