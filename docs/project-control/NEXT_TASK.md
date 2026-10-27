# المهمة التالية الوحيدة / Next Task

## `MOD-022C` — التوفر والحجوزات / Inventory availability and reservations

> الحالة: APPROVED — NEXT INCREMENT ضمن موجة المخزون

اكتمل `MOD-022A..B`: سجل الحركات وكل عمليات أرصدة المنتج الحالية والتسوية اليدوية تعمل في Angular.
نجحت 1210 فحوص backend و155/155 Angular عبر 54 ملفاً و62 قالب وصولية، وبقي MVC افتراضياً.

### الهدف

- نقل قائمة التوفر البُعدي الحالية من `InventoryBalances` إلى Angular مع البحث والمستودع والموقع والتصفح.
- نقل سجل الحجوزات الحالي من `InventoryReservations` إلى Angular مع المرشحات والتصفح والأسماء بدل المعرّفات.
- إبقاء `OnHand` و`Reserved` و`Available` ومصدر الحجز وحماية التزامن وعزل الشركة داخل Application/Domain والخادم.
- استخدام DTOs صريحة وصلاحيتي `InventoryBalances.View` و`InventoryReservations.View` وحدود الشركة وProblem Details.
- توفير العربية والإنجليزية وRTL وحالات التحميل/الفراغ/الخطأ وروابط المصدر المصرح بها.

### القبول

- شاشتا التوفر والحجوزات الحاليتان تعملان وتُختبران في Angular عبر Application layer دون أي mutation جديد.
- تظهر أسماء المنتجات والمستودعات والمواقع ومراجع المصادر للمستخدم وتبقى المعرّفات داخل عقود النقل.
- لا تتغير قاعدة البيانات أو قواعد الرصيد والحجز والتتبع والتكلفة أو الترحيل المحاسبي.
- Release وSecurityRegression وAngular tests/build والوصولية وexternal SCSS والميزانية تنجح.
- تحديث `PHASE_22_INVENTORY.md` وبقاء MVC افتراضياً حتى cutover مستقل.
