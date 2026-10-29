# المهمة التالية الوحيدة / Next Task

## `MOD-027A` — الاشتراك وcheckout / Tenant subscription and checkout

> الحالة: APPROVED — NEXT INCREMENT ضمن موجة SaaS والواجهات العامة

اكتمل `MOD-026A..B` لكل أسطح المحاسبة الحالية المتبقية، وبقي MVC افتراضياً.

### الهدف

- نقل ملخص اشتراك الشركة وخيارات الخطط الحالية إلى Angular.
- نقل checkout الحالي بما فيه رمز العرض وطريقة الدفع والحالة الناتجة.
- إبقاء صلاحية الخطة والسعر والعرض وحالة الاشتراك والدفع داخل خدمات SaaS.

### القبول

- تعمل كل عمليات `SubscriptionController` الحالية في Angular عبر Application layer.
- لا يثق الخادم بالسعر أو الخصم أو حالة الاشتراك المرسلة من المتصفح.
- mutation يحتفظ بالمصادقة وantiforgery والتحقق الخادمي.
- الترجمة العربية/الإنجليزية وRTL والوصولية مكتملة.
- تنجح Release وSecurityRegression وAngular tests/build وexternal SCSS والميزانية.
- تحديث سجل الترحيل وتوثيق `MOD-027` مع بقاء MVC افتراضياً.
