# المهمة التالية الوحيدة / Next Task

## `MOD-023Q` — مطابقة محاسبة المشتريات / Purchase accounting reconciliation

> الحالة: APPROVED — NEXT INCREMENT ضمن موجة المشتريات

اكتمل `MOD-023A..P`: المسار حتى تحليلات المشتريات يعمل في Angular، وبقي MVC افتراضياً.

### الهدف

- نقل شاشة مطابقة المخزون مع دفتر الأستاذ ومطابقة GRNI مع دفتر الأستاذ، مع بطاقات الأدلة وروابط المصدر والإعداد.
- احترام صلاحية `PurchaseAccountingReconciliation.View` وProblem Details وعزل الشركة.
- إبقاء كل التجميعات والفروقات وحالة المطابقة داخل Application layer والخادم؛ Angular يعرض الأدلة فقط.

### القبول

- كل حالات وأدلة Purchase Accounting Reconciliation الحالية ضمن النطاق تعمل وتُختبر في Angular عبر Application layer.
- لا يحسب Angular أرصدة أو فروقات موثوقة ولا يخلط بيانات الشركات أو يغير مستندات المصدر.
- Release وSecurityRegression وAngular tests/build والوصولية وexternal SCSS والميزانية تنجح.
- تحديث توثيق المرحلة وبقاء MVC افتراضياً حتى cutover مستقل.
