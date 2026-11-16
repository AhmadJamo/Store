# خطة استعادة المشروع — V3.0

> الحالة: ACTIVE  
> آخر تحديث: 2026-11-16

## الهدف

استعادة مصدر حقيقة قابل للتتبع، تثبيت P0، ثم تطوير UX/POS والتقارير على شرائح عمودية مكتملة دون إعادة كتابة الوظائف السليمة.

## المراحل

| المرحلة | الحالة | المخرجات | معيار القبول |
|---|---|---|---|
| REC-000 التدقيق التأسيسي ومركز التحكم | Completed | Audit/Recovery/Tasks/Coverage/UX/Bugs/Execution/Test docs | أدلة مصدرية، baseline موثق، مهمة تالية واحدة |
| REC-001 إدارة الفروع | Planned | CRUD آمن ضمن النموذج الحالي، صلاحيات دقيقة، Angular UX | Create/edit/deactivate/list وإعادة تحميل ورفض غير المخول |
| REC-002 سلامة المستودعات | Discovered | اختبار حذف مرتبط ورسالة مفهومة وتحسين CTA إن لزم | لا حذف ضار، create/edit وربط branch متحقق |
| REC-003 جرد العمليات العابرة للطبقات | Planned | route/function matrix على دفعات | كل مسار مصنف بدليل لا بالظهور فقط |
| REC-004 اتساق الأخطاء | Planned | mapping موحد 400/401/403/404/409/500 | correlationId ورسالة قابلة للفهم وحفظ النموذج |
| UX-REC موجات UX | Planned | إصلاحات حسب الأدلة، لا redesign شكلي | تقليل clicks واختبارات RTL/LTR/keyboard |
| POS-REC audit & stabilization | Planned | مقارنة المتطلبات مع POS الحالي | لا حذف قبل تكافؤ business/API/audit |
| POS advanced capabilities | Deferred | policies/studio/devices/offline حسب ADRs | شرائح مستقلة، idempotency وسلامة مالية |
| Reporting & control center | Deferred | semantic projections/drill-down | لا SQL عشوائي ولا حساب مالي في المتصفح |

## ترتيب الاعتماديات

REC-000 → REC-001 → REC-002/003 → REC-004 → UX-REC → POS-REC. Offline وStudio والسياسات المتقدمة لا تبدأ قبل حسم نموذج device/register/legal entity وكتابة ADRs.

## المخاطر

- اتساع V3.0 أكبر من جلسة واحدة؛ التنفيذ سيكون على كومات صغيرة مكتملة.
- التوثيق التاريخي ضخم ومتكرر؛ لا يحذف بل يوسم ما تم تجاوزه.
- توسيع نموذج Branch قد يتطلب Migration وترحيل بيانات؛ مؤجل حتى اعتماد نموذج واضح.
- الاختبارات المتصفحية تحتاج جلسات مصادقة صالحة، ولا يُدّعى نجاحها دون تشغيل.

## آلية التحقق

Release build عبر `MiniStore.slnx` أو المشاريع الفعلية، SecurityRegression عند الصلاحيات، Angular tests/build، SQL integration عند البيانات، ثم Edge acceptance للمسارات عالية المخاطر. بعد كل مهمة: Task Register + Recovery Plan + Execution Log + Bug Register + الوثائق المعمارية المتأثرة.
