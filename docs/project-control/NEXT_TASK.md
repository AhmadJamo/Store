# المهمة التالية الوحيدة / Next Task

## MOD-030W — تقاعد PurchaseApprovalRules MVC

> الحالة: PROPOSED — APPROVAL REQUIRED

أغلق `MOD-030V` فجوة SQL لدورة اعتماد المشتريات، لكن حذف طبقة العرض القديمة يبقى قرارًا مستقلًا يحتاج موافقة المالك
على النطاق الدقيق أدناه.

### النطاق المقترح

- حذف `MiniStore.Web/Controllers/Purchases/PurchaseApprovalRulesController.cs`.
- حذف `MiniStore.Web/Views/PurchaseApprovalRules/Index.cshtml`.
- نقل `PurchaseApprovalRules:Index` فقط إلى retired catalog على `/purchases/approval-rules` بصلاحية
  `PurchaseApprovalRules.View`.
- إزالة AngularDefault والمطابقة النشطة لهذا السطح فقط.
- إبقاء API وAngular و`PurchaseApprovalService` والكيانات والمستودعات والصلاحيات وكل workflow كما هو.
- إثبات تحويل GET/HEAD القديم و405 لمساري Create/SetActive القديمين دون mutation.

### خارج النطاق

- لا تعديل أو إنشاء أو تفعيل قاعدة اعتماد تشغيلية.
- لا Submit أو Approve أو Reject لطلب شراء فعلي.
- لا حذف أي Controller/View آخر.
