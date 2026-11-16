# سجل تنفيذ برنامج Recovery

## 2026-11-16 — TASK-REC-000

- العمل: قرئ Master Prompt V3.0، فُحص Git والحل والمشاريع والمسارات ومصادر الفروع والمستودعات والأخطاء، وأُنشئ مركز التحكم العربي.
- التغييرات: وثائق project-control فقط؛ لا runtime/schema.
- النتائج: 4 مشاريع إنتاجية، 4 مشاريع اختبار، 196 route، 148 migration، 329 وثيقة داخل docs.
- المشاكل: `MiniStore.sln` فارغ؛ `MiniStore.slnx` صحيح. فرع CRUD غير مكتمل. Warehouse create موجود خلاف الافتراض الأولي.
- البناء/الاختبارات: baseline الأخير قبل هذه المهمة Release ناجح، SecurityRegression 1,649، Angular 250/250، production build ناجح. لم تعد هذه المهمة تشغيل الاختبارات لأنها توثيق فقط؛ ستعاد في REC-001.
- المتبقي: TASK-REC-001.
- الكومت: يضاف بعد التثبيت.

## 2026-11-17 — TASK-REC-001

- العمل: أُكمل lifecycle الفروع من شاشة `/branches`: إنشاء، تعديل، تفعيل وتعطيل داخل مكوّن feature واحد متماسك.
- الحدود: لم يُوسّع نموذج Branch بحقول قانونية أو عنوان أو منطقة زمنية، ولم يحدث تغيير schema.
- الحماية: صلاحيات مستقلة `Branches.Create` و`Branches.Edit` و`Branches.SetActive`، antiforgery، tenant-scoped Application service، DTOs بدل entities، وتدقيق دلالي قبل/بعد.
- تجربة المستخدم: نموذج ثنائي اللغة يحفظ القيم عند خطأ API ويركّز أول حقل غير صالح، مع SCSS خارجي متوافق مع بوابة التحديث.
- التحقق: Web Release صفر أخطاء/تحذيرات؛ SecurityRegression 1,650؛ Angular 250/250؛ production build ناجح؛ أكبر lazy chunk 81,895 من 81,920 بايت.
- قيد الدليل: بوابة المتصفح العامة السابقة توقفت عند selector قديم `#preview-title`؛ لذلك لا يُسجّل نجاح E2E للفروع في هذه المهمة.
- التالي: TASK-REC-002.
## 2026-11-17 — TASK-REC-002

- التدقيق: create/edit وربط branch/account موجودة ومكتملة في `/warehouses`; الفجوة الحقيقية كانت معالجة رفض حذف مستودع مرتبط.
- الإصلاح: ترجمة DbUpdateException الخاصة بحذف Warehouse إلى رسالة أعمال ثابتة، وإرجاع ProblemDetails بحالة 409 مع ترجمة عربية.
- الدليل: SQL integration استخدم قاعدة مؤقتة ومخطط migrations الفعلي؛ حفظ المستودع المرتبط وProductStock وحذف مستودع غير مستخدم.
- UX: اختبار Angular يثبت بقاء الصف وإظهار التفاصيل عند 409؛ لا إعادة تصميم غير لازمة.
- التحقق: Release 0/0، SQL integration Passed، Angular 251/251، SecurityRegression 1,650.
- التالي: TASK-REC-003.