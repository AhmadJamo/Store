# سجل الأخطاء والفجوات — V3.0

| ID | العنوان | الوحدة | الشدة | الحالة |
|---|---|---|---|---|
| BUG-REC-001 | إدارة الفرع غير متاحة من صفحة الفروع | Branches `/branches` | P1 | Fixed / Verified |
| BUG-REC-002 | ملف MiniStore.sln لا يحتوي مشاريع | Build | P2 | Confirmed |
| BUG-REC-003 | حالات ERP متعارضة في PROJECT_CONTEXT | Docs | P2 | Confirmed |
| BUG-REC-004 | سلامة حذف مستودع مرتبط غير مثبتة في Application | Warehouses | P1 | Fixed / Verified |
| BUG-REC-005 | تصنيف InvalidOperationException غير متسق بين Controllers والميدلوير | API Errors | P2 | Requires Inventory |

## BUG-REC-001

- إعادة الإنتاج: افتح `/app/branches` بصلاحيات الفروع؛ لا يوجد زر إضافة/تعديل/تعطيل.
- الحالي: الإنشاء موجود في `/api/v1/catalog-masters/branches` وصفحة Catalog Masters تحت `Administration.Access`.
- المتوقع: إدارة lifecycle من صفحة الفروع بصلاحيات دقيقة.
- الدليل: `branches-page.ts`, `CatalogMastersApiController.cs`, `BranchService.cs`, `PermissionDefinitions.cs`.
- التأثير: صعوبة إنشاء الفرع وفصل غير صحيح للصلاحيات؛ لا فساد بيانات مثبت.
- الحل: TASK-REC-001؛ أضيف lifecycle كامل وصلاحيات دقيقة وتدقيق دلالي واختبارات API/حماية.

## BUG-REC-004

- الدليل: `WarehouseService.DeleteAsync` يستدعي repository delete مباشرة دون فحص Application ظاهر.
- الفرضية: قيود FK قد تمنع الضرر، لكن يجب اختبار قاعدة حقيقية ورسالة API.
- لا يُعد خطأ مؤكدًا قبل إعادة الإنتاج؛ الحالة Requires Investigation.
