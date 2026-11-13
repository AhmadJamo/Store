# المهمة التالية الوحيدة / Next Task

## MOD-030Q — تقاعد Recipes MVC / Recipes MVC retirement

> الحالة: PROPOSED — DESTRUCTIVE — EXPLICIT APPROVAL REQUIRED

اكتمل `MOD-030P` دون حذف. أثبت أن Recipes هو الحد التالي الأقل مخاطرة، لكن التقاعد الفعلي غير معتمد بعد.

### نطاق الحذف المقترح الدقيق

- حذف `MiniStore.Web/Controllers/Catalog/RecipesController.cs`.
- حذف `MiniStore.Web/Views/Recipes/Index.cshtml` و`Edit.cshtml`.
- تثبيت تحويل GET/HEAD القديمة إلى `/app/products/recipes` و`/app/products/recipes/{productId}` مع صلاحياتها.
- إبقاء Recipes API وRecipeService وDTOs والمستودعات وAngular وكل قواعد المجال.
- إثبات أن MVC POST القديم أصبح غير متاح وأن API PUT المحمي بـ antiforgery هو مسار الحفظ الوحيد.

### خارج النطاق

- لا تغيير لقاعدة البيانات أو قواعد الوصفة أو المخزون أو المحاسبة.
- لا تنفيذ حفظ وصفة أثناء القبول؛ فحوص GET فقط.
- لا بدء للحذف قبل موافقة المستخدم الصريحة على الملفات الثلاثة أعلاه.
