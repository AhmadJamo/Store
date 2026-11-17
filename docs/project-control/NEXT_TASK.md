# المهمة التالية الوحيدة / Next Task

## TASK-REC-004 — توحيد ProblemDetails وتعارضات الأعمال

> الحالة: IN PROGRESS — REC-004A وREC-004B1 مكتملتان

اجرد أنماط معالجة الأخطاء في Internal API، وحدد InvalidOperationException التي تمثل validation أو duplicate أو not-found أو conflict، ثم وحّد الاستجابة 400/404/409/500 مع correlationId ورسالة محلية قابلة للفهم وحفظ نموذج Angular. ابدأ بالمسارات عالية المخاطر التي كشفتها مصفوفة REC-003، ولا تغيّر قواعد العمل نفسها.

## المنجز في REC-004B1

- أضيف كود conflict الثابت لتعارضات الأعمال 409 غير المرتبطة بـrowversion.
- بقيت concurrency حصرية للتعارض المتزامن الصريح، وبقي validation على 400.
- التالي: جرد بقية InvalidOperationException وتحويل الحالات المؤكدة إلى أنواع/استجابات صريحة دون مطابقة رسائل هشة.
