# المهمة التالية الوحيدة / Next Task

## MOD-030T — تدقيق lifecycle قواعد الاعتماد / Approval-rule lifecycle audit

> الحالة: PROPOSED — NON-DESTRUCTIVE

اكتمل `MOD-030S`. الخطوة التالية تقارن PurchaseApprovalRules وSalesApprovalRules بعمق قبل أي تقاعد، لأن تفعيل
القواعد يغير مسار اعتماد المستندات فعليًا.

### النطاق المقترح

- مراجعة MVC/API/Angular وخدمات Application والكيانات والمستودعات والصلاحيات وrowversion.
- تتبع إنشاء القاعدة وتفعيلها وحلها عند Submit ومسار approve/reject دون تنفيذ mutation.
- مراجعة اختبارات القواعد الحالية وتحديد أي فجوة acceptance تمنع التقاعد.
- فحوص GET فقط واختيار cohort واحد إن ثبت الفصل الآمن.

### خارج النطاق

- لا حذف Controller أو View.
- لا إنشاء/تفعيل قاعدة ولا Submit/Approve/Reject.
- أي تقاعد فعلي لاحق يحتاج نطاق ملفات دقيقًا وموافقة مستقلة.
