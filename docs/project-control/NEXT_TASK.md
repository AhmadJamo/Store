# المهمة التالية الوحيدة / Next Task

## MOD-030V — دليل SQL لدورة PurchaseApprovalRules

> الحالة: PROPOSED — TEST EVIDENCE ONLY

اكتمل `MOD-030U`. قبل اقتراح حذف PurchaseApprovalRules، يجب إغلاق فجوة الدليل التي سجلها MOD-030T باختبار تكامل
SQL معزول يعمل على قاعدة الاختبار فقط.

### النطاق المقترح

- إضافة سيناريو SQL ينشئ قاعدة شراء في سياق الاختبار ويثبت warehouse/priority resolution وترتيب الخطوات.
- إثبات أن Submit يجمّد اسم القاعدة والأدوار، وأن approve/reject يتطلب الدور الصحيح ويحدث الطلب داخل transaction.
- إثبات rowversion عند تفعيل/تعطيل القاعدة وعدم إنشاء instance مكرر.
- تشغيل بوابات Release والأمان واختبارات SQL ذات الصلة وتحديث التوثيق.

### خارج النطاق

- لا تغيير في قاعدة التشغيل أو أي قاعدة اعتماد فعلية.
- لا حذف Controller أو View.
- أي تقاعد لاحق لـPurchaseApprovalRules يحتاج نطاقًا دقيقًا وموافقة مستقلة.
