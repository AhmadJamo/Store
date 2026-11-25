# 07 — مصفوفة ترحيل الوحدات

> الحالة: Active planning matrix
> آخر تحديث: 2026-11-24
> القاعدة: لا تعتبر الوحدة Migrated قبل parity وظيفي وAPI/permission evidence واختبارات وvisual acceptance

## 1. ترتيب التنفيذ

| الموجة | النطاق | السبب | بوابة الخروج |
|---:|---|---|---|
| 0 | Shell + Design System | أساس مشترك؛ حالياً pilot | visual acceptance وtokens/primitives مستقرة |
| 1 | Products List/Form/Record | أقل مخاطرة مالية ويثبت archetypes الأساسية | CRUD/search/barcode/permissions/RTL/mobile |
| 2 | Contacts | selectors وعقود الأطراف لباقي المستندات | Customers/Suppliers parity |
| 3 | Branches + Warehouses + Access | سياق التشغيل والاختيار بالأسماء | branch guard وwarehouse naming |
| 4 | Inventory | يعتمد على products/branches/warehouses | stock/transfer/adjustment/tracking workflows |
| 5 | Purchases | يعتمد على masters والمخزون والأطراف | request-to-pay parity |
| 6 | Sales | يعتمد على catalog/contacts/inventory | quote-to-cash parity ودليل مالي |
| 7 | Accounting | أعلى حساسية؛ يحتاج مستندات upstream مستقرة | journal/report/posting evidence |
| 8 | Administration + Settings | توحيد الأسطح المتبقية دون كسر setup | users/roles/settings/document design parity |
| 9 | POS | workspace مستقل مع جلسات ونقد وتدقيق | POS E2E وsession/cash integrity |
| 10 | Analytics + Work Center + Exceptions | cross-module بعد استقرار العقود | safe drill-down والpermission evidence |

هذا ترتيب UX migration لا ترتيب بناء business capabilities؛ الخدمات الحالية تبقى authority طوال العمل.

## 2. حالة الوحدات

| Module | Research | Design | Implementation | API Integration | Tests | Status / next gate |
|---|---|---|---|---|---|---|
| Shell / Launcher | Verified | Pilot | First slices | Existing | 259 + static | In Progress — visual acceptance |
| Design System | Verified | Contract v1 | First slice | N/A | focused + build | In Progress — primitives |
| Products / Catalog | Audited | In Progress | FRD-F List pilot | Verified current APIs | 271 tests + Playwright 7/7 | List actions/empty states complete; density then Form/Record |
| Contacts | Audited | Planned | Legacy V3 | Verified | existing specs | Planned wave 2 |
| Branches / Warehouses | Audited | Planned | Legacy V3 | Verified | existing specs | Planned wave 3 |
| Inventory | Audited | Planned | Legacy V3 | Verified | existing specs | Planned wave 4 |
| Purchases | Audited | Planned | Legacy V3 | Verified | existing specs | Planned wave 5 |
| Sales | Audited | Record concept | Legacy V3 | Verified | existing specs | Planned wave 6 |
| Accounting | Audited | Planned | Legacy V3 | Verified | existing specs | Planned wave 7 |
| Administration / Settings | Audited | Planned | Legacy V3 | Verified | existing specs | Planned wave 8 |
| POS | Verified research | Workspace concept | Existing workspace | Verified current APIs | existing specs | Planned wave 9 |
| Analytics / Work Center | Audited | Planned | Existing | Verified | existing specs | Planned wave 10 |
| Public / Platform / Portal | Audited | Exception | Existing | Existing | existing specs | Exception review |

## 3. سجل الترحيل المطلوب لكل سطح

يضاف صف عند بدء أي صفحة فعلية:

| Old Route | Old Component | New Route | New Component | Actions preserved | Permission | API evidence | Tests | Cutover | Rollback |
|---|---|---|---|---|---|---|---|---|---|
| مثال فقط | — | — | — | — | — | — | — | Not Started | legacy remains |

لا يوضع صف Implemented قبل توثيق كل action موجود، بما فيه الإجراءات النادرة والـempty/error/concurrency states.

## 4. Products pilot

### الأسطح

- Products List.
- Product Create/Edit.
- Product Record/Details.
- Product lookup المستخدم من المستندات.
- Categories/Attributes/Templates/Recipes تبقى وجهات داخل التطبيق ولا تدخل pilot الأول كلها دفعة واحدة.

### الوظائف التي يجب الحفاظ عليها

- server search/paging/sorting الحالية.
- product-name وbarcode search.
- اختيار الأسماء مع ID داخلي.
- SalePrice المهيأ كسلطة المبيعات ما لم توجد قاعدة override صريحة.
- permissions لكل view/create/edit/delete/action.
- validation وduplicate/concurrency/recovery.
- dirty guard وdeep links وعودة list context.
- العربية/الإنجليزية وRTL/LTR والكثافتان والهاتف.

### معايير الخروج

1. List control bar موحد مع empty/loading/error/filtered-empty.
2. Form grouped وkeyboard-safe بلا hard-coded labels.
3. Record header/status/related evidence بلا metrics وهمية.
4. لا تغير API أو schema أو business rules في الشريحة البصرية.
5. كل الاختبارات الحالية والجديدة والبناء وفحوص static والقبول البصري ناجحة.
6. لا يحذف surface قديم قبل parity وrollback موثقين.

## 5. قواعد cutover

- migration additive أولاً؛ route الحالي يمكنه تحميل component الجديد خلف قرار محدود وقابل للعكس.
- لا تعديل route public أو tokenized ضمن migration داخلي.
- لا إزالة CSS/component قديم بمجرد توقف route واحد عن استخدامه؛ البحث عن كل references إلزامي.
- عند فشل parity يعود route للسطح السابق دون تغيير البيانات أو العقود.
- تحديث هذه المصفوفة و`12-IMPLEMENTATION-PROGRESS.md` بعد كل وحدة أو slice.
