# المهمة التالية الوحيدة / Next Task

## MOD-030J — تقاعد Public المجهول / Anonymous Public physical retirement

> الحالة: PROPOSED — AWAITING OWNER APPROVAL

اكتمل `MOD-030I` كتدقيق دون حذف. ثبت تكافؤ Angular/API، لكن Public صُنّف R1-A لأن `/` و`/pricing` مساران
مجهولان صريحان، بينما كتالوج التقاعد الحالي يبني مسارات Controller/Action ويفرض صلاحية.

### النطاق المقترح

- إضافة عقد exact-route مجهول صريح لـ`/` و`/pricing` دون إضعاف التحويلات المحمية الحالية.
- حفظ GET/HEAD وquery إلى `/app/welcome` و`/app/pricing`.
- حذف `PublicController` وملفي `Views/Public` فقط بعد إثبات عدم وجود مراجع معلقة.
- تشغيل اختبارات Edge المجهولة لمسارات legacy والتسجيل والخطة، واعتماد deployment rollback.

### خارج النطاق

- لا تنفيذ أو حذف قبل موافقة مالك صريحة لهذه الدفعة.
- لا تعديل للمصادقة أو التسجيل أو checkout أو cookies أو rate limiting.
- لا حذف `_PublicLayout` أو أي سطح R2/R3/R4 أو shared Razor infrastructure.

القرار التفصيلي في `docs/modernization/MOD_030I_PUBLIC_BOUNDARY_AUDIT.md`؛ هذه المهمة ليست معتمدة للتنفيذ بعد.
