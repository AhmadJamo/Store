# المهمة التالية الوحيدة / Next Task

## `UX-003` — بنية الصفحات ومناطق الإجراءات / Page Architecture and Action Zones

> الحالة: APPROVED

اكتملت `UX-001` و`UX-002`: التنقل الهرمي وعقد حفظ السياق لخمس صفحات تجريبية. الخطوة الحالية توحّد تركيب
الصفحات ومناطق البحث والإجراءات دون تغيير قواعد الأعمال.

### النطاق المحدود

- توسيع page header بمناطق واضحة للعنوان والسياق والإجراء الأساسي والثانوي.
- إنشاء filter bar وsummary strip وsection header وsticky workflow footer قابلة لإعادة الاستخدام.
- Pilot على Products وPurchase Requests وStock Transfers ثم تعميم محدود على الصفحات المثبتة فقط.
- استخدام logical CSS وموارد الترجمة العربية/الإنجليزية.

### القبول

- تقليل تكرار أنماط filters/actions دون إخفاء صلاحية أو عملية.
- نجاح desktop/mobile وRTL/LTR ولوحة المفاتيح والوصولية.
- بقاء الوظائف والصلاحيات وdirty guards الحالية سليمة.
- تحديث `UI_UX_MODERNIZATION_PLAN.md` بنتائج الاختبارات والمخاطر والمتبقي.

### خارج النطاق

- إعادة تصميم enterprise table الشامل؛ مكانه `UX-004`.
- تغيير API أو business rule أو workflow status أو صلاحية.
- حذف MVC أو مسارات rollback.
