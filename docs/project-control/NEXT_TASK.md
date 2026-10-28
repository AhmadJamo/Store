# المهمة التالية الوحيدة / Next Task

## `MOD-023S` — تكاملات الموردين / Supplier integrations

> الحالة: APPROVED — NEXT INCREMENT ضمن موجة المشتريات

اكتمل `MOD-023A..R`: المسار حتى اتصالات الموردين يعمل في Angular، وبقي MVC افتراضياً.

### الهدف

- نقل بيانات اعتماد التكامل، روابط البوابة الخاصة، طابور الإرسال الخارجي، إعادة المحاولة وتنزيل الحمولة الثابتة.
- احترام صلاحيات `SupplierIntegrations.View/Manage/Queue/Retry` وCSRF وProblem Details وعزل الشركة.
- إبقاء الأسرار محمية، الرموز لمرة واحدة، idempotency وسياسة المحاولة داخل Application layer والخادم.

### القبول

- كل عمليات Supplier Integrations الحالية ضمن النطاق تعمل وتُختبر في Angular عبر Application layer.
- لا يعرض Angular الأسرار المحمية بعد الإنشاء ولا يكرر إرسال مستند خارجي دون ضوابط الخادم.
- Release وSecurityRegression وAngular tests/build والوصولية وexternal SCSS والميزانية تنجح.
- تحديث توثيق المرحلة وبقاء MVC افتراضياً حتى cutover مستقل.
