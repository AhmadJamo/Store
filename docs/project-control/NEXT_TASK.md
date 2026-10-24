# المهمة التالية الوحيدة / Next Task

## `MOD-012` — Document journey, related records and Explain This Number

> الحالة: APPROVED — CONTINUOUS EXECUTION

### آخر شريحة منجزة: MOD-011

اكتمل عقد Problem Details ثابت ومركزي وتصنيف Angular ومكوّن recovery قابل لإعادة الاستخدام على Customers وSuppliers.
نجح Web Release build وAngular production build و52/52 اختبار Angular و1084 فحص backend دون تغيير قاعدة البيانات.

### الهدف التالي

إنشاء طبقة عرض عامة للأدلة المترابطة والشفافية دون إعادة حساب قواعد الأعمال في المتصفح:

- مكوّن document journey يعرض المصدر والحالة والخطوات أو المستندات المرتبطة زمنيًا؛
- related-records contract عام يدعم روابط مسموحة ومصنفة بدل روابط hard-coded داخل كل صفحة؛
- Explain This Number يعرض عناصر المبلغ ومصدرها وتقريبها وعملتها من دليل backend معتمد؛
- حالات loading وempty وerror وrecovery ثنائية اللغة ومتوافقة مع RTL/LTR ولوحة المفاتيح؛
- إثبات إعادة الاستخدام على مسارين حقيقيين يملكان evidence حاليًا، دون اختراع endpoint عام أو حساب بديل؛
- إخفاء كل سجل أو رابط أو إجراء لا تسمح به صلاحية المستخدم الحالية.

### حدود التنفيذ

- backend Application evidence هو المرجع الوحيد للحالة والعلاقات والأرقام.
- لا graph traversal حر، ولا generic entity endpoint، ولا استعلام EF من controller.
- لا إعادة حساب سعر أو ضريبة أو رصيد في Angular، ولا استنتاج علاقة من أرقام أو نصوص.
- الروابط allowlisted والـAPI يعيد التحقق من tenant والpermission لكل وجهة.
- MVC يبقى عاملاً، وكل CSS في ملفات SCSS منفصلة.

### بوابة القبول

- الرحلة والعلاقات والأرقام المعروضة تطابق دليل backend ويمكن تفسير مصدر كل قيمة.
- المكوّن reusable مثبت على استخدامين فعليين دون ربطه بوحدة واحدة.
- لا تعرض الواجهة رابطًا غير مصرح ولا stack trace أو معرّفًا داخليًا غير ضروري.
- RTL/LTR واللوحة والموبايل وحالات empty/error تعمل.
- Angular/Web build والاختبارات والانحدار تبقى ناجحة.
- التعقيد: عالٍ.
