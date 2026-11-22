# 04 — معمارية تجربة الاستخدام المستهدفة

> الحالة: Design contract
> المرجع: تدقيق النظام، بحث Odoo 19، ومصفوفة الخصائص
> النطاق: Angular ERP الداخلي؛ الشاشات العامة وPlatform وSupplier Portal استثناءات مستقلة

## 1. القرار التنفيذي

يعاد تنظيم MiniStore كمساحة عمل ERP ذات ثلاث طبقات فقط:

1. **Global shell** ثابت وخفيف للهوية، مبدّل التطبيقات، البحث العام، الفرع، اللغة، والتنبيهات/الحساب.
2. **Current app navigation** يعرض وجهات الوحدة الحالية فقط، لا قائمة عملاقة بكل النظام.
3. **Page control area** يضم عنوان السجل أو القائمة، البحث/الفلاتر، تبديل العرض، والإجراءات المرتبطة بالسياق.

لا تُبنى طبقة Tabs مرئية دائمة فوق الصفحات. تبقى خدمة Workspace الحالية مسؤولة عن الاستعادة، منع فقد التعديلات،
والسجل الآمن، ويعرض العمل المفتوح عند الحاجة من قائمة سياقية واحدة.

## 2. مبادئ التجربة

- **الوحدة قبل الصفحة:** شاشة التطبيقات تعرض الوحدات الرئيسية، ثم تكشف وجهات الوحدة بعد دخولها.
- **السياق قبل الخيارات:** لا تظهر أزرار أو عروض أو مجموعات لا تدعمها الصلاحية والعقود الفعلية.
- **المعلومة قبل الزخرفة:** الكثافة مناسبة لـERP؛ البطاقات تستخدم للهوية أو التجميع الحقيقي لا لكل حقل.
- **إجراء أساسي واحد:** لكل حالة صفحة إجراء أساسي واضح، وباقي الإجراءات ثانوية أو داخل قائمة.
- **الخادم هو السلطة:** الحالة، الانتقالات، الأرصدة، الأسعار، الأذونات، والعدادات تأتي من العقود الحالية.
- **التدرج لا الانفصال:** List → Record/Form → Related records يحافظ على سياق الوحدة والعودة.
- **ثنائي اللغة من الأصل:** كل بنية تعمل EN/LTR وAR/RTL باستخدام خصائص CSS منطقية.
- **الهاتف للعمل المحدود:** الهاتف يدعم البحث، المراجعة، والإجراءات القصيرة؛ الجداول الثقيلة تتحول لقائمة سجلات.

## 3. نموذج المعلومات

### 3.1 مستوى التطبيقات

التطبيقات الرئيسية المعتمدة:

| App | الغرض | أمثلة وجهات داخلية |
|---|---|---|
| Sales | من التسعير حتى التحصيل والمرتجعات | Quotations, Orders, Deliveries, Invoices, Payments |
| Purchases | من الطلب حتى الدفع والمطابقة | Requests, Orders, Receipts, Bills, Suppliers |
| Inventory | المخزون والتتبع والحركات | Stock, Transfers, Adjustments, Lots, Warehouses |
| Products | بيانات المنتج والتسعير الأساسية | Products, Categories, Attributes, Recipes |
| Contacts | العملاء والموردون | Customers, Suppliers |
| Accounting | الحسابات والقيود والتقارير | Accounts, Journals, Periods, Reports |
| POS | جلسة البيع الفوري | Register, Session, Orders, Returns |
| Analytics | التحليلات ومساحات العمل | Dashboards, Builder, Shared workspaces |
| Administration | المستخدمون والصلاحيات والفروع | Users, Roles, Branches, Access |
| Settings | إعدادات الشركة والمستندات والتكامل | General, Identity, Documents, Integrations |

ظهور App أو Destination تابع للصلاحيات. لا تعرض شاشة التطبيقات CRUD pages أو صفحات detail كبلاطات.

### 3.2 مستوى الوحدة

كل وحدة تملك:

- اسم وأيقونة وهوية لونية مساعدة لا تتغلب على هوية النظام.
- وجهة افتراضية واحدة آمنة.
- مجموعات وجهات بحد أقصى مستويين.
- Recent/Favorites داخل الوحدة إن وجدت بيانات فعلية.
- عدم تكرار نفس الوجهة في shell وpage body.

### 3.3 مستوى الصفحة

كل صفحة تنتمي إلى واحد من archetypes التالية:

| Archetype | الاستخدام | التركيب الإلزامي |
|---|---|---|
| List | مجموعات قابلة للبحث والتصفية | Control bar + saved search + view + results + paging |
| Record | قراءة سجل مع إجراءات وعلاقات | Record header + status + sheet + related evidence |
| Form | إنشاء/تحرير | Form header + grouped sections + sticky actions + dirty guard |
| Kanban | مراحل/حالات قابلة للمسح البصري | Group columns + record cards + bounded actions |
| Dashboard | مؤشرات وقرارات | Time/context controls + metrics + actionable exceptions |
| Workspace | عمليات مركبة مثل POS أو reconciliation | Task-specific layout دون فرض List/Form |
| Settings | إعدادات منظمة حسب مجال | Searchable sections + save scope + impact guidance |

## 4. Shell المستهدف

### 4.1 الشريط العالمي

- زر App Launcher.
- اسم التطبيق الحالي مع route back إلى hub.
- بحث/أوامر عام `Ctrl+K` مع نتائج مسموحة فقط.
- مبدّل الفرع الموثّق خادمياً، مع حارس dirty state قبل التغيير.
- اللغة/الاتجاه، قائمة العمل المفتوح، ثم حساب المستخدم.
- لا يزيد ارتفاع desktop المقصود عن 48–52px في الكثافة المريحة.

### 4.2 تنقل التطبيق

- Desktop: شريط أفقي ثانوي أو rail مضغوط بحسب عدد الوجهات والمساحة.
- Tablet: قائمة قابلة للفتح مرتبطة باسم التطبيق.
- Mobile: زر قائمة واحد مع drawer؛ لا تمرير أفقي لعشرات الروابط.
- الوجهة النشطة واضحة نصياً ولونياً ولا تعتمد على اللون وحده.

### 4.3 العمل المفتوح

تبقى `WorkspaceService` مسؤولة عن deduplication، history، restoration، وdirty state. تعرض الواجهة قائمة سياقية
بعدد العناصر المفتوحة، مع فتح/إغلاق/إغلاق الباقي. لا تتحول هذه القائمة إلى نظام تبويبات دائم يستهلك الارتفاع.

## 5. نمط List

ترتيب ثابت:

1. Breadcrumb مختصر عند الحاجة.
2. Title + count موثوق + primary create action.
3. Search bar مركزي؛ Filters وGroup By وFavorites في popovers منفصلة.
4. View switcher يظهر فقط للعروض المنفذة فعلياً.
5. Results toolbar للاختيار الجماعي والإجراءات المسموحة.
6. Table على desktop، record list على الهاتف.
7. Server paging ومعلومات النطاق.

قواعد:

- اختيار الأعمدة محلي/مستخدم فقط للأعمدة الآمنة المتاحة في DTO.
- لا فرز أو تجميع وهمي على بيانات الصفحة الحالية؛ العمليات server-side ما لم يذكر العقد غير ذلك.
- الفلاتر المحفوظة شخصية افتراضياً؛ المشاركة تحتاج backend contract قبل ظهورها.
- Bulk actions لا تظهر قبل الاختيار ولا تتجاوز صلاحيات كل عملية.
- empty state يفرق بين «لا بيانات» و«لا نتائج للفلاتر» و«لا صلاحية» و«فشل التحميل».

## 6. نمط Record/Form

### 6.1 Record header

- العنوان والرقم المرجعي والحالة.
- إجراء أساسي متاح الآن فقط.
- إجراءات ثانوية، ثم قائمة More للأفعال النادرة/الخطرة.
- Status pipeline للـworkflows الفعلية، لا stepper تجميلي.
- Smart links/metrics لا تظهر إلا بعد وجود count/route/permission موثوقة.

### 6.2 Sheet

- مجموعة هوية أولى: الاسم، الطرف، التاريخ، الفرع، العملة، المرجع.
- حقول في مجموعات منطقية، لا card لكل field.
- line editor للجداول التابعة مع keyboard flow واضح.
- related sections كـtabs داخل السجل فقط عندما تكون العلاقات حقيقية ومستخدمة.
- activity/evidence side panel يظهر فقط للكيانات ذات endpoint حقيقي؛ لا Chatter عام وهمي.

### 6.3 الأفعال والحفظ

- Create/Edit: Save أساسي وDiscard ثانوي؛ sticky على الشاشات الطويلة.
- عند النجاح يظهر next logical step المصرح به.
- الأفعال غير القابلة للتكرار تختفي بعد النجاح ويظهر دليل الحالة أو المستند الناتج.
- destructive actions تتطلب تسمية دقيقة للأثر وتأكيداً متناسباً مع الخطورة.

## 7. Kanban والأنشطة

- Kanban عرض بديل مشروط ببيانات المرحلة/المجموعة، وليس replacement عاماً للجداول.
- السحب والإفلات مستبعد افتراضياً؛ يُفعّل فقط إذا كان الانتقال مسموحاً ويدعمه API مع concurrency.
- البطاقة تعرض هوية السجل، 2–4 حقول حاسمة، status/activity، وإجراء سريع واحد كحد أقصى.
- Activities تعني التزامات مرتبطة بسجل وتحتاج عقد قراءة/إنشاء/إكمال. قبل ذلك تعرض أدلة workflow الحالية فقط.

## 8. Responsive وRTL

| النطاق | السلوك |
|---|---|
| ≥ 1200px | shell كامل، content density مريحة، side evidence اختياري |
| 768–1199px | app nav قابل للفتح، controls تلتف، الجداول تختصر أعمدتها |
| < 768px | top bar مختصر، drawer، record list، primary action مرئي والباقي في More |

- الاتجاه من locale على مستوى الوثيقة.
- استخدام `inline/block/start/end` وعدم افتراض left/right.
- الأرقام والأكواد والمستندات تبقى قابلة للقراءة باتجاه مناسب داخل RTL.
- ترتيب DOM منطقي للوحة المفاتيح في الاتجاهين، ولا يعكس بصرياً بطريقة تكسر tab order.

## 9. حالات النظام

كل archetype يدعم: initial loading، incremental loading، empty، filtered empty، permission denied، recoverable error،
offline/connection loss إن أمكن كشفها، success feedback، stale/concurrency، وdirty navigation. Skeletons تحاكي البنية ولا
تسبب layout shift كبير. الأخطاء تعرض correlation evidence وإجراء retry أو setup المسموح فقط.

## 10. حدود لا يجوز كسرها

- لا تعديل business rules أو API payloads لتسهيل الشكل دون قرار عقد مستقل.
- لا كشف IDs كاختيار أساسي؛ المنتجات والمستودعات والأطراف بالأسماء مع IDs داخلية.
- لا bypass لـApplication layer أو permission registry.
- لا تخزين translated labels كحالة مجال.
- لا ادعاء Saved views/Chatter/Activities/drag-drop/offline POS قبل عقودها.
- Public site، Login، Onboarding، Platform، Supplier Portal، وprint/PDF لا ترث shell الداخلي قسراً.

## 11. بوابة القبول

تغلق معمارية UX عندما:

- ترتبط كل route فاعلة بتطبيق ووحدة وarchetype أو exception موثقة.
- يملك كل archetype contract للمكونات والحالات وdesktop/mobile/RTL.
- لا تتكرر سلطة التنقل بين أكثر من registry.
- ينجح keyboard navigation وvisible focus وscreen-reader naming في prototype.
- يعتمد Products كـList/Form/Record pilot، ثم Sales document كـworkflow pilot قبل rollout واسع.
