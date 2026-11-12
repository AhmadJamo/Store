# المهمة التالية الوحيدة / Next Task

## MOD-030G — أول دفعة تقاعد MVC للقراءة فقط / First read-only MVC retirement pilot

> الحالة: PROPOSED — AWAITING OWNER APPROVAL

اكتمل `MOD-030F` كجرد غير هدّام وصنّف كل 411 إجراء MVC و217 ملف Razor دون حذف. الدفعة التالية المقترحة
صغيرة ومحددة داخل المخزون فقط، لأنها لا تملك mutation أو download أو session dependency مكتشفًا.

### النطاق المقترح

- `InventoryBalances.Index` وView الخاصة به.
- `InventoryReservations.Index` وView الخاصة به.
- `InventoryInsights.Index` وView الخاصة به.
- `InventoryReconciliation.Index` وView الخاصة به.
- `StockMovements.Index` وView الخاصة به.
- تثبيت سلوك الروابط القديمة بعد إزالة العرض، وتحديث `PresentationCutover` بحيث لا يشير أي وضع رجوع إلى كود محذوف.
- بحث مراجع كامل، ثم Release/regression/Angular/browser acceptance وخطة رجوع نشر قابلة للتنفيذ.

### خارج النطاق

- أي mutation أو download أو shared Razor infrastructure.
- `JournalEntries` أو `PurchaseAccountingReconciliation` رغم تصنيفهما R1، بسبب المخاطر المالية الأعلى.
- `Public` بسبب كونه مدخلًا عامًا مجهول الجلسة.
- أي سطح R2/R3/R4، وأي حذف جماعي للـMVC.

لا يبدأ الحذف قبل موافقة مالك صريحة على هذه الملفات الخمسة وعلى سياسة الروابط القديمة والرجوع بعد إزالة MVC.
