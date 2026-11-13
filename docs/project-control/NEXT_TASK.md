# المهمة التالية الوحيدة / Next Task

## MOD-030S — تقاعد ProductCategories MVC / ProductCategories MVC retirement

> الحالة: PROPOSED — DESTRUCTIVE — EXPLICIT APPROVAL REQUIRED

اكتمل `MOD-030R` دون حذف. أثبت أن ProductCategories هو الحد التكويني التالي الأقل مخاطرة، لكن التقاعد غير معتمد بعد.

### نطاق الحذف المقترح الدقيق

- حذف `MiniStore.Web/Controllers/Catalog/ProductCategoriesController.cs`.
- حذف `MiniStore.Web/Views/ProductCategories/Index.cshtml`.
- تثبيت GET/HEAD القديمة إلى `/app/products/categories` مع `Products.View`.
- إبقاء ProductCategories API وProductCategoryService وDTOs والمستودع والكيان وAngular.
- إثبات أن Create/SetActive MVC POST يعيدان 405 دون mutation، مع بقاء API POST/PUT وantiforgery.

### خارج النطاق

- لا تغيير لبيانات التصنيفات أو المخزون أو المنتجات أو قاعدة البيانات.
- لا POST/PUT/DELETE أثناء القبول.
- لا بدء للحذف قبل موافقة المستخدم الصريحة على الملفين أعلاه.
