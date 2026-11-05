# المهمة التالية الوحيدة / Next Task

## `UX-005` — النماذج وصفحات التفاصيل / Forms and Detail Pages

> الحالة: APPROVED

اكتملت `UX-001..004`: التنقل، حفظ السياق، بنية الصفحات والجداول typed. الخطوة الحالية توحّد تركيب النماذج
وإشارات المطلوب/الاختياري والتحقق وصفحات التفاصيل دون تغيير DTO أو قواعد الأعمال.

### النطاق المحدود

- مكونات field group وform section وإشارات required/optional ومساعدة/خطأ موحدة.
- بنية detail sections أو tabs فقط حيث توجد مجموعات معلومات حقيقية.
- Pilot على Product create/edit/details وPurchase Request وStock Transfer create/details.
- دمج validation summary وdirty guards وsubmission locking الحالية بدل تكرارها.

### القبول

- عدم فقدان أي حقل أو validation أو قيمة مرفوضة من الخادم.
- نجاح keyboard/focus وRTL/LTR والموبايل وإشارات المطلوب/الخطأ.
- بقاء dirty guards ومنع الإرسال المكرر والصلاحيات والعمليات الحالية سليمة.
- تحديث `UI_UX_MODERNIZATION_PLAN.md` بنتائج الاختبارات والمخاطر والمتبقي.

### خارج النطاق

- تغيير DTO أو API أو business rule أو workflow status أو صلاحية.
- حفظ حقول حساسة أو ملاحظات غير مقيدة.
- tabs شكلية لا تمثل مجموعات معلومات حقيقية.
- حذف MVC أو مسارات rollback.
