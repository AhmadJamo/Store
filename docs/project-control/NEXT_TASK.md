# المهمة التالية الوحيدة / Next Task

## MOD-030X — تدقيق واختيار دفعة ثلاثة Controllers

> الحالة: APPROVED FOR NON-DESTRUCTIVE AUDIT ONLY

### الهدف

راجع الجرد الحالي وحدد بالاسم ثلاثة Controllers فقط تشكل دفعة تقاعد واحدة متماسكة وآمنة. يجب فحص كل Controller مع
Views وAPI وAngular وApplication service والصلاحيات وantiforgery والمسارات القديمة وأي mutation/download/session behavior.

### المخرجات المطلوبة

- أسماء Controllers الثلاثة والملفات التابعة لكل منها.
- مسارات Angular/API البديلة ودليل التكافؤ لكل flow.
- تصنيف GET/HEAD وPOST/download وأي استثناء لا يجوز تحويله.
- مخاطر الأعمال والبيانات وخطة rollback واختبارات التنفيذ المقترحة.
- تحديث التوثيق باقتراح دفعة تنفيذ مستقلة؛ لا يبدأ الحذف ضمن هذا التدقيق.

### خارج النطاق

- لا حذف Controller أو View.
- لا تغيير runtime أو cutover catalog أو configuration.
- لا mutation لبيانات تشغيلية.
- لا توسيع الدفعة عن ثلاثة Controllers.
