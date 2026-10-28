# المهمة التالية الوحيدة / Next Task

## `MOD-023R` — اتصالات الموردين / Supplier communications

> الحالة: APPROVED — NEXT INCREMENT ضمن موجة المشتريات

اكتمل `MOD-023A..Q`: المسار حتى المطابقة المحاسبية يعمل في Angular، وبقي MVC افتراضياً.

### الهدف

- نقل تجهيز رسالة المورد من مستند الشراء، تفاصيلها ومرفقاتها وتسجيل حالة الإرسال/النشاط.
- احترام صلاحيات `SupplierCommunications.Prepare/View/RecordActivity` وCSRF وProblem Details وعزل الشركة.
- إبقاء أهلية المستند، اللقطات والمرفقات وسجل النشاط داخل Application layer والخادم.

### القبول

- كل عمليات Supplier Communications الحالية ضمن النطاق تعمل وتُختبر في Angular عبر Application layer.
- لا يرسل Angular رسالة خارجية ضمنيًا ولا يغيّر مستند المصدر؛ يسجل المستخدم النتيجة الفعلية صراحةً.
- Release وSecurityRegression وAngular tests/build والوصولية وexternal SCSS والميزانية تنجح.
- تحديث توثيق المرحلة وبقاء MVC افتراضياً حتى cutover مستقل.
