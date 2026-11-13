# المهمة التالية الوحيدة / Next Task

## MOD-030O — تقاعد MVC لـAnalyticsCenter / AnalyticsCenter MVC retirement

> الحالة: PROPOSED — AWAITING EXPLICIT DELETION APPROVAL

اكتمل `MOD-030N` دون mutation واختار AnalyticsCenter كمرشح تالٍ. هذه الخطوة هدّامة ولا يشملها تصريح التدقيق العام.

### النطاق المقترح بعد الموافقة الصريحة

- حذف `MiniStore.Web/Controllers/Analytics/AnalyticsCenterController.cs`.
- حذف `MiniStore.Web/Views/AnalyticsCenter/Index.cshtml` و`Create.cshtml`.
- تحويل GET/HEAD لـIndex إلى `/app/analytics` تحت `AnalyticsCenter.View` مع حفظ query.
- تحويل GET/HEAD لـCreate إلى `/app/analytics/builder` تحت `AnalyticsCenter.Customize`.
- تقاعد MVC `POST /AnalyticsCenter/Create` دون redirect؛ يبقى API workspace POST هو المسار الفعلي.
- إبقاء الخدمة والـAPI وAngular والمزودات والصلاحيات والتفويضات دون تغيير.

### بوابة التنفيذ

- Release/security/Angular والجرد المولد.
- فحوص GET حية وصلاحيات View/Customize/Manage ورفض POST القديم دون إنشاء workspace للاختبار.
- الرجوع بنشر الكومت السابق؛ لا تغيير قاعدة بيانات.

### خارج النطاق

- لا حذف قبل موافقة صريحة على الملفات الثلاثة وPOST القديم.
- لا تغيير Analytics business rules أو delegation أو providers.
- لا تقاعد LocationMovements أو PlatformAuditEvents.

التدقيق الكامل: `docs/modernization/MOD_030N_NEXT_MIXED_CANDIDATE_AUDIT.md`.
