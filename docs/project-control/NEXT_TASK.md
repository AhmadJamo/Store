# المهمة التالية الوحيدة / Next Task

## `MOD-022F` — المواقع والحركة الداخلية / Warehouse locations and internal movement

> الحالة: APPROVED — NEXT INCREMENT ضمن موجة المخزون

اكتمل `MOD-022A..E`: السجل والأرصدة والتوفر والحجوزات والجرد والتتبع والحجر والاستدعاء تعمل في Angular.
نجحت 1219 فحوص backend و164/164 Angular عبر 61 ملفاً و68 قالب وصولية، وبقي MVC افتراضياً.

### الهدف

- نقل إدارة مواقع التخزين الحالية: القائمة والإنشاء والتعديل والتسلسل والباركود والقدرات التشغيلية.
- نقل شاشة Unassigned Stock الحالية مع اقتراح putaway، التنفيذ، وقواعد putaway وإدارة تفعيلها.
- نقل relocation داخل المستودع وسجل Location/Stock Movement المرتبط دون اختراع حركة جديدة.
- احترام صلاحيات WarehouseLocations وLocationMovements وStockMovements المستقلة وCSRF وحدود الشركة وProblem Details.
- إبقاء السعة وخصائص الاستلام/الالتقاط والتسلسل وidempotency والأرصدة البُعدية والحركة الفعلية داخل Application/Domain والخادم.

### القبول

- كل عمليات Warehouse Locations وUnassigned Stock وLocation Movements الحالية تعمل وتُختبر في Angular عبر Application layer.
- تظهر أسماء المستودعات والمواقع والمنتجات والباركود للمستخدم وتبقى المعرّفات داخل عقود النقل.
- لا تتغير قاعدة البيانات أو قواعد السعة والرصيد والتتبع والتكلفة خارج السلوك الحالي.
- Release وSecurityRegression وAngular tests/build والوصولية وexternal SCSS والميزانية تنجح.
- تحديث `PHASE_22_INVENTORY.md` وبقاء MVC افتراضياً حتى cutover مستقل.
