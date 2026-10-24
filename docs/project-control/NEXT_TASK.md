# المهمة التالية الوحيدة / Next Task

## `MOD-005` — Customers Angular vertical slice

> الحالة: PROPOSED — AWAITING EXPLICIT APPROVAL

### آخر شريحة منجزة: MOD-004

أُنشئ محرك Workspace مركزي مبني على metadata والصلاحيات، مع هوية تبويب مستقرة، منع التكرار، تاريخ مستقل،
إغلاق وإعادة فتح واستعادة آمنة وحماية للتغييرات غير المحفوظة. نجح Angular build دون تحذيرات و12/12 tests،
ونجح 1072 backend checks وبقي production npm audit دون ثغرات.

### الهدف المقترح التالي

تنفيذ أول vertical slice فعلي للزبائن على Angular مع إبقاء شاشة MVC الحالية كمسار رجوع:

- عقود API DTO صريحة عبر Application layer للقائمة والتفاصيل والإنشاء والتعديل؛
- بحث وفرز وتقسيم صفحات خادمي bounded مع صلاحيات وعزل الشركة؛
- قائمة وتفاصيل ونموذج إنشاء/تعديل ثنائي اللغة ومتجاوب؛
- إبقاء CustomerCode المولد وإعدادات الذمم/الضريبة الاختيارية محكومة بالخلفية؛
- عرض validation/ProblemDetails عند الحقول وملخص واضح؛
- optimistic concurrency مع رسالة استعادة/إعادة تحميل مفهومة؛
- ربط الشاشة بمحرك Workspace وdirty guard؛
- اختبارات API وAngular وحالات الحفظ الناجح والفاشل.

### حدود التنفيذ

- لا حذف أو تعطيل شاشة MVC الحالية.
- لا تغيير قواعد Customer أو الترقيم أو الذمم أو الضرائب.
- لا حفظ نموذج الزبون أو أي payload أعمال في browser storage.
- لا قاعدة بيانات أو migration.
- لا generic CRUD framework؛ يعاد الاستخدام فقط عند ظهور حاجة فعلية.

### بوابة القبول

- list/search/paging/details/create/edit تعمل ضمن صلاحيات وعزل الشركة.
- رمز الزبون المولد لا يُطلب من المستخدم والحقول الاختيارية تبقى اختيارية.
- أخطاء validation والتزامن تظهر بوضوح ولا تختفي بصمت.
- dirty form لا يفقد إدخاله دون قرار واضح.
- API لا يعرض EF entities وMVC يبقى عاملًا دون تغير.
- Angular build/tests وAPI/backend regression تبقى ناجحة.
