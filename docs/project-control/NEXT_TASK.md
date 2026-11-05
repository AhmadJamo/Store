# المهمة التالية الوحيدة / Next Task

## `UX-004` — بنية جداول ERP / Enterprise Data-Table Infrastructure

> الحالة: APPROVED

اكتملت `UX-001..003`: التنقل، حفظ السياق، وبنية الصفحات ومناطق الإجراءات المشتركة. الخطوة الحالية تطور
`app-data-table` إلى بنية قابلة للتهيئة دون نقل paging أو filter أو sort من الخادم إلى المتصفح.

### النطاق المحدود

- حالات loading وempty وerror، رؤوس sticky، محاذاة الأنواع، خلايا الحالة، وإجراءات الصفوف.
- عقد typed لإعداد الأعمدة مع إبقاء الخلايا والأوامر التجارية داخل الوحدة.
- Pilot على Products وPurchase Requests وStock Transfers.
- تفضيلات أعمدة آمنة ومسموح بها فقط إذا أمكن دمجها دون تخزين بيانات أعمال.

### القبول

- بقاء paging/filter/sort الحالي server-side وعدم اختلاق قدرة API غير موجودة.
- نجاح desktop/mobile وRTL/LTR ولوحة المفاتيح والوصولية والجداول العريضة.
- بقاء روابط الصفوف والصلاحيات والحالات الحالية سليمة.
- تحديث `UI_UX_MODERNIZATION_PLAN.md` بنتائج الاختبارات والمخاطر والمتبقي.

### خارج النطاق

- تغيير API أو business rule أو workflow status أو صلاحية.
- اختيار متعدد أو bulk commands ما لم يكن لهما عقد backend مثبت.
- حذف MVC أو مسارات rollback.
