# المهمة التالية الوحيدة / Next Task

## `UX-006` — دمج سير العمل / Workflow Integration

> الحالة: APPROVED

اكتملت `UX-001..005`: التنقل، حفظ السياق، بنية الصفحات، الجداول، والنماذج/التفاصيل. الخطوة الحالية تضيف
عرضًا موحدًا لتقدم سير العمل والمتطلبات السابقة دون نقل قرار الانتقال أو قواعده من الخادم إلى Angular.

### النطاق المحدود

- مكوّن display-only لعرض المرحلة الحالية والمراحل المسجلة من الحالة التي يعيدها الخادم.
- إرشاد واضح للإجراء التالي والمتطلبات الناقصة فقط عندما توفرها العقود الحالية.
- Pilot على Stock Transfers وPurchase Requests وPurchase Orders وSales Orders.
- إعادة استخدام workflow footer الحالي مع إبقاء permission/state checks داخل الميزات والخادم.

### القبول

- عدم اختراع transition أو status أو prerequisite في Angular.
- نجاح كل إجراء حالي ومسار بديل ورابط next-step بنفس الصلاحيات الحالية.
- نجاح keyboard وRTL/LTR والموبايل وحالات loading/empty/error.
- تحديث `UI_UX_MODERNIZATION_PLAN.md` بنتائج الاختبارات والمخاطر والمتبقي.

### خارج النطاق

- تغيير DTO أو API أو business rule أو workflow transition أو صلاحية دون ضرورة مثبتة وموافقة مستقلة.
- تشغيل إجراءات تلقائيًا أو تجاوز تأكيد/سبب/antiforgery.
- tabs أو progress وهمي لا يستند إلى حالة خادمية.
- حذف MVC أو مسارات rollback.
