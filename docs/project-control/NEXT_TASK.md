# المهمة التالية الوحيدة / Next Task

## `MOD-017` — Governed report builder, delegation and financial preview

> الحالة: APPROVED — NEXT INCREMENT

### آخر حزمة منجزة: MOD-012..016

اكتملت شفافية المستندات، مركز العمل، ملفات مساحة العمل، استكشاف التحليلات ومراقبة العمال الحقيقيين.
نجح Web Release build وAngular production build دون تحذيرات، و55/55 اختبار Angular و1092 فحص backend،
دون migration أو تغيير قواعد الأعمال.

### الهدف التالي

إضافة أساس reusable لبناء التقارير وحفظ تعريفاتها ضمن مركز التحليلات الحالي، مع تفويض مضبوط ومعاينة مالية
مصدرها backend:

- كتالوج حقول ومقاييس محكوم يملكه كل Analytics provider بدل أسماء جداول أو أعمدة حرة؛
- تعريف تقرير شخصي أو مشترك يحدد provider والحقول والتجميع والفلاتر والترتيب ضمن حدود معلنة؛
- preview محدود يعيد DTOs من Application ولا ينفذ SQL أو تعبيرات يكتبها المستخدم؛
- تفويض عرض أو تشغيل تقرير لمستخدم/دور داخل الشركة بمدة اختيارية، دون نقل صلاحية المصدر؛
- financial preview يوضح العملة والفترة والمصدر والتقريب وتعريف كل رقم من backend؛
- واجهة Angular ثنائية اللغة وRTL/LTR وكل CSS في ملفات SCSS منفصلة.

### حدود التنفيذ

- لا raw SQL، ولا أسماء EF entities/tables/columns، ولا expression/script builder.
- التقرير لا يمنح صلاحية؛ التشغيل يعيد فحص صلاحية provider وحقول التقرير الحالية لكل مستخدم.
- لا scheduled delivery أو بريد أو ملف كامل قبل تصميم durable job/delivery مستقل.
- لا حساب مالي في Angular، ولا generic CRUD أو generic entity endpoint.
- لا schema change قبل مراجعة aggregate والعلاقات والفهارس والصلاحيات وقرار migration إضافي.
- MVC وAnalytics Center الحاليان يبقيان مسار الرجوع.

### بوابة القبول

- تقرير واحد على الأقل مثبت على مزودين حقيقيين دون ربط المحرك بمبيعات أو مشتريات.
- crafted field/filter/provider/delegation inputs مرفوضة خادميًا.
- preview محدود وله تعريف ومصدر وعملة/فترة واضحة ولا يكشف صفوفًا غير مصرح بها.
- delegation منتهي/ملغى أو فاقد لصلاحية المصدر لا يعمل.
- Angular/Web build والاختبارات الأمنية والانحدار تبقى ناجحة.
- التعقيد: عالٍ، ويبدأ بمراجعة تصميم قبل migration.
