# تغطية الاختبارات / Test Coverage

## آخر خط أساس منفذ

| الأمر | النتيجة | التاريخ |
|---|---|---|
| `dotnet run --project tests/SecurityRegression/SecurityRegression.csproj -c Debug --no-build` | Passed 756 checks (نجح 756 فحصًا) | 2026-10-16 |
| `dotnet run --project tests/InventorySqlIntegration/InventorySqlIntegration.csproj -c Release --no-build` | Passed including semantic role/settings event count plus rollback/isolation/redaction/append-only/idempotency/integrity (نجح) | 2026-10-16 |
| Live local historic-credential negative test (اختبار رفض بيانات الدخول التاريخية) | Rejected after remediation (رُفضت بعد المعالجة) | 2026-10-15 |
| Live local replacement-admin positive test (اختبار دخول المدير البديل) | Passed with authorized redirect (نجح مع تحويل مصرح) | 2026-10-15 |
| Encrypted backup verify/isolated restore/DBCC (تحقق النسخة والاستعادة المعزولة وسلامة القاعدة) | Passed with AES-256 and checksum (نجح) | 2026-10-15 |
| Restored ERP reconciliation (مصالحة النظام المستعاد) | 147 tables and 21 business checks; zero mismatches (صفر فروق) | 2026-10-15 |
| EF/database migration reconciliation (مطابقة الترحيلات) | P0 baseline 112/112; current Demo 113/113 after additive AuditEvents migration, no pending model change | 2026-10-16 |
| Live schema integrity (سلامة المخطط الحي) | 147 tables, 420 trusted FKs, zero disabled constraints/indexes or unsafe tenant relations | 2026-10-15 |
| Release build (بناء الإصدار) | 4 projects, zero warnings/errors (نجح) | 2026-10-16 |

اختبار `SQL` (قاعدة البيانات) غطى: إنشاء/تغيير صلاحيات/حذف الدور وتغيير إعداد حساس مع 4 أحداث متوقعة ورجوع آمن، عزل الشركات، المصالحة، الأرصدة، تزامن التزويد، تقسيم الجوائز،
الاستلام، فاتورة المورد، دفع المورد، العكس، المطابقة، GRNI (بضاعة مستلمة غير مفوترة)، المرتجع، التكلفة
الإضافية، التحليل، المسح، الجرد، التتبع، الحجر والاستدعاء.

## التغطية حسب المجال

| المجال | الموجود | الحكم |
|---|---|---|
| Domain invariants (قواعد المجال) | فحوص تنفيذية كثيرة داخل SecurityRegression | قوي لكن غير مفصول إلى وحدات |
| Permissions/Security (الصلاحيات/الأمان) | انعكاس attributes ومسارات وتصعيد وعزل | قوي داخل النطاق |
| SQL concurrency (تزامن قاعدة البيانات) | مخزون/طلب/استلام/فاتورة/دفع/عكس | قوي للمشتريات والمخزون |
| Accounting integrity (سلامة المحاسبة) | توازن ومصالحة وفترات ومفاتيح مصدر | قوي للمسارات المدعومة |
| UI/E2E (واجهة/طرف لطرف) | لا suite آلية | مفقود |
| Performance/load (الأداء/التحميل) | لا workload مثبت | مفقود |
| Backup/restore (نسخ/استعادة) | نسخة Demo مشفرة، تحقق، استعادة للقراءة فقط، DBCC ومصالحة ثم تنظيف آمن | مثبت محليًا؛ خارج الجهاز مفقود |
| Penetration (اختراق) | لا اختبار مستقل | مفقود |
| Sales professional lifecycle (دورة المبيعات الاحترافية) | غير منفذة | غير منطبق الآن |
| Customer master (ملف الزبون) | تحقق بسيط فقط | ضعيف |
| Audit events (أحداث التدقيق) | مجال/نموذج + SQL rollback، شركتان، منصة، منع حذف/تكرار، تنقيح وبصمة | الأساس قوي؛ تغطية خدمات الأعمال لم تبدأ |

## قواعد الحالة

- لا تعني نتيجة 756 أن كل ERP مختبر؛ إنها عدد فحوص برنامج مخصص واحد.
- لا يعد build (البناء) اختبار سلوك.
- كل مهمة جديدة تضيف اختبار مجال + قاعدة بيانات/أمان حسب الخطر، وتعيد الاختبارات المناسبة.
- المهمة `COMPLETE` (مكتملة) فقط إذا سجل الأمر والنتيجة والتاريخ؛ التعذر يعني `UNVERIFIED` (غير متحقق)، لا نجاحًا.
