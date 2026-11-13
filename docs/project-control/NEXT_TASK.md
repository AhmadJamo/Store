# المهمة التالية الوحيدة / Next Task

## MOD-030U — تقاعد SalesApprovalRules MVC

> الحالة: PROPOSED — APPROVAL REQUIRED

اكتمل `MOD-030T` كتدقيق غير هدّام. أثبت أن SalesApprovalRules يملك بديل Angular/API مستقلًا مع دليل SQL لدورة
التقييم والاعتماد، لكن هذا الاقتراح لا يسمح بالحذف قبل موافقة المالك على النطاق الدقيق أدناه.

### النطاق المقترح

- حذف `MiniStore.Web/Controllers/Sales/SalesApprovalRulesController.cs`.
- حذف `MiniStore.Web/Views/SalesApprovalRules/Index.cshtml`.
- نقل `SalesApprovalRules:Index` فقط إلى retired catalog على `/sales/approval-rules` بصلاحية `SalesApprovalRules.View`.
- إزالة AngularDefault والمطابقة النشطة لهذا السطح فقط.
- إبقاء API وAngular و`SalesOrderApprovalService` والكيانات والمستودع والصلاحيات وكل سلوك workflow كما هو.
- إثبات تحويل GET/HEAD القديم و405 لمساري Create/SetActive القديمين دون mutation.

### خارج النطاق

- لا تعديل أو إنشاء أو تفعيل قاعدة اعتماد.
- لا تأكيد أو اعتماد أو رفض Sales Order.
- لا حذف PurchaseApprovalRules أو أي Controller/View آخر.
