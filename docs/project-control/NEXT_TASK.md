# المهمة التالية الوحيدة / Next Task

## `MOD-030E1` — دفعة مخزون للقراءة فقط / Read-only Inventory Cutover

> الحالة: APPROVED

أظهر تدقيق الجاهزية أن 82 سطحًا لها Angular counterpart، لكن خمسة أسطح فقط و29 إجراء GET تعمل AngularDefault.
يبقى 77 سطحًا MVC افتراضيًا، لذلك أُجل حذف MVC وبدأت مرحلة إكمال القطع التشغيلي.

### المتطلب السابق

- InventoryBalances وInventoryReservations وInventoryInsights وInventoryReconciliation لكل منها Index GET فقط.
- تبقى صلاحيات backend وواجهات API هي السلطة، ولا يضاف أي تحويل mutation.

### الهدف بعد الموافقة

- إضافة خرائط GET الدقيقة للمسارات الأربعة وتفعيلها AngularDefault.
- توسيع مشغل القطع ليطلب الصلاحيات الأربع ويختبر query strings.
- تشغيل hosted acceptance والقطع والـprocess-local MVC rollback.
- إبقاء Controllers وRazor كاملة كرجوع.

### القبول

- كل GET مصرح يعيد التوجيه إلى هدف Angular المطابق ويحافظ على query string.
- rollback يعيد المسارات الأربعة إلى MVC 200.
- البوابة الكاملة تبقى خضراء دون حذف ملفات أو تغيير قاعدة البيانات.
