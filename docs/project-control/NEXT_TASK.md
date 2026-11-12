# المهمة التالية الوحيدة / Next Task

## MOD-030I — تدقيق حد Public المجهول / Anonymous Public boundary audit

> الحالة: PROPOSED — AWAITING OWNER APPROVAL

اكتمل `MOD-030H` بحذف سطحي القراءة الماليين بعد إثبات API/Angular. بقي `Public` وحده ضمن R1، لكنه حد مجهول
مرتبط بالدخول والتسعير والتسجيل ولذلك لا يُعامل كصفحة أعمال داخلية.

### النطاق المقترح

- جرد `Public.Index` و`Public.Pricing` وروابط التسجيل والخطة والعودة وسلوك المستخدم المجهول.
- إثبات تكافؤ Angular والحفاظ على query/rate-limit/session semantics قبل اقتراح أي حذف.
- تحديد ما إذا كان يلزم redirect دائم محمي بسياسة anonymous-safe أو إبقاء حد MVC صغير.
- توثيق قرار rollback واختبارات Edge المجهولة بصورة مستقلة.

### خارج النطاق

- لا حذف في مرحلة التدقيق دون موافقة لاحقة صريحة.
- لا تعديل للمصادقة أو التسجيل أو checkout أو cookies أو rate limiting.
- لا أي سطح R2/R3/R4 أو shared Razor infrastructure.

النتيجة المطلوبة أولًا هي قرار موثق فقط؛ أي تقاعد فعلي لـPublic يحتاج موافقة مالك مستقلة جديدة.
