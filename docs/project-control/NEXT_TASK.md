# المهمة التالية الوحيدة / Next Task

## MOD-030L — تقاعد MVC لـAuditEvents / Tenant AuditEvents MVC retirement

> الحالة: PROPOSED — AWAITING OWNER APPROVAL

اكتمل `MOD-030K` دون حذف، واختار tenant AuditEvents كأصغر cohort تالٍ بعد إثبات Angular/API للقراءة والتصدير.

### النطاق المقترح

- حذف `AuditEventsController` و`Views/AuditEvents/Index.cshtml` فقط بعد موافقة صريحة.
- إبقاء `AuditEventReadService` و`AuditEventsApiController` وAngular والصلاحيات كما هي.
- حفظ GET/HEAD legacy تحت `AuditEvents.View` مع query إلى `/app/audit-events`.
- تقاعد POST MVC القديم للتصدير صراحة؛ لا redirect للـPOST، والـAngular يستخدم API محميًا بـCSRF.
- تشغيل Release/security/Angular وEdge read acceptance، مع إثبات صلاحيات View/Export/ViewSensitive دون إنشاء export للاختبار.

### خارج النطاق

- لا تنفيذ قبل موافقة مالك صريحة على حذف هذين الملفين وتقاعد MVC Export.
- لا تغيير Application service أو API أو audit semantics أو retention أو Platform audit.
- لا أي R2 آخر أو R3/R4/shared Razor infrastructure.

التدقيق الكامل في `docs/modernization/MOD_030K_MIXED_CONTROLLER_PRIORITY_AUDIT.md`؛ لا توجد موافقة حذف بعد.
