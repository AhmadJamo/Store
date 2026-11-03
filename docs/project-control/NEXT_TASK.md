# المهمة التالية الوحيدة / Next Task

## `MOD-030F` — جرد جاهزية تقاعد MVC / MVC Retirement Readiness Inventory

> الحالة: APPROVED

اكتمل runtime cutover: أسطح العرض الـ81 كلها AngularDefault، صفر فجوات عرض، وLanguage حد POST-only compatibility
وحيد. ملفات MVC ما زالت موجودة للرجوع ولم يُحذف شيء.

### النطاق المحدود

- جرد Controllers وRazor والروابط والاختبارات والتحميلات والأوامر التي تمنع حذفًا آمنًا.
- تقسيم التقاعد الفعلي إلى دفعات صغيرة قابلة للرجوع مع ترتيب الاعتماديات ودليل النشر المطلوب.
- تحديد الملفات التي يجب إبقاؤها لأوامر mutation/download/authentication/compatibility.

### القبول

- تقرير مولّد أو قابل للتحقق يميز مرشحي الحذف عن الملفات المشتركة والاستثناءات.
- كل دفعة مقترحة لها نطاق واختبارات ورجوع واضحان.
- لا حذف ولا تغيير مسارات ولا كسر rollback في هذه المهمة.

### خارج النطاق

- حذف Controllers أو Razor views.
- إزالة PresentationCutover أو مسارات MVC rollback.
- أي تغيير قاعدة بيانات أو business rule أو صلاحيات.
