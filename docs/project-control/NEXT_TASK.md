# المهمة التالية الوحيدة / Next Task

## MOD-030H — تقاعد سطحي MVC ماليين للقراءة فقط / Financial read-only MVC retirement

> الحالة: PROPOSED — AWAITING OWNER APPROVAL

اكتمل `MOD-030G` بحذف خمسة أسطح مخزون R1 مع إبقاء روابطها القديمة محمية ومتجهة إلى Angular. بقي ضمن R1
ثلاثة أسطح فقط؛ المقترح التالي يعالج السطحين الماليين معًا ويؤجل Public كحد عام مستقل.

### النطاق المقترح

- `JournalEntries.Details` و`Views/JournalEntries/Details.cshtml`.
- `PurchaseAccountingReconciliation.Index` وView الخاصة به.
- تحويل صيغ الروابط القديمة إلى مسارات Angular الدقيقة مع نفس `JournalEntries.View` و
  `PurchaseAccountingReconciliation.View` وحفظ المعرف/query.
- إزالة مفاتيح cutover/configuration الخاصة بهما، وتثبيت Release/regression/Angular/Edge وdeployment rollback.

### خارج النطاق

- `Public` أو أي مسار anonymous/authentication.
- أي mutation أو download أو shared Razor infrastructure.
- أي سطح R2/R3/R4 أو تغيير في القيود والترحيل والمصالحة المالية الخادمية.

لا يبدأ الحذف قبل موافقة مالك صريحة بسبب حساسية روابط الدليل والمصالحة المالية.
