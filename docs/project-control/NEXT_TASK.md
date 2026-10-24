# المهمة التالية الوحيدة / Next Task

## `MOD-006` — Proven shared form and table primitives

> الحالة: PROPOSED — AWAITING EXPLICIT APPROVAL

### آخر شريحة منجزة: MOD-005

أُنجزت أول شريحة أعمال Angular للزبائن: قائمة خادمية، تفاصيل، إنشاء وتعديل عبر API داخلي محمي يستخدم
`CustomerService` نفسه، مع validation و`409` للتزامن وحماية dirty form. نجح Web/Angular build، و17/17 tests،
و1076 backend checks، وبقي MVC وقاعدة البيانات وقواعد الأعمال دون تغيير.

### الهدف المقترح التالي

استخراج المكونات المشتركة التي أثبتتها شاشة الزبائن والتحقق منها في مستهلك أعمال ثانٍ محدود:

- primitives صغيرة للعنوان، الفلاتر، حالة التحميل/الفراغ/الخطأ، الجدول وpager؛
- form error summary وfield error mapping قابلان لإعادة الاستخدام؛
- تجربة الاستخدام في شاشة أعمال ثانية قبل تعميم أي abstraction؛
- configuration مرنة للأعمدة والإجراءات دون تمرير صلاحيات كسلطة؛
- RTL/LTR، keyboard وresponsive behavior؛
- اختبارات للمكونات المشتركة وللمستهلكين الفعليين.

### حدود التنفيذ

- لا generic CRUD engine أو schema-driven business forms.
- لا نقل validation أو permissions أو paging authority إلى الواجهة.
- لا تغيير API أو business rules لمجرد ملاءمة component عام.
- لا قاعدة بيانات أو migration.
- CSS يبقى في ملفات SCSS منفصلة؛ لا inline CSS داخل HTML.

### بوابة القبول

- يستخدم كل primitive مستهلكان حقيقيان أو يبقى محليًا ولا يُعمم.
- التخصيص لا يضع business rules داخل component المشترك.
- RTL/LTR والوصولية والاستجابة تعمل دون inline styles.
- شاشة الزبائن لا تفقد أي سلوك أو validation أو حماية dirty state.
- Angular build/tests وbackend regression تبقى ناجحة.
