# المهمة التالية الوحيدة / Next Task

## MOD-030M — تدقيق جاهزية PlatformAuditEvents / Platform audit retirement readiness audit

> الحالة: PROPOSED — AWAITING OWNER APPROVAL
> النوع: تدقيق غير هدّام؛ لا حذف ولا تغيير runtime

اكتمل `MOD-030L` وأغلق tenant AuditEvents. المرشح الأقل تعقيدًا التالي هو PlatformAuditEvents، لكنه يستخدم جلسة
Platform cookie وسياسات Owner منفصلة، ولا يجوز افتراض أن دليل جلسة tenant يغطيه.

### النطاق المقترح

- مراجعة Controller/View وPlatform API وAngular وسياسات View/Export وعزل cookie الفعلي.
- التحقق من مسارات GET والتصدير وCSRF والبيانات الحساسة والروابط الثابتة دون إنشاء تصدير أو تغيير بيانات.
- تحديد fixture آمن لجلسة Platform مصرح بها، أو توثيق العائق بدقة إن لم تتوفر.
- إصدار قرار readiness فقط مع قائمة الملفات والمسارات والرجوع المقترح للمرحلة اللاحقة.
- تحديث سجل التقاعد والخطة والتوثيق بنتيجة التدقيق.

### خارج النطاق

- لا حذف `PlatformAuditEventsController` أو View.
- لا تعديل مصادقة Platform أو cookies أو السياسات أو API أو audit retention.
- لا تقاعد لأي R2/R3/R4 إضافي.

أي تقاعد فعلي لاحق يحتاج موافقة مالك مستقلة بعد اكتمال هذا التدقيق.
