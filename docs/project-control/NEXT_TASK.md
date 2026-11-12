# المهمة التالية الوحيدة / Next Task

## MOD-030K — تدقيق أولويات mixed controllers / Mixed-controller prioritization audit

> الحالة: PROPOSED — AWAITING OWNER APPROVAL

اكتمل `MOD-030J` وحُذف آخر سطح R1/R1-A. بقي 54 سطحًا R2 مختلطًا يجمع العرض مع mutation/download/session boundaries،
ولا يجوز حذف Views منها قبل فصل اعتمادياتها الفعلية.

### النطاق المقترح

- جرد R2 حسب عدد mutations/downloads/views والمخاطر والصلاحيات والمراجع المشتركة.
- اختيار دفعة صغيرة يمكن فصل presentation منها دون نقل business logic أو حذف commands.
- توثيق الملفات الدقيقة، API/Angular parity، legacy-link behavior وخطة rollback المقترحة.
- إخراج توصية فقط؛ كل حذف فعلي يحتاج موافقة مالك مستقلة جديدة.

### خارج النطاق

- لا حذف Controller أو View في مرحلة التدقيق.
- لا تغيير mutations أو downloads أو session/authentication أو business rules.
- لا R3/R4 ولا shared Razor infrastructure.

النتيجة المطلوبة تقرير أولوية موثق فقط؛ لا توجد موافقة حذف R2 حاليًا.
